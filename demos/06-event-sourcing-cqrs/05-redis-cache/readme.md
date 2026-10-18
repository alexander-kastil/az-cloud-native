---
minutes: 10
---

# Managed Redis: Change-Feed Cache Invalidation

A parcel's tracking page can be opened hundreds of times while the parcel is in transit, and each open would otherwise run a query against the `tracking` container. Azure Managed Redis answers those repeat reads from memory. The hard part is keeping the cache correct, because a scan that changes the parcel must remove the stale page.

This topic puts the tracking page behind the cache-aside pattern. The change feed projection invalidates the cached page after each scan commits, expirations bound any staleness, and the service connects with Microsoft Entra ID and gets its connection from Aspire.

## Cache-Aside for the Tracking Page

Cache-aside loads data into the cache on demand. The application checks the cache first, loads from the data store on a miss, stores the result with an expiration, and returns it. The Azure Architecture Center uses this pattern with Azure Managed Redis as the cache, and StackExchange.Redis as the .NET client.

1. Read the key `tracking:<trackingNumber>` from Redis.
2. On a hit, deserialize the cached page and return it.
3. On a miss, query the `tracking` container.
4. Store the serialized page with an expiration.
5. Return the page.

```csharp
using System.Text.Json;
using StackExchange.Redis;

public sealed class TrackingCache(IDatabase cache, TrackingQueryHandler queries)
{
    public async Task<IReadOnlyList<ScanRecord>> GetScansAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        string key = $"tracking:{trackingNumber}";

        RedisValue cached = await cache.StringGetAsync(key);

        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<List<ScanRecord>>(cached.ToString())!;
        }

        IReadOnlyList<ScanRecord> scans = await queries.HandleAsync(trackingNumber, cancellationToken);

        TimeSpan ttl = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(Random.Shared.Next(0, 30));

        await cache.StringSetAsync(key, JsonSerializer.Serialize(scans), ttl);

        return scans;
    }
}
```

The cache is an optimization, so a miss costs one query. The Architecture Center notes that cache-aside doesn't guarantee that the cache matches the store at every moment.

## Expiration and Jitter

Each cached page gets its own time to live.

The `StringSetAsync` overload with a `TimeSpan` sets a per-key expiration, and `KeyTimeToLive` reports what remains. `KeyExpireAsync` takes an absolute `DateTime` value. The expiration limits how long a missed invalidation can keep a stale page on screen.

Pages for many parcels can expire together when they were loaded in the same minute, and the resulting burst of queries lands on Cosmos DB at once. The code above adds a random offset of up to 30 seconds to each TTL, which spreads the expirations. Learn recommends the same jitter when simultaneous expiration would cause a large spike.

Choose the base TTL from how fast the data changes. A parcel in transit changes often, so a few minutes is a sensible ceiling. A delivered parcel rarely changes again, so its page can stay cached longer.

## Invalidating From the Change Feed

Invalidation happens in the projection, which runs on the change feed path. The projection runs after the scan commits, so the cache never removes a page for a write that failed. The Learn event-driven cache pattern uses the same order: the write commits to Cosmos DB first, then the change feed drives the Redis update.

The projection from the second topic gains a cache dependency and one line. It upserts the scan, then deletes the cached page. The delete runs with `KeyDeleteAsync`.

```csharp
using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

public sealed class TrackingProjection(Container tracking, IDatabase cache)
{
    public async Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<ShipmentEventDocument> changes, CancellationToken cancellationToken)
    {
        foreach (ShipmentEventDocument change in changes.Where(item => item.DocumentType == "event" && item.EventType == "ParcelScanned"))
        {
            ScanRecord scan = new(change.Id, change.TrackingNumber, change.ScanType, change.OccurredAt);

            await tracking.UpsertItemAsync(scan, new PartitionKey(scan.trackingNumber), cancellationToken: cancellationToken);

            await cache.KeyDeleteAsync($"tracking:{scan.trackingNumber}");
        }
    }
}
```

Deleting the key makes the next read load a fresh copy from the container. If the delete fails after the upsert, the processor retries the batch. The upsert is idempotent, so the retry is safe, and the TTL bounds how long the old page can survive if the retries keep failing.

## Entra ID Authentication and Aspire Wiring

`Microsoft.Azure.StackExchangeRedis` extends StackExchange.Redis so that the connection authenticates with a Microsoft Entra token. The extension refreshes the token before it expires, so the connection stays open for days. The Entra identity must also be added as a Redis user on the cache, on the Authentication page of the resource menu.

```csharp
using Azure.Identity;
using Microsoft.Azure.StackExchangeRedis;
using StackExchange.Redis;

ConfigurationOptions options = new()
{
    EndPoints = { "<cache-name>.<region>.redis.azure.net:10000" }
};

await options.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());

ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(options);

IDatabase cache = connection.GetDatabase();
```

Keep one `ConnectionMultiplexer` for the whole process, and register it as a singleton. Learn's cache-aside sample shares a single lazily created connection for the same reason.

Aspire wires the cache into the service for you. The AppHost adds the Azure Managed Redis resource, and `WithReference` passes its connection to the service. The Aspire integration uses Microsoft Entra ID by default, so no password lands in the connection string.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureManagedRedis("cache");

builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(cache);

builder.Build().Run();
```

The service project registers the client with the same resource name, which makes `IConnectionMultiplexer` available for dependency injection.

```csharp
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddRedisClient("cache");
```

Aspire's Azure Managed Redis integration calls the Azure provisioning support, so the AppHost needs a subscription and a location before it runs. Aspire 13.4 documents the `AddAzureManagedRedis` method, and older Aspire material shows `AddAzureRedis` for Azure Cache for Redis.

## Key Topics

- [What is Azure Managed Redis?](https://learn.microsoft.com/azure/redis/overview)
- [Cache-Aside pattern](https://learn.microsoft.com/azure/architecture/patterns/cache-aside)
- [Use Microsoft Entra ID for cache authentication with Azure Managed Redis](https://learn.microsoft.com/azure/redis/entra-for-authentication)
- [Quickstart: Use Azure Managed Redis in .NET Core](https://learn.microsoft.com/azure/redis/dotnet)
- [Set up Azure Cache for Redis in the AppHost](https://aspire.dev/integrations/cloud/azure/azure-cache-redis/azure-cache-redis-host/)
