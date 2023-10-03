using Microsoft.Azure.Cosmos;

namespace FoodApp.Orders
{
    public class OrderEventsStore : IOrderEventsStore
    {
        private Container container;
        public OrderEventsStore(
                string connectionString,
                string databaseName,
                string containerName)
        {
            CosmosClient client = new CosmosClient(connectionString);
            container = client.GetContainer(databaseName, containerName);
        }

        public async Task<string> CreateOrderEventAsync(OrderEvent @event)
        {
            await container.CreateItemAsync<OrderEvent>(@event, new PartitionKey(@event.Id));
            return @event.OrderId;
        }

        public async Task CancelOrderAsync(Order item)
        {
            item.CanceledByUser = true;
            await container.UpsertItemAsync<Order>(item, new PartitionKey(item.Id));
        }

    }
}