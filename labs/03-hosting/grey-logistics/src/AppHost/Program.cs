var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
shipmentEvents.AddServiceBusSubscription("tracking-subscription", "tracking");
shipmentEvents.AddServiceBusSubscription("exception-agent-subscription", "exception-agent");

var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsEmulator();
var logistics = cosmos.AddCosmosDatabase("logistics");
logistics.AddContainer("shipments", "/shipmentId");
logistics.AddContainer("tracking-container", "/trackingNumber", "tracking");

var shippingApi = builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging);

builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging);

builder.AddViteApp("ops-console", "../../web/ops-console")
    .WithReference(shippingApi)
    .WaitFor(shippingApi)
    .WithEnvironment("API_HTTPS", shippingApi.GetEndpoint("https"))
    .WithEnvironment("API_HTTP", shippingApi.GetEndpoint("http"))
    .WithExternalHttpEndpoints();

builder.AddUvicornApp("exception-agent", "../../agents/exception-agent", "main:app")
    .WithReference(messaging)
    .WaitFor(messaging);

builder.Build().Run();
