using Grey.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDaprClient();

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapDefaultEndpoints();

app.MapPost("/parcel-events", (ParcelScanned scan) => Results.Ok()).WithTopic("pubsub", "parcel-events");

app.Run();
