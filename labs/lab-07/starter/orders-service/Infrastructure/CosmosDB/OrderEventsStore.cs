using Microsoft.Azure.Cosmos;

namespace FoodApp
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

        public async Task<OrderEventResponse> CreateOrderEventAsync(OrderEvent evt)
        {
            var resp = await container.CreateItemAsync<OrderEvent>(evt, new PartitionKey(evt.Id));
            return new OrderEventResponse { 
                Id = resp.Resource.Id, 
                EventType = resp.Resource.EventType, 
                OrderId = resp.Resource.OrderId, 
                CustomerId = resp.Resource.CustomerId,
                Timestamp = resp.Resource.Timestamp
            };            
        }
    }
}