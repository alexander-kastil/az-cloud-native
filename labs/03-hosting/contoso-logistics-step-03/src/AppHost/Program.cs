var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var messaging = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
shipmentEvents.AddServiceBusSubscription("tracking-subscription", "tracking");
shipmentEvents.AddServiceBusSubscription("exception-agent-subscription", "exception-agent");

var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsEmulator();
var logistics = cosmos.AddCosmosDatabase("logistics");
logistics.AddContainer("shipments", "/shipmentId");
logistics.AddContainer("tracking-container", "/trackingNumber", "tracking");

var keyVault = builder.AddAzureKeyVault("key-vault");

var config = builder.AddAzureAppConfiguration("config");

var shippingApi =builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WithReference(keyVault)
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WithReference(config)
    .WaitFor(cosmos)
    .WaitFor(messaging)
    .WithExternalHttpEndpoints();

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
