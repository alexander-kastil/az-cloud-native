using Grey.Contracts;
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
            var data = new Dictionary<string, object?>(change.data, StringComparer.OrdinalIgnoreCase);
            var parcelScanned = new ParcelScanned(
                Guid.Parse(data[nameof(ParcelScanned.ParcelId)]!.ToString()!),
                Guid.Parse(data[nameof(ParcelScanned.ShipmentId)]!.ToString()!),
                data[nameof(ParcelScanned.TrackingNumber)]!.ToString()!,
                Enum.Parse<ScanType>(data[nameof(ParcelScanned.ScanType)]!.ToString()!),
                data[nameof(ParcelScanned.ScanLocation)]!.ToString()!,
                DateTimeOffset.Parse(data[nameof(ParcelScanned.ScannedAt)]!.ToString()!, null, System.Globalization.DateTimeStyles.RoundtripKind));
            await dapr.PublishEventAsync("pubsub", "parcel-events", parcelScanned, new Dictionary<string, string> { ["SessionId"] = parcelScanned.TrackingNumber }, cancellationToken);
        }
    }
}
