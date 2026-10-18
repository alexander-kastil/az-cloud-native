using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
builder.AddAzureAppConfiguration("config", configureOptions: options => options.Select("Tracking:*"));
builder.Services.AddHealthChecks().AddCheck<AppConfigHealthCheck>("app-configuration");
builder.Services.AddHostedService<ShipmentBookedProcessor>();
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("Contoso.Tracking"));
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

    var document = JsonNode.Parse(response.Content)!.AsObject();
    response.Dispose();
    document["schemaVersion"] = 2;

    return Results.Json(document);
});

app.Run();
