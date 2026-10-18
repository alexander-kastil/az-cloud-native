using System.Text.Json;
using Microsoft.Azure.Cosmos;

public sealed record AgentStateItem(string Id, string ParcelId, string Status, string? Draft = null, string? EventId = null, string? ShipmentId = null);

public static class AgentState
{
    public const string Database = "logistics";
    public const string ContainerName = "agent-state";

    public static Container GetContainer(this CosmosClient client) => client.GetContainer(Database, ContainerName);

    public static async Task<AgentStateItem?> ReadAsync(this Container container, string id, string parcelId)
    {
        using var response = await container.ReadItemStreamAsync(id, new PartitionKey(parcelId));
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<AgentStateItem>(response.Content, JsonSerializerOptions.Web);
    }
}
