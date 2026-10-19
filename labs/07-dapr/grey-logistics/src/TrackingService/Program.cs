using Microsoft.Azure.Cosmos;
using OpenTelemetry.Trace;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureCosmosClient("cosmos");
builder.AddAzureServiceBusClient("messaging");
builder.AddRedisClient("cache");
builder.Services.AddSingleton(services => services.GetRequiredService<IConnectionMultiplexer>().GetDatabase());
if (builder.Configuration.GetConnectionString("config") is not null)
{
    builder.AddAzureAppConfiguration("config", configureOptions: options => options.Select("Tracking:*"));
    builder.Services.AddHealthChecks().AddCheck<AppConfigHealthCheck>("app-configuration");
}
builder.Services.AddHostedService<ShipmentBookedProcessor>();
builder.Services.AddSingleton<TrackingContainerSetup>();
builder.Services.AddHostedService<TrackingProjectionHost>();
builder.Services.AddSingleton(services => new TrackingQueryHandler(services.GetRequiredService<CosmosClient>().GetContainer("logistics", "tracking")));
builder.Services.AddSingleton<TrackingCache>();
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource("Grey.Tracking"));
var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapGet("/tracking/{trackingNumber}", async (string trackingNumber, TrackingCache cache, CancellationToken cancellationToken) =>
    Results.Ok(await cache.GetScansAsync(trackingNumber, cancellationToken)));

app.Run();
