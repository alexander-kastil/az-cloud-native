using Microsoft.Azure.Cosmos;

public class ShipmentsContainerSetup
{
    public async Task<Container> EnsureAsync(Database database, CancellationToken cancellationToken)
    {
        var properties = new ContainerProperties("shipments", "/shipmentId")
        {
            DefaultTimeToLive = -1
        };

        var response = await database.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
        return response.Container;
    }
}
