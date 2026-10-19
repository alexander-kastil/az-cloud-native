using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Dapr;

const string StoreName = "statestore";
const int MaxAttempts = 3;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddDaprClient();
builder.Services.AddActors(options => options.Actors.RegisterActor<VanActor>());

var app = builder.Build();

app.MapDefaultEndpoints();
app.MapActorsHandlers();

app.MapPost("/reserve-capacity", async (ReserveCapacity command, DaprClient dapr, CancellationToken cancellationToken) =>
{
    var options = new StateOptions { Concurrency = ConcurrencyMode.FirstWrite };
    var van = ActorProxy.Create<IVanActor>(new ActorId(command.VanId), nameof(VanActor));
    var assigned = false;

    for (var attempt = 0; attempt < MaxAttempts; attempt++)
    {
        var (existing, etag) = await dapr.GetStateAndETagAsync<CapacityAssignment>(StoreName, command.ParcelId, cancellationToken: cancellationToken);
        if (existing is not null)
        {
            return Results.Conflict();
        }

        if (!assigned)
        {
            if (!await van.AssignParcelAsync(command.ParcelId))
            {
                return Results.Conflict();
            }

            assigned = true;
        }

        var assignment = new CapacityAssignment(command.ParcelId, command.VanId);
        try
        {
            if (await dapr.TrySaveStateAsync(StoreName, command.ParcelId, assignment, etag ?? string.Empty, options, cancellationToken: cancellationToken))
            {
                return Results.Ok(assignment);
            }
        }
        catch (DaprException)
        {
        }
    }

    return Results.Conflict();
});

app.Run();

record ReserveCapacity(string ParcelId, string VanId);

record CapacityAssignment(string ParcelId, string VanId);
