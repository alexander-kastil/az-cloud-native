var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddAzureServiceBus("messaging").RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
shipmentEvents.AddServiceBusSubscription("tracking-subscription", "tracking");
shipmentEvents.AddServiceBusSubscription("exception-agent-subscription", "exception-agent");

var cosmos = builder.AddAzureCosmosDB("cosmos").RunAsEmulator();
var logistics = cosmos.AddCosmosDatabase("logistics");
logistics.AddContainer("shipments", "/shipmentId");
logistics.AddContainer("tracking-container", "/trackingNumber", "tracking");

builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging);

builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cosmos)
    .WithReference(messaging)
    .WaitFor(cosmos)
    .WaitFor(messaging);

builder.Build().Run();
