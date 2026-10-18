using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
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

var activitySource = new ActivitySource("Contoso.Shipping");

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapPost("/shipments", async (BookShipment command, BookShipmentHandler handler, ILogger<Program> logger, CancellationToken cancellationToken) =>
{
    using var activity = activitySource.StartActivity("BookShipment");

    if (!await handler.HandleAsync(command, cancellationToken))
    {
        return Results.Conflict();
    }

    var trackingNumber = BookShipmentHandler.TrackingNumberFor(command.ShipmentId);
    logger.LogInformation("Shipment {ShipmentId} booked with tracking number {TrackingNumber}", command.ShipmentId, trackingNumber);

    return Results.Accepted($"/shipments/{command.ShipmentId}", new { command.ShipmentId, TrackingNumber = trackingNumber });
});

app.Run();
