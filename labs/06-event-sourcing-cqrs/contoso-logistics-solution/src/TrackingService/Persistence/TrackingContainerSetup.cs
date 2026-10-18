using Microsoft.Azure.Cosmos;

public class TrackingContainerSetup
{
    public async Task<Container> EnsureAsync(Database database, CancellationToken cancellationToken)
    {
        var properties = new ContainerProperties("tracking", "/trackingNumber");
        properties.IndexingPolicy.IndexingMode = IndexingMode.Consistent;
        properties.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
        properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/_etag/?" });
        properties.IndexingPolicy.CompositeIndexes.Add(
        [
            new CompositePath { Path = "/scanType", Order = CompositePathSortOrder.Ascending },
            new CompositePath { Path = "/occurredAt", Order = CompositePathSortOrder.Descending }
        ]);

        var response = await database.CreateContainerIfNotExistsAsync(properties, cancellationToken: cancellationToken);
        return response.Container;
    }
}
