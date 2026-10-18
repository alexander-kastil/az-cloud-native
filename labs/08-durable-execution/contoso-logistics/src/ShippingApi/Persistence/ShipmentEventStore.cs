using System.Net;
using Microsoft.Azure.Cosmos;

public class ShipmentEventStore(Container container)
{
    public async Task<bool> AppendAsync(ShipmentDocument shipmentEvent, CancellationToken cancellationToken)
    {
        var document = shipmentEvent with { id = $"{shipmentEvent.shipmentId}-v{shipmentEvent.version:D6}" };

        try
        {
            await container.CreateItemAsync(
                document,
                new PartitionKey(document.shipmentId),
                cancellationToken: cancellationToken);
            return true;
        }
        catch (CosmosException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            return false;
        }
    }
}
