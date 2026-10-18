using Microsoft.Azure.Cosmos;

public class ShipmentOutboxWriter(Container container)
{
    public async Task<bool> SaveBookingAsync(ShipmentDocument booked, ShipmentDocument outbox, CancellationToken cancellationToken)
    {
        var batch = container.CreateTransactionalBatch(new PartitionKey(booked.shipmentId))
            .CreateItem(booked)
            .CreateItem(outbox);

        using var response = await batch.ExecuteAsync(cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
