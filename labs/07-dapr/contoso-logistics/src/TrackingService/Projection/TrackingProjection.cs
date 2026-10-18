using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

public class TrackingProjection(Container container, IDatabase cache)
{
    public async Task HandleChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<ShipmentEventDocument> changes,
        CancellationToken cancellationToken)
    {
        foreach (var change in changes.Where(change => change.documentType == "event" && change.eventType == "ParcelScanned"))
        {
            var scan = new ScanRecord(change.id, change.trackingNumber, change.scanType!, change.occurredAt);
            await container.UpsertItemAsync(scan, new PartitionKey(scan.trackingNumber), cancellationToken: cancellationToken);
            await cache.KeyDeleteAsync($"tracking:{scan.trackingNumber}");
        }
    }
}
