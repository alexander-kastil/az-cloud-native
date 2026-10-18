using Microsoft.Azure.Cosmos;
using Microsoft.DurableTask;

public sealed class ReadProcessedDraft(CosmosClient cosmos) : TaskActivity<DelayedParcelInput, string?>
{
    public override async Task<string?> RunAsync(TaskActivityContext context, DelayedParcelInput input)
    {
        var item = await cosmos.GetContainer().ReadAsync(input.EventId, input.ParcelId);
        return item?.Status == "processed" ? item.Draft : null;
    }
}
