---
minutes: 10
---

# Durable Task SDK: KEDA-Scaled Workers

Contoso's routing workers assign vans to parcels, and their load rises and falls with the number of waiting orchestrations. They do not need the Functions host, so they run as plain .NET services on Azure Container Apps. The Durable Task SDK provides the worker and client libraries, and a KEDA scale rule adds replicas when the scheduler reports a backlog.

This topic shows how to register orchestrations and activities on a generic host, how a client starts an instance and waits for its result, and how the `azure-durabletask-scheduler` scale rule decides the replica count.

## Worker and Client Packages

The SDK ships a worker package and a client package. The `AzureManaged` variants connect to the Durable Task Scheduler and add the `UseDurableTaskScheduler` call to the builder. Install both packages in the worker project, and install only the client package in a service that starts instances.

```bash
dotnet add package Microsoft.DurableTask.Worker.AzureManaged
dotnet add package Microsoft.DurableTask.Client.AzureManaged
```

The worker registers its orchestrators and activities during host setup. The worker runs as a background service, so the same code works in a console app, a worker service, or an ASP.NET Core project.

```csharp
using Microsoft.DurableTask.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
string connectionString = builder.Configuration["DURABLE_TASK_SCHEDULER_CONNECTION_STRING"]!;

builder.Services.AddDurableTaskWorker()
    .AddTasks(registry =>
    {
        registry.AddOrchestrator<RouteOrchestrator>();
        registry.AddActivity<AssignVanActivity>();
    })
    .UseDurableTaskScheduler(connectionString);

builder.Build().Run();
```

The orchestrator derives from `TaskOrchestrator<TInput, TOutput>`, and the activity derives from `TaskActivity<TInput, TOutput>`. The orchestrator calls the activity with `nameof`, so renaming the class fails the build.

```csharp
public class RouteOrchestrator : TaskOrchestrator<string, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, string parcelId)
    {
        return await context.CallActivityAsync<string>(nameof(AssignVanActivity), parcelId);
    }
}

public sealed class AssignVanActivity : TaskActivity<string, string>
{
    public override Task<string> RunAsync(TaskActivityContext context, string parcelId)
    {
        return Task.FromResult($"van-17:{parcelId}");
    }
}
```

A service that starts instances registers the client with the same connection string. The client schedules the orchestration and then waits for the result.

```csharp
builder.Services.AddDurableTaskClient(clientBuilder => clientBuilder.UseDurableTaskScheduler(connectionString));
```

```csharp
public class RouteClient
{
    private readonly DurableTaskClient _client;

    public RouteClient(DurableTaskClient client)
    {
        _client = client;
    }

    public async Task<string> RouteParcel(string parcelId)
    {
        string instanceId = await _client.ScheduleNewOrchestrationInstanceAsync(nameof(RouteOrchestrator), parcelId);
        var metadata = await _client.WaitForInstanceCompletionAsync(instanceId, getInputsAndOutputs: true);
        return metadata.ReadOutputAs<string>();
    }
}
```

The scheduler pushes work items to the app as they arrive, so the worker does not poll for them. Each replica processes several work items in parallel, and it sends each result back when the task completes. The .NET SDK also generates work item filters from the tasks each worker registers, so a worker receives only the orchestrations and activities it hosts.

## Running Workers on Container Apps

The worker container app scales with a custom rule of type `azure-durabletask-scheduler`. The rule polls the scheduler for pending work items of one work item type. Each rule sets four metadata values and one identity, and the table explains each of them.

| Field | Purpose |
| --- | --- |
| `endpoint` | Scheduler endpoint that the scaler polls, such as `https://dts-ID.centralus.durabletask.io` |
| `taskhubName` | Task hub that holds the work items |
| `workItemType` | `Orchestration`, `Activity`, or `Entity` |
| `maxConcurrentWorkItemsCount` | Work items that one replica runs at once; lower values add replicas sooner |
| Scale rule identity | Managed identity linked to the scheduler and task hub |

The CLI command below creates the worker with one orchestration rule. Add a second rule with `workItemType` set to `Activity`, because the orchestration backlog and the activity backlog grow separately.

```bash
az containerapp create --resource-group rg-contoso-dts --name route-worker --environment ENVIRONMENT_ID --image contosoacr.azurecr.io/route-worker:1.0.0 --user-assigned USER_ASSIGNED_IDENTITY_ID --min-replicas 1 --max-replicas 10 --scale-rule-name dts-scaler-orchestration --scale-rule-type azure-durabletask-scheduler --scale-rule-metadata "endpoint=DTS_ENDPOINT" "maxConcurrentWorkItemsCount=1" "taskhubName=bookings" "workItemType=Orchestration" --scale-rule-identity USER_ASSIGNED_IDENTITY_ID
```

The Learn walkthrough describes the effect of this rule on a deployed worker. The scaler counts the pending orchestrations in the task hub, adds worker replicas as the backlog grows, and removes them as the load falls.

After each deployment, confirm that the new revision holds the orchestration and activity scale rules. The Learn sample checks the replicas on the revisions page after `azd up`. Adding or changing a scale rule creates a new revision, so review the rules in each revision you deploy.

## Tuning Concurrency and Scale-to-Zero

The `maxConcurrentWorkItemsCount` value sets how many work items a single replica takes at once. Start with 1 for CPU-intensive work, because a lower value makes the scaler add replicas sooner. Raise the value for I/O-bound work such as HTTP calls, which spends most of its time waiting on the network.

The Learn autoscaling guide applies to every Durable Task SDK app on Container Apps. The sample there uses the .NET SDK, but the guide states that autoscaling is language-agnostic, so a Python or Java worker uses the same rule type and metadata.

Set `minReplicas` to 0 to scale to zero when no work is waiting. The first work item after an idle period then waits for a cold start, so keep one replica for latency-sensitive routes. Durable Functions apps on Container Apps need the same Durable settings in the deployed `host.json` file, because the autoscaler reads that file and environment overrides alone do not generate the scale rules.

## Key Topics

- [Configure autoscaling for Durable Task SDK apps in Azure Container Apps](https://learn.microsoft.com/azure/durable-task/sdks/durable-task-scheduler-auto-scaling)
- [Durable Task SDKs overview](https://learn.microsoft.com/azure/durable-task/sdks/durable-task-overview)
- [Workflow in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/workflows-overview)
- [Set scaling rules in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/scale-app)
