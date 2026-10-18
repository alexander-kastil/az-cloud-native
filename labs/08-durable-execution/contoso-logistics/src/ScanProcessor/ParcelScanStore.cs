using System.Net;
using Microsoft.Azure.Cosmos;

namespace ScanProcessor;

public class ParcelScanStore(Container container)
{
    public async Task<bool> TryAppendAsync(string trackingNumber, string scanType, string messageId)
    {
        var parcelId = $"parcel-{trackingNumber}";
        var key = new PartitionKey(trackingNumber);

        var response = await ExecuteBatchAsync(key, parcelId, trackingNumber, scanType, messageId);

        if (response.Count > 1 && response[1].StatusCode == HttpStatusCode.NotFound)
        {
            await EnsureParcelAsync(key, parcelId, trackingNumber);
            response = await ExecuteBatchAsync(key, parcelId, trackingNumber, scanType, messageId);
        }

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.Count > 0 && response[0].StatusCode == HttpStatusCode.Conflict)
        {
            return false;
        }

        throw new CosmosException(response.ErrorMessage, response.StatusCode, 0, response.ActivityId, response.RequestCharge);
    }

    private async Task<TransactionalBatchResponse> ExecuteBatchAsync(PartitionKey key, string parcelId, string trackingNumber, string scanType, string messageId)
    {
        var marker = new { id = $"processed-{messageId}", trackingNumber };

        return await container.CreateTransactionalBatch(key)
            .CreateItem(marker)
            .PatchItem(parcelId, [PatchOperation.Add("/scans/-", scanType)])
            .ExecuteAsync();
    }

    private async Task EnsureParcelAsync(PartitionKey key, string parcelId, string trackingNumber)
    {
        try
        {
            await container.CreateItemAsync(new { id = parcelId, trackingNumber, scans = Array.Empty<string>() }, key);
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
        {
        }
    }
}
