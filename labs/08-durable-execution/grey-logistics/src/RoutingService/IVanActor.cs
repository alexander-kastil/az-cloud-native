using Dapr.Actors;

public interface IVanActor : IActor
{
    Task<bool> AssignParcelAsync(string parcelId);

    Task ReportPositionAsync(double latitude, double longitude);
}
