---
minutes: 10
---

# Read Models: Indexing, RUs, Consistency

The tracking page asks for one parcel, and the ops console searches the whole network by scan type and time. Cosmos DB charges request units (RUs) for both, and the bill depends on how the container is indexed, how the query is shaped, and which consistency level the read uses. A read model that ignores these settings can cost far more than it should.

This topic tunes the `tracking` container. You write an indexing policy with included, excluded, and composite paths, read the RU charge from each response, choose a point read over a query when the ID and partition key are known, and pass session tokens so a user reads back their own writes.

## Indexing Policy: Include, Exclude, Composite

Cosmos DB indexes every property of every item by default. Learn recommends an opt-out policy that indexes everything and excludes the paths you never filter on, because new properties then get indexed automatically. The `tracking` documents carry a large `rawScanPayload` field from the scanner that the search never filters on, so the policy excludes it.

```json
{
  "indexingMode": "consistent",
  "automatic": true,
  "includedPaths": [
    {
      "path": "/*"
    }
  ],
  "excludedPaths": [
    {
      "path": "/rawScanPayload/*"
    },
    {
      "path": "/_etag/?"
    }
  ],
  "compositeIndexes": [
    [
      {
        "path": "/scanType",
        "order": "ascending"
      },
      {
        "path": "/occurredAt",
        "order": "descending"
      }
    ]
  ]
}
```

The `id` and `_ts` properties are always indexed when the indexing mode is `consistent`. Learn also recommends keeping the partition key path in the index.

A composite index serves a query that sorts on more than one property, and it must match the sort order of that query. The composite above supports this query, which lists the newest scans of each type.

```sql
SELECT * FROM c ORDER BY c.scanType ASC, c.occurredAt DESC
```

Composite paths never use the `/*` or `/?` wildcards, and their names are case-sensitive. The .NET SDK exposes the policy through the `IndexingPolicy` property of `ContainerProperties`, and it accepts included and excluded paths there. The composite entry is easiest to keep in the JSON that you deploy with Bicep or the CLI.

```csharp
using Microsoft.Azure.Cosmos;

ContainerProperties properties = new("tracking", "/trackingNumber");
properties.IndexingPolicy.IncludedPaths.Add(new IncludedPath { Path = "/*" });
properties.IndexingPolicy.ExcludedPaths.Add(new ExcludedPath { Path = "/rawScanPayload/*" });

Container tracking = await database.CreateContainerAsync(properties);
```

## Reading the Request Charge

Every Cosmos DB response reports the RUs it used. In the .NET SDK, `ItemResponse<T>.RequestCharge` returns the charge as a double, and `ActivityId` identifies the request in diagnostics. Log both on every read that you tune, so you can compare a point read with a query before and after an index change.

```csharp
using Microsoft.Azure.Cosmos;

ItemResponse<ScanRecord> response = await tracking.ReadItemAsync<ScanRecord>(scanId, new PartitionKey(trackingNumber), cancellationToken: cancellationToken);

logger.LogInformation("Point read {ScanId} cost {RequestCharge} RU, activity {ActivityId}", scanId, response.RequestCharge, response.ActivityId);
```

Query pages report their charge through the `x-ms-request-charge` response header at the REST level, so the same measurement works for queries. Measure before you change the indexing policy, because index changes can take time to apply.

## Point Reads Versus Queries

A point read asks for one item by its `id` and partition key. It skips the query engine and costs the least. The tracking page can't use a point read when it wants every scan of a parcel, because it doesn't know the scan IDs in advance, so it runs a query scoped to one partition.

The partition key in `QueryRequestOptions` keeps that query on one partition. Without it, Cosmos DB fans the query out across every physical partition, and the RU charge grows with the partition count. The query code in the previous topic sets the partition key for this reason.

Use a point read when the caller holds the ID and the partition key. Use a single-partition query when the caller holds only the partition key. Use a cross-partition query only for the ops console search, and accept its cost.

## Session Consistency for Read-Your-Writes

Session is the default consistency level for Cosmos DB accounts. Within one session, a client reads its own writes and sees writes in order. Across sessions, the level offers no ordering guarantee, and a reader in another region can lag behind.

The five levels are strong, bounded staleness, session, consistent prefix, and eventual.

Each write returns a session token in `Headers.Session`. The SDK keeps the most recent token for each client, which works while the same client instance serves the user. A web tier with several instances loses that cache, so the token must travel back to the browser and return with the next request.

Session tokens are partition bound, so keep one token per tracking number.

Pass the token along with the request that reads back the write.

```csharp
using Microsoft.Azure.Cosmos;

ItemResponse<ScanRecord> written = await tracking.UpsertItemAsync(scan, new PartitionKey(scan.trackingNumber), cancellationToken: cancellationToken);

ItemRequestOptions options = new()
{
    SessionToken = written.Headers.Session
};

ItemResponse<ScanRecord> read = await tracking.ReadItemAsync<ScanRecord>(scan.id, new PartitionKey(scan.trackingNumber), options, cancellationToken);
```

Learn notes that a client without a session token for a partition reads with eventual behavior. A new client instance that hasn't written to that partition therefore sees eventual results until it writes or receives a token.

## Key Topics

- [Manage indexing policies in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/how-to-manage-indexing-policy)
- [Indexing policies in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/index-policy)
- [Performance tips for Azure Cosmos DB and .NET](https://learn.microsoft.com/azure/cosmos-db/performance-tips-dotnet-sdk-v3)
- [Consistency levels in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/consistency-levels)
- [Manage consistency levels in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/how-to-manage-consistency)
