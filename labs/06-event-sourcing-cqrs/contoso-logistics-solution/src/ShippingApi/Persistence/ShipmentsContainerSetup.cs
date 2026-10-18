using System.Net;
using Microsoft.Azure.Cosmos;

public class ShipmentsContainerSetup
{
    public async Task<Container> EnsureAsync(Database database, CancellationToken cancellationToken)
    {
        var properties = new ContainerProperties("shipments", "/shipmentId")
        {
            DefaultTimeToLive = -1
        };

        var created = await database.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
        if (created.StatusCode == HttpStatusCode.Created)
        {
            return created.Container;
        }

        var existing = created.Resource;
        existing.DefaultTimeToLive = -1;

        var replaced = await created.Container.ReplaceContainerAsync(existing, cancellationToken: cancellationToken);
        return replaced.Container;
    }
}
