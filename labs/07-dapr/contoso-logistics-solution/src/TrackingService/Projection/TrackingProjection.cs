using Contoso.Contracts;
using Dapr.Client;
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

public class TrackingProjection(Container container, IDatabase cache, DaprClient dapr)
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
            var parcelScanned = new ParcelScanned(
                Guid.Parse(change.id),
                Guid.Parse(change.shipmentId),
                change.trackingNumber,
                Enum.Parse<ScanType>(change.scanType!),
                change.data.GetValueOrDefault("ScanLocation")?.ToString() ?? string.Empty,
                change.occurredAt);
            await dapr.PublishEventAsync("pubsub", "parcel-events", parcelScanned, new Dictionary<string, string> { ["SessionId"] = change.trackingNumber }, cancellationToken);
        }
    }
}
