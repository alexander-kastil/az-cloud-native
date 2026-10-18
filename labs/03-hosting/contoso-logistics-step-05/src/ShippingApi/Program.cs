using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Contoso.Contracts;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
builder.AddAzureKeyVaultClient("key-vault");
builder.Services.AddHealthChecks().AddCheck<CarrierKeyHealthCheck>("carrier-api-key");
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("Contoso.Shipping"));
var app = builder.Build();

var activitySource = new ActivitySource("Contoso.Shipping");

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapPost("/shipments", async (BookShipmentRequest request, CosmosClient cosmos, ServiceBusClient serviceBus, ILogger<Program> logger) =>
{
    using var activity = activitySource.StartActivity("BookShipment");

    var container = cosmos.GetContainer("logistics", "shipments");
    await container.CreateItemAsync(
        new { id = Guid.NewGuid().ToString(), shipmentId = request.ShipmentId, trackingNumber = request.TrackingNumber },
        new PartitionKey(request.ShipmentId.ToString()));

    var booked = new ShipmentBooked(
        request.ShipmentId,
        request.TrackingNumber,
        "Contoso",
        new Address("1 Main Street", "Vienna", "1010", "AT"),
        "Medium",
        1m,
        "Standard",
        DateTimeOffset.UtcNow);

    await using var sender = serviceBus.CreateSender("shipment-events");
    await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(booked))
    {
        ContentType = "application/json"
    });

    logger.LogInformation("Shipment {ShipmentId} booked with tracking number {TrackingNumber}", request.ShipmentId, request.TrackingNumber);

    return Results.Created($"/shipments/{request.ShipmentId}", new { request.TrackingNumber });
});

app.Run();

record BookShipmentRequest(Guid ShipmentId, string TrackingNumber);
