using Microsoft.Azure.Cosmos;
using Microsoft.DurableTask;

public sealed record SaveRerouteProposalInput(DelayedParcelInput Parcel, string Draft);

public sealed class SaveRerouteProposal(CosmosClient cosmos) : TaskActivity<SaveRerouteProposalInput, bool>
{
    public override async Task<bool> RunAsync(TaskActivityContext context, SaveRerouteProposalInput input)
    {
        var parcel = input.Parcel;
        var proposal = new AgentStateItem($"reroute-{parcel.EventId}", parcel.ParcelId, "proposed", input.Draft, parcel.EventId, parcel.ShipmentId.ToString());
        var marker = new AgentStateItem(parcel.EventId, parcel.ParcelId, "processed", input.Draft, parcel.EventId, parcel.ShipmentId.ToString());

        using var response = await cosmos.GetContainer()
            .CreateTransactionalBatch(new PartitionKey(parcel.ParcelId))
            .UpsertItem(proposal)
            .UpsertItem(marker)
            .ExecuteAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"agent-state batch failed with {response.StatusCode}: {response.ErrorMessage}");
        }

        return true;
    }
}
