using Aspire.Hosting.Azure;
using Azure.Provisioning.ServiceBus;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var messaging = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
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

var publishing = builder.ExecutionContext.IsPublishMode;

var keyVault = publishing ? builder.AddAzureKeyVault("key-vault") : null;

var config = publishing ? builder.AddAzureAppConfiguration("config") : null;

var appInsights = publishing ? builder.AddAzureApplicationInsights("app-insights") : null;

var shippingApi = builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WithExternalHttpEndpoints();

var trackingService = builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WithExternalHttpEndpoints();

var scanProcessor = builder.AddAzureFunctionsProject<Projects.ScanProcessor>("scan-processor")
    .WithReference(cosmos)
    .WithReference(messaging, "ServiceBusConnection")
    .WaitFor(cosmos)
    .WaitFor(messaging);

if (publishing)
{
    shippingApi.WithReference(appInsights!).WithReference(keyVault!);
    trackingService.WithReference(appInsights!).WithReference(config!);
}

builder.AddViteApp("ops-console", "../../web/ops-console")
    .WithReference(shippingApi)
    .WaitFor(shippingApi)
    .WithEnvironment("API_HTTPS", shippingApi.GetEndpoint("https"))
    .WithEnvironment("API_HTTP", shippingApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints()
    .ExcludeFromManifest();

builder.AddUvicornApp("exception-agent", "../../agents/exception-agent", "main:app")
    .WithReference(messaging)
    .WaitFor(messaging)
    .ExcludeFromManifest();

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
