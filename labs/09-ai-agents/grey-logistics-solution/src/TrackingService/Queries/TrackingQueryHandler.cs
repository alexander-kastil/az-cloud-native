using Microsoft.Azure.Cosmos;

public class TrackingQueryHandler(Container container)
{
    public async Task<IReadOnlyList<ScanRecord>> HandleAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        var query = new QueryDefinition(
                "SELECT * FROM c WHERE c.trackingNumber = @trackingNumber AND IS_DEFINED(c.scanType) ORDER BY c.occurredAt ASC")
            .WithParameter("@trackingNumber", trackingNumber);

        var options = new QueryRequestOptions { PartitionKey = new PartitionKey(trackingNumber) };
        var scans = new List<ScanRecord>();

        using var iterator = container.GetItemQueryIterator<ScanRecord>(query, requestOptions: options);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            scans.AddRange(page);
        }

        return scans;
    }

    public async Task<IReadOnlyList<ScanRecord>> NewestScansByTypeAsync(CancellationToken cancellationToken)
    {
        var query = new QueryDefinition("SELECT * FROM c ORDER BY c.scanType ASC, c.occurredAt DESC");
        var scans = new List<ScanRecord>();

        using var iterator = container.GetItemQueryIterator<ScanRecord>(query);
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync(cancellationToken);
            scans.AddRange(page);
        }

        return scans;
    }
}
