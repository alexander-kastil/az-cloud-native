using Dapr.Actors.Runtime;

public class VanActor(ActorHost host, IConfiguration configuration) : Actor(host), IVanActor
{
    private const string ParcelCountKey = "parcel-count";
    private const string PositionKey = "last-position";

    private readonly int capacity = configuration.GetValue<int>("Van:Capacity");

    public async Task<bool> AssignParcelAsync(string parcelId)
    {
        var count = await StateManager.GetOrAddStateAsync(ParcelCountKey, 0);
        if (count >= capacity)
        {
            return false;
        }

        await StateManager.SetStateAsync(ParcelCountKey, count + 1);
        return true;
    }

    public Task ReportPositionAsync(double latitude, double longitude) =>
        StateManager.SetStateAsync(PositionKey, new VanPosition(latitude, longitude));
}

public record VanPosition(double Latitude, double Longitude);
