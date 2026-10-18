# Aspire Integrations: Hosting & Client

> **_NOTE:_** Duration: about 9 minutes.

Contoso needs Service Bus for events, Cosmos DB for shipment history and Storage for proof-of-delivery photos, and each dependency needs a connection on a laptop and in Azure.
This topic separates the hosting package that defines a resource from the client package that a service uses, and it shows what changes when the same code deploys.

## Hosting Packages Declare the Resources

A hosting integration runs in the AppHost. Its package adds methods to the distributed application builder, such as `AddAzureServiceBus`, and those methods create the resource in the model.

| Resource | Hosting package | Main calls |
| --- | --- | --- |
| Service Bus | `Aspire.Hosting.Azure.ServiceBus` | `AddAzureServiceBus`, `AddServiceBusTopic`, `AddServiceBusSubscription` |
| Cosmos DB | `Aspire.Hosting.Azure.CosmosDB` | `AddAzureCosmosDB`, `AddCosmosDatabase`, `AddContainer` |
| Storage | `Aspire.Hosting.Azure.Storage` | `AddAzureStorage`, `AddBlobs`, `AddBlobContainer` |

```bash
aspire add azure-service-bus
aspire add azure-cosmosdb
```

The `aspire add` command adds the hosting package and writes its version to `aspire.config.json`. Each hosting package also supplies the emulator option for its service, so the `RunAsEmulator` call sits next to the resource it replaces.

The split keeps ownership clear. The AppHost package carries the provisioning logic, and the client package carries only the SDK wiring. A service that references only the client package never creates a Service Bus namespace, so the namespace stays a resource that the AppHost owns.

## Client Packages Consume the Connection

A client integration runs in the service. It registers the SDK client with dependency injection, and it adds the health checks and OpenTelemetry instrumentation that the service needs.

| Client package | Registration call | Injected type |
| --- | --- | --- |
| `Aspire.Azure.Messaging.ServiceBus` | `AddAzureServiceBusClient(connectionName: "messaging")` | `ServiceBusClient` |
| `Aspire.Microsoft.Azure.Cosmos` | `AddAzureCosmosClient(connectionName: "cosmos")` | `CosmosClient` |
| `Aspire.Azure.Storage.Blobs` | `AddAzureBlobServiceClient("blobs")` | `BlobServiceClient` |

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddAzureServiceBusClient(connectionName: "messaging");

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
```

Health checks and OpenTelemetry instrumentation come with the client package, so the readiness endpoint reports a broken Service Bus connection without extra code. The `connectionName` must match the resource name in the AppHost, so the name is the contract between the two projects. The service receives the client through its constructor, as in `public class ShipmentEventSender(ServiceBusClient client)`, and it never builds a connection itself.

## RunAsEmulator for Local Development

`RunAsEmulator` replaces the Azure resource with a local container during `aspire run`. Each service has its own emulator, and each one changes what the developer needs installed.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
shipmentEvents.AddServiceBusSubscription("tracking");

var cosmos = builder.AddAzureCosmosDB("cosmos")
    .RunAsEmulator();
var logistics = cosmos.AddCosmosDatabase("logistics");
logistics.AddContainer("shipments", "/shipmentId");

var storage = builder.AddAzureStorage("storage")
    .RunAsEmulator();
var blobs = storage.AddBlobs("blobs");

builder.Build().Run();
```

| Service | Emulator | What to know |
| --- | --- | --- |
| Service Bus | Service Bus emulator container | Runs with a SQL Server companion container and has no Entra ID sign-in |
| Cosmos DB | vNext Linux emulator | `RunAsClassicEmulator()` selects the classic emulator, and Data Explorer works only with vNext |
| Storage | Azurite | Emulates blob, queue and table storage |

The Service Bus emulator pulls its container images on first use, so the first `aspire run` takes longer than the ones after it. Every emulator needs a container runtime on the developer machine.

Each emulator runs as a named container that the AppHost starts, so its logs appear in the dashboard next to the services that use it. The Cosmos DB emulator also accepts `WithDataVolume()`, which keeps its data across runs.

Local runs use the connection values that the AppHost injects, so the Service Bus emulator needs no Entra ID sign-in. The consuming code is the same on the Azure path, so a mistake in the connection setup appears on the laptop before it reaches Azure.

## Connection Injection at Run Time

`WithReference` injects each property of the resource as an environment variable named `[RESOURCE]_[PROPERTY]`. A Service Bus resource named `messaging` exposes `MESSAGING_ENDPOINT`, and a Cosmos DB account named `cosmos` exposes `COSMOS_CONNECTIONSTRING`.

The client package reads those values through its connection name, so the service code never contains an endpoint. A database or container resource adds its own name to the Cosmos DB connection string, so the consumer can target one container.

The injected names follow one rule for every resource. A developer who knows the rule can read a value from the environment without opening the AppHost.

## What Changes at Deploy Time

The AppHost code does not change between a laptop and Azure. Without `RunAsEmulator`, Aspire provisions a real Service Bus namespace during the run, so the AppHost needs an Azure subscription and location.

The emulator is a local stand-in, so a flow that passes against it still needs a run on the Azure path before a release. Deployment changes where the connection comes from, not how the client reads it. [Module 3](../../03-hosting/01-aspire-deploy/readme.md) covers `aspire deploy`, and the consuming code keeps the same `connectionName`.

## Key Topics

- [Set up Azure Service Bus in the AppHost](https://aspire.dev/integrations/cloud/azure/azure-service-bus/azure-service-bus-host/)
- [Connect to Azure Service Bus](https://aspire.dev/integrations/cloud/azure/azure-service-bus/azure-service-bus-connect/)
- [Set up Azure Cosmos DB in the AppHost](https://aspire.dev/integrations/cloud/azure/azure-cosmos-db/azure-cosmos-db-host/)
- [Connect to Azure Cosmos DB](https://aspire.dev/integrations/cloud/azure/azure-cosmos-db/azure-cosmos-db-connect/)
- [Set up Azure Storage Blobs in the AppHost](https://aspire.dev/integrations/cloud/azure/azure-storage-blobs/azure-storage-blobs-host/)
