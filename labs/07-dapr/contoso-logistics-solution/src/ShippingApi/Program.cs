using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Dapr.Client;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
builder.Services.AddDaprClient();
if (builder.Configuration.GetConnectionString("key-vault") is not null)
{
    builder.AddAzureKeyVaultClient("key-vault");
    builder.Services.AddHealthChecks().AddCheck<CarrierKeyHealthCheck>("carrier-api-key");
}
builder.Services.AddSingleton(services => services.GetRequiredService<ServiceBusClient>().CreateSender("shipment-events"));
builder.Services.AddSingleton(services => new ShipmentOutboxWriter(services.GetRequiredService<CosmosClient>().GetContainer("logistics", "shipments")));
builder.Services.AddSingleton<BookShipmentHandler>();
builder.Services.AddSingleton<OutboxRelay>();
builder.Services.AddHostedService<OutboxRelayHost>();
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("Contoso.Shipping"));
var app = builder.Build();

var cosmosClient = app.Services.GetRequiredService<CosmosClient>();
await new ShipmentsContainerSetup().EnsureAsync(cosmosClient.GetDatabase("logistics"), app.Lifetime.ApplicationStopping);

var activitySource = new ActivitySource("Contoso.Shipping");

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapPost("/shipments", async (BookShipment command, BookShipmentHandler handler, DaprClient dapr, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    using var activity = activitySource.StartActivity("BookShipment");

    if (!await handler.HandleAsync(command, cancellationToken))
    {
        return Results.Conflict();
    }

    await dapr.InvokeMethodAsync<ReserveCapacity, CapacityAssignment>("routing-service", "reserve-capacity", new ReserveCapacity(command.ShipmentId, command.VanId), cancellationToken);

    var trackingNumber = BookShipmentHandler.TrackingNumberFor(command.ShipmentId);
    logger.LogInformation("Shipment {ShipmentId} booked with tracking number {TrackingNumber}", command.ShipmentId, trackingNumber);

    return Results.Accepted($"/shipments/{command.ShipmentId}", new { command.ShipmentId, TrackingNumber = trackingNumber });
});

app.Run();
