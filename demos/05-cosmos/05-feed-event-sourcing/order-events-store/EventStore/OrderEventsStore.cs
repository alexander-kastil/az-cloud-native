using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<OrderEventResponse> AddEventAsync(OrderEvent evt)
        {
            var resp = await container.CreateItemAsync<OrderEvent>(evt, new PartitionKey(evt.Id));
            return new OrderEventResponse { 
                Id = resp.Resource.Id, 
                EventType = resp.Resource.EventType, 
                OrderId = resp.Resource.OrderId, 
                Timestamp = resp.Resource.Timestamp,
                CustomerId = resp.Resource.CustomerId
            };
        }

        public async Task<IEnumerable<Order>> GetAllEventsForOrder(string queryString)
        {
            var query = container.GetItemQueryIterator<Order>(new QueryDefinition(queryString));
            List<Order> results = new List<Order>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }


        public async Task<Order> GetOrderAsync(string id, string customerId)
        {
            try
            {
                ItemResponse<Order> response = await container.ReadItemAsync<Order>(id, new PartitionKey(customerId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
    }
}