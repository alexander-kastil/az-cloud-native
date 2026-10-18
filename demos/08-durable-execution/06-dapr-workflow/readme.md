# Dapr Workflow: Portable Orchestrations

> **_NOTE:_** Duration: about 9 minutes.

Contoso runs several services on AKS through Dapr, and the booking workflow should use the same sidecar model as the services it calls. Dapr Workflow gives the orchestration similar durable behavior to the Durable Task model, with a runtime that sits next to each service. The state store and the messaging components are configuration, so the workflow code does not change when those components change.

This topic covers the `Dapr.Workflow` package for .NET, the Dapr 1.18 workflow features, and the local and Azure hosting options. It ends with a comparison against the Durable Task model from topics 3 and 5, so you can choose between them for a given service.

## Workflows and Activities in Dapr.Workflow

A workflow derives from `Workflow<TInput, TOutput>` and overrides `RunAsync` with a `WorkflowContext`. An activity derives from `WorkflowActivity<TInput, TOutput>` and overrides `RunAsync` with a `WorkflowActivityContext`. The workflow calls the activity with `CallActivityAsync<T>` and the activity's name.

```csharp
public sealed class ParcelBookingWorkflow : Workflow<string, string>
{
    public override async Task<string> RunAsync(WorkflowContext context, string parcelId)
    {
        return await context.CallActivityAsync<string>(nameof(ReserveCapacityActivity), parcelId);
    }
}

public sealed class ReserveCapacityActivity : WorkflowActivity<string, string>
{
    public override Task<string> RunAsync(WorkflowActivityContext context, string parcelId)
    {
        return Task.FromResult($"capacity-{parcelId}");
    }
}
```

Register the services with `AddDaprWorkflow`. Dapr 1.18 registers the workflow and activity classes itself, so the registration needs no options delegate. Code written for earlier versions can still call `RegisterWorkflow<T>` inside that delegate.

```csharp
builder.Services.AddDaprWorkflow();
```

A service schedules the workflow through an injected `DaprWorkflowClient`. The call returns the instance ID, and `GetWorkflowStateAsync` reads the current state of that instance.

```csharp
public static Task<string> StartBooking(DaprWorkflowClient daprWorkflowClient, string parcelId)
{
    return daprWorkflowClient.ScheduleNewWorkflowAsync(nameof(ParcelBookingWorkflow), input: parcelId);
}
```

Dapr persists the result of each completed step, so a workflow resumes from its last completed step after a crash, restart, or redeployment. The workflow overview page does not list code rules for workflow authors, so check the authoring guidance before you port an existing orchestrator from the Durable Task model.

## Dapr 1.18 Workflow Features

Dapr 1.18 shipped on 10 June 2026 with a focus on security, durability, and scale for workflows. Workflow history can be signed and verified on every state load, so tampering shows up when the state is read. Signing needs mTLS, and the Dapr docs describe it as a per-workflow feature flag.

The release also attests child workflow and activity completions across applications. The new `WorkflowAccessPolicy` resource is a Kubernetes custom resource, and it controls which app IDs can invoke which workflows and activities.

Workflows can propagate slices of their history to the children they schedule. The scheduler caps how many workflows and activities run at once across all sidecar replicas.

The 1.18 release also removes the need for the `Dapr.Workflow.Versioning` and `Dapr.Workflow.Analyzers` package references, so remove them when you upgrade. The `Dapr.Workflow` package is consolidated in the same release.

## Hosting Dapr Workflows on Azure

Azure Container Apps offers managed Dapr, but its documentation states that "the Dapr server extensions, actor, and workflow SDK packages aren't compatible with Azure Container Apps". Workflow code that uses `Dapr.Workflow` therefore runs on AKS, where the Dapr cluster extension installs the runtime. The extension supports the current and the previous Dapr version, so the team must upgrade regularly to stay within that window.

For local development, the Aspire integration runs the sidecar through the `CommunityToolkit.Aspire.Hosting.Dapr` package. The AppHost attaches the sidecar to a project, and the sidecar takes the resource name as its app ID. The Dapr CLI must be installed, and `dapr init` must have run on the machine first.

```csharp
var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.BookingWorker>("booking-worker").WithDaprSidecar();
builder.Build().Run();
```

The toolkit documentation lists the Dapr runtime version it tests against, and that version is older than 1.18. Check the package's compatibility notes before you run 1.18 workflows through Aspire.

## Comparing Dapr Workflow with Durable Task

Both models persist progress and resume after a failure, so the choice depends on hosting, portability, and operations. The table compares the properties that matter for a service in this class.

| Concern | Durable Task (Functions and SDKs) | Dapr Workflow |
| --- | --- | --- |
| .NET package | `Microsoft.DurableTask.Worker.AzureManaged`, or the Functions extension | `Dapr.Workflow` |
| State backend | Durable Task Scheduler | A state store component that supports workflows |
| Start and read state | `ScheduleNewOrchestrationInstanceAsync`, status URLs | `ScheduleNewWorkflowAsync`, `GetWorkflowStateAsync` |
| External events | `RaiseEventAsync` | Raise event through the HTTP or gRPC management API |
| Resume after failure | Replays orchestrator code under determinism rules | Resumes from the last completed step |
| Azure hosting | Functions, Container Apps, AKS, App Service, and VMs | AKS through the Dapr extension; not Container Apps managed Dapr |
| Upgrade model | Azure-managed backend | Dapr runtime version that the team schedules |

Choose Durable Task when the service runs on Functions or Container Apps and the backend should be a managed Azure service. Choose Dapr Workflow when the service already runs on AKS with Dapr, and the team wants the same component model for state, messaging, and workflows.

## Key Topics

- [Dapr workflow overview](https://docs.dapr.io/developing-applications/building-blocks/workflow/workflow-overview/)
- [Dapr .NET SDK workflow](https://docs.dapr.io/developing-applications/sdks/dotnet/dotnet-workflow/)
- [Microservice APIs powered by Dapr in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/dapr-overview)
- [Dapr extension for Azure Kubernetes Service](https://learn.microsoft.com/azure/aks/dapr-overview)
- [Dapr integration with Aspire](https://aspire.dev/integrations/frameworks/dapr/)
