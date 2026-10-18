using System.Net;
using Microsoft.Azure.Cosmos;

namespace ScanProcessor;

public class ParcelScanStore(Container container)
{
    public async Task<bool> TryAppendAsync(string trackingNumber, string scanType, string messageId)
    {
        var id = $"parcel-{trackingNumber}";
        var key = new PartitionKey(trackingNumber);

        try
        {
            await container.PatchItemAsync<object>(id, key, [PatchOperation.Add("/scans/-", scanType)]);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            try
            {
                await container.CreateItemAsync(new { id, trackingNumber, scans = new[] { scanType } }, key);
            }
            catch (CosmosException conflict) when (conflict.StatusCode == HttpStatusCode.Conflict)
            {
                await container.PatchItemAsync<object>(id, key, [PatchOperation.Add("/scans/-", scanType)]);
            }
        }

        return true;
    }
}
