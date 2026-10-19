using System.Net;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
builder.Services.AddHostedService<ShipmentBookedProcessor>();
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("Grey.Tracking"));
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapGet("/tracking/{trackingNumber}", async (string trackingNumber, CosmosClient cosmos) =>
{
    var container = cosmos.GetContainer("logistics", "tracking");
    var response = await container.ReadItemStreamAsync(trackingNumber, new PartitionKey(trackingNumber));

    if (response.StatusCode == HttpStatusCode.NotFound)
    {
        response.Dispose();
        return Results.NotFound();
    }

    return Results.Stream(response.Content, "application/json");
});

app.Run();
