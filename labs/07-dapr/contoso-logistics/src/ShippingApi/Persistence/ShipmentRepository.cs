using Microsoft.Azure.Cosmos;

public class ShipmentRepository(Container container)
{
    public async Task<Shipment> LoadAsync(string shipmentId, CancellationToken cancellationToken)
    {
        var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.shipmentId = @shipmentId AND c.documentType = @documentType ORDER BY c.version ASC")
            .WithParameter("@shipmentId", shipmentId)
            .WithParameter("@documentType", "event");

        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(shipmentId) };
        var shipment = Shipment.Empty(shipmentId);

        using var iterator = container.GetItemQueryIterator<ShipmentDocument>(query, requestOptions: options);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            foreach (var document in page)
            {
                shipment = shipment.Apply(document);
            }
        }

        return shipment;
    }
}
