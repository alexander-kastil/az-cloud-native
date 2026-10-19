using System.Text.Json;
using StackExchange.Redis;

public class TrackingCache(IDatabase database, TrackingQueryHandler queryHandler)
{
    public async Task<IReadOnlyList<ScanRecord>> GetScansAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        var key = $"tracking:{trackingNumber}";
        var cached = await database.StringGetAsync(key);
        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<List<ScanRecord>>(cached.ToString())!;
        }

        var scans = await queryHandler.HandleAsync(trackingNumber, cancellationToken);
        var timeToLive = TimeSpan.FromMinutes(5) + TimeSpan.FromSeconds(Random.Shared.Next(0, 31));
        await database.StringSetAsync(key, JsonSerializer.Serialize(scans), timeToLive);
        return scans;
    }
}
