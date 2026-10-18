using Microsoft.Azure.Cosmos;
using Microsoft.DurableTask;
using Microsoft.Extensions.AI;

public sealed class DraftCustomerMessage(IChatClient chatClient, CosmosClient cosmos) : TaskActivity<DelayedParcelInput, string>
{
    private const string SystemMessage = "You write short, polite customer notices about delayed parcels. Keep the notice to a few sentences.";

    public override async Task<string> RunAsync(TaskActivityContext context, DelayedParcelInput input)
    {
        var container = cosmos.GetContainer();

        var stored = await container.ReadAsync(input.EventId, input.ParcelId);
        if (stored?.Draft is not null)
        {
            return stored.Draft;
        }

        var response = await chatClient.GetResponseAsync(
            [
                new ChatMessage(ChatRole.System, SystemMessage),
                new ChatMessage(ChatRole.User, $"Shipment {input.ShipmentId}, parcel {input.ParcelId} is delayed.")
            ],
            new ChatOptions { MaxOutputTokens = 1000 });

        var item = new AgentStateItem(input.EventId, input.ParcelId, "drafted", response.Text, input.EventId, input.ShipmentId.ToString());
        await container.UpsertItemAsync(item, new PartitionKey(input.ParcelId));

        return response.Text;
    }
}
