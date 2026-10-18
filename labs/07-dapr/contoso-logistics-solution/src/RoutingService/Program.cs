using Dapr.Client;

const string StoreName = "statestore";
const int MaxAttempts = 3;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDaprClient();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/reserve-capacity", async (ReserveCapacity command, DaprClient dapr, CancellationToken cancellationToken) =>
{
    var options = new StateOptions { Concurrency = ConcurrencyMode.FirstWrite };

    for (var attempt = 0; attempt < MaxAttempts; attempt++)
    {
        var (existing, etag) = await dapr.GetStateAndETagAsync<CapacityAssignment>(StoreName, command.ParcelId, cancellationToken: cancellationToken);
        if (existing is not null)
        {
            return Results.Conflict();
        }

        var assignment = new CapacityAssignment(command.ParcelId, command.VanId);
        if (await dapr.TrySaveStateAsync(StoreName, command.ParcelId, assignment, etag ?? string.Empty, options, cancellationToken: cancellationToken))
        {
            return Results.Ok(assignment);
        }
    }

    return Results.Conflict();
});

app.Run();

record ReserveCapacity(string ParcelId, string VanId);

record CapacityAssignment(string ParcelId, string VanId);
