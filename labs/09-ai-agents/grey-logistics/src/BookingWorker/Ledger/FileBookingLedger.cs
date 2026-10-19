using System.Text.Json;

namespace BookingWorker.Ledger;

public sealed class FileBookingLedger(string path) : IBookingLedger
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);
    private readonly SemaphoreSlim gate = new(1, 1);

    public async Task AppendAsync(string parcelId, string activity)
    {
        var entry = new LedgerEntry(parcelId, activity, $"{parcelId}:{activity}", DateTimeOffset.UtcNow);
        var line = JsonSerializer.Serialize(entry, Json) + Environment.NewLine;
        await gate.WaitAsync();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.AppendAllTextAsync(path, line);
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<IReadOnlyList<string>> ReadActivitiesAsync(string parcelId)
    {
        await gate.WaitAsync();
        try
        {
            if (!File.Exists(path))
            {
                return [];
            }

            var lines = await File.ReadAllLinesAsync(path);
            return lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l => JsonSerializer.Deserialize<LedgerEntry>(l, Json)!)
                .Where(e => e.ParcelId == parcelId)
                .Select(e => e.Activity)
                .ToList();
        }
        finally
        {
            gate.Release();
        }
    }

    private sealed record LedgerEntry(string ParcelId, string Activity, string Key, DateTimeOffset At);
}
