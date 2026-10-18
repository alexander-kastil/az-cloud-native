using Aspire.Hosting.Azure;
using Azure.Provisioning.ServiceBus;
using CommunityToolkit.Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddDapr();

builder.AddAzureContainerAppEnvironment("aca-env");

var messaging = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events").WithProperties(topic =>
{
    topic.RequiresDuplicateDetection = true;
    topic.DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(5);
});
shipmentEvents.AddServiceBusSubscription("tracking-subscription", "tracking");
shipmentEvents.AddServiceBusSubscription("exception-agent-subscription", "exception-agent");

var parcelEvents = messaging.AddServiceBusTopic("parcel-events").WithProperties(topic =>
{
    topic.RequiresDuplicateDetection = true;
    topic.DuplicateDetectionHistoryTimeWindow = TimeSpan.FromMinutes(5);
});
AddSubjectSubscription(parcelEvents, "tracking", requiresSession: true, "ParcelScanned");
AddSubjectSubscription(parcelEvents, "customer-messages", requiresSession: false, "ParcelDelivered", "DeliveryDelayed");
AddSubjectSubscription(parcelEvents, "exceptions", requiresSession: false, "DeliveryDelayed");
messaging.ConfigureInfrastructure(infrastructure =>
{
    foreach (var rule in infrastructure.GetProvisionableResources().OfType<ServiceBusRule>())
    {
        rule.BicepIdentifier = $"{rule.Parent.BicepIdentifier}_{rule.BicepIdentifier}";
    }
});

var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsEmulator();
var logistics = cosmos.AddCosmosDatabase("logistics");
logistics.AddContainer("shipments", "/shipmentId");
logistics.AddContainer("tracking-container", "/trackingNumber", "tracking");

EndpointReference? kafkaEndpoint = null;
var eventHubs = builder.AddAzureEventHubs("event-hubs").RunAsEmulator(emulator =>
{
    emulator.WithEndpoint(port: 9092, targetPort: 9092, name: "kafka");
    kafkaEndpoint = emulator.GetEndpoint("kafka");
    emulator.WithConfiguration(document =>
    {
        foreach (var entity in document["UserConfig"]!["NamespaceConfig"]![0]!["Entities"]!.AsArray())
        {
            if (entity!["Name"]!.GetValue<string>() == "van-positions")
            {
                entity["PartitionCount"] = 4;
            }
        }
    });
});
eventHubs.AddHub("van-positions").AddConsumerGroup("ops-console-group", "ops-console");

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blobs = storage.AddBlobs("blobs");
storage.AddBlobContainer("checkpoints", "ops-console");

var publishing = builder.ExecutionContext.IsPublishMode;

var keyVault = publishing ? builder.AddAzureKeyVault("key-vault") : null;

var config = publishing ? builder.AddAzureAppConfiguration("config") : null;

var appInsights = publishing ? builder.AddAzureApplicationInsights("app-insights") : null;

var pubsub = builder.AddDaprComponent("pubsub", "pubsub.redis", new DaprComponentOptions { LocalPath = Path.Combine("components", "pubsub-local.yaml") });
var statestore = builder.AddDaprComponent("statestore", "state.redis", new DaprComponentOptions { LocalPath = Path.Combine("components", "statestore-local.yaml") });

var shippingApi = builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WithEnvironment("Outbox__RelayEnabled", builder.Configuration["Outbox:RelayEnabled"] ?? "true")
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WithDaprSidecar(sidecar => sidecar.WithReference(pubsub).WithReference(statestore))
    .WithExternalHttpEndpoints();

var cache = builder.AddRedis("cache");

var trackingService = builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WithReference(cache)
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WaitFor(cache)
    .WithDaprSidecar(sidecar => sidecar.WithReference(pubsub).WithReference(statestore))
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.NotificationService>("notification-service")
    .WithDaprSidecar(sidecar => sidecar.WithReference(pubsub).WithReference(statestore));

builder.AddProject<Projects.RoutingService>("routing-service")
    .WithDaprSidecar(sidecar => sidecar.WithReference(statestore));

var scanProcessor =builder.AddAzureFunctionsProject<Projects.ScanProcessor>("scan-processor")
    .WithReference(cosmos)
    .WithReference(messaging, "ServiceBusConnection")
    .WaitFor(cosmos)
    .WaitFor(messaging);

if (publishing)
{
    shippingApi.WithReference(appInsights!).WithReference(keyVault!);
    trackingService.WithReference(appInsights!).WithReference(config!);
}

var telemetryIngest = builder.AddProject<Projects.TelemetryIngest>("telemetry-ingest")
    .WithReference(eventHubs)
    .WithReference(blobs)
    .WaitFor(eventHubs)
    .WaitFor(blobs)
    .WithExternalHttpEndpoints();

var opsConsole = builder.AddViteApp("ops-console", "../../web/ops-console")
    .WithReference(shippingApi)
    .WaitFor(shippingApi)
    .WithEnvironment("API_HTTPS", shippingApi.GetEndpoint("https"))
    .WithEnvironment("API_HTTP", shippingApi.GetEndpoint("http"))
    .WithEnvironment("TELEMETRY_HTTPS", telemetryIngest.GetEndpoint("https"))
    .WithEnvironment("TELEMETRY_HTTP", telemetryIngest.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .ExcludeFromManifest();

telemetryIngest.WithEnvironment("OpsConsole__Origin", opsConsole.GetEndpoint("http"));

builder.AddUvicornApp("exception-agent", "../../agents/exception-agent", "main:app")
    .WithReference(messaging)
    .WaitFor(messaging)
    .ExcludeFromManifest();

builder.AddProject<Projects.VanSimulator>("van-simulator")
    .WithReference(eventHubs)
    .WithEnvironment("KAFKA_BOOTSTRAP_SERVERS", ReferenceExpression.Create($"{kafkaEndpoint!.Property(EndpointProperty.HostAndPort)}"))
    .WaitFor(eventHubs);

builder.Build().Run();

static void AddSubjectSubscription(IResourceBuilder<AzureServiceBusTopicResource> topic, string name, bool requiresSession, params string[] subjects) =>
    topic.AddServiceBusSubscription(name).WithProperties(subscription =>
    {
        subscription.RequiresSession = requiresSession;
        foreach (var subject in subjects)
        {
            subscription.Rules.Add(new AzureServiceBusRule(subject)
            {
                FilterType = AzureServiceBusFilterType.CorrelationFilter,
                CorrelationFilter = new AzureServiceBusCorrelationFilter { Subject = subject }
            });
        }
    });
