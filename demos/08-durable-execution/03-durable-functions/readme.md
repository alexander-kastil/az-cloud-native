# Durable Functions: Orchestrations & Entities

> **_NOTE:_** Duration: about 8 minutes.

The parcel route check runs as a function in Azure Functions, and it needs to remember what happened between scans. Contoso Logistics also tracks how many parcels each van carries, and many scanners change that count at the same time. Durable Functions adds orchestrations and entities to Functions, so the workflow and its state persist on the Durable Task Scheduler without a database you manage.

The in-process model reaches end of support on 10 November 2026, so this topic uses the .NET isolated worker model. It shows the three function kinds, the host configuration that points the app at the scheduler, and how a client starts an instance and reads its status.

## The Isolated Worker Model

The isolated worker uses the Durable Functions extension package, and the Durable Task Scheduler backend needs the `AzureManaged` package. The quickstart installs the latest prerelease, and it requires the base extension at version 1.2.2 or later.

```bash
dotnet add package Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged --prerelease
```

An orchestrator takes a `TaskOrchestrationContext` and calls activities by name. An activity takes an `ActivityTrigger` parameter and returns its result to the orchestrator. The starter uses the `DurableClient` binding to schedule the instance and to return the status URLs.

```csharp
public class ParcelRouteFunctions
{
    [Function(nameof(ParcelRouteOrchestrator))]
    public static async Task<string> ParcelRouteOrchestrator(
        [OrchestrationTrigger] TaskOrchestrationContext context, string parcelId)
    {
        return await context.CallActivityAsync<string>(nameof(RecordScan), parcelId);
    }

    [Function(nameof(RecordScan))]
    public static string RecordScan([ActivityTrigger] string parcelId)
    {
        return $"Scanned {parcelId}";
    }

    [Function("StartRoute")]
    public static async Task<HttpResponseData> StartRoute(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "routes/{parcelId}")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        string parcelId)
    {
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(ParcelRouteOrchestrator), parcelId);
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}
```

The orchestrator receives the parcel ID as a plain parameter, so it needs no call to `GetInput`. The activity is an ordinary function that can perform I/O. Only orchestrators must stay deterministic, so route every service call through an activity.

## Durable Entities for Van Load

An entity holds a small piece of state and accepts operations from many callers. Contoso models each van as one entity, keyed by the van ID, and the entity stores how many parcels the van carries. The class below uses the POCO form, where the framework serializes the whole object.

```csharp
public class VanLoad
{
    public int Parcels { get; set; }

    public void Load(int count)
    {
        Parcels += count;
    }

    public void Unload(int count)
    {
        Parcels -= count;
    }

    public int Get()
    {
        return Parcels;
    }

    [Function(nameof(VanLoad))]
    public static Task Run([EntityTrigger] TaskEntityDispatcher dispatcher)
    {
        return dispatcher.DispatchAsync<VanLoad>();
    }
}
```

The `TaskEntity<TState>` base class offers the other form. It stores the state in a `State` property and supports constructor injection, while the POCO form serializes the whole object. Both forms use the same entity trigger, so clients and orchestrators call them the same way.

Clients use `Entities` on the `DurableTaskClient`. A signal is one-way, so the caller never sees the result. A read returns a snapshot of the state, which can be stale by the time your code uses it.

```csharp
public static async Task<int> LoadVan(DurableTaskClient client, int count)
{
    var entityId = new EntityInstanceId(nameof(VanLoad), "van-17");
    await client.Entities.SignalEntityAsync(entityId, "Load", count);
    EntityMetadata<int>? van = await client.Entities.GetEntityAsync<int>(entityId);
    return van?.State ?? 0;
}
```

An orchestrator uses the same operations through its context. Signals and calls share the `Entities` property, and a call waits for the operation's result.

```csharp
[Function(nameof(VanLoadOrchestrator))]
public static async Task<int> VanLoadOrchestrator(
    [OrchestrationTrigger] TaskOrchestrationContext context, string vanId)
{
    var entityId = new EntityInstanceId(nameof(VanLoad), vanId);
    await context.Entities.SignalEntityAsync(entityId, "Load", 1);
    return await context.Entities.CallEntityAsync<int>(entityId, "Get");
}
```

An entity processes its operations one at a time. Concurrent signals to the same van therefore queue up on that entity, and no two operations update its state at once.

The isolated model serializes state with System.Text.Json, which compares property names case-sensitively. Keep the state type simple, and keep each entity under the 1 MB state limit.

## Host Configuration and the azureManaged Provider

The `host.json` file selects the storage provider. The `azureManaged` type points the Durable Functions extension at the Durable Task Scheduler, and the `connectionStringName` value names an app setting that holds the connection string.

```json
{
  "version": "2.0",
  "extensions": {
    "durableTask": {
      "hubName": "%TASKHUB_NAME%",
      "storageProvider": {
        "type": "azureManaged",
        "connectionStringName": "DURABLE_TASK_SCHEDULER_CONNECTION_STRING"
      }
    }
  }
}
```

The setting name belongs in `host.json`, and the connection string itself belongs in an app setting. The `hubName` must match the task hub that holds the instances, because a mismatch makes the worker poll a different hub.

For local runs, set the value in `local.settings.json` to the emulator endpoint. Azure apps store the managed identity connection string in the function app settings.

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "DURABLE_TASK_SCHEDULER_CONNECTION_STRING": "Endpoint=http://localhost:8080;TaskHub=default;Authentication=None",
    "TASKHUB_NAME": "default"
  }
}
```

## Client Starts and Status Queries

The starter returns the response from `CreateCheckStatusResponseAsync`. The body contains `statusQueryGetUri`, and a GET on that URL returns the instance's `runtimeStatus` and, when it completes, its output. Instance IDs that you supply must stay within 100 printable ASCII characters, and IDs that start with `@` are reserved for entities.

Poll the status URL from a test client, or open the dashboard from topic 2 to inspect the history. Topic 4 adds the `RaiseEventAsync` call, which sends an external event to an instance that is already running.

## Key Topics

- [Durable Functions overview in the .NET isolated worker](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-dotnet-isolated-overview)
- [Bindings for Durable Functions](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-bindings)
- [Durable entities in .NET developer's guide (isolated)](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-dotnet-entities)
- [Durable Functions settings in host.json](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-host-json-settings)
- [Quickstart: Configure a Durable Functions app to use Durable Task Scheduler](https://learn.microsoft.com/azure/durable-task/scheduler/quickstart-durable-task-scheduler)
