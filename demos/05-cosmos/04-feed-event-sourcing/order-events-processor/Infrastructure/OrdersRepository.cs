using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace FoodApp
{
    public class OrdersRepository : IOrdersRepository 
    {
        private CosmosClient dbClient;
        private Container container;
        public OrdersRepository()
        {
            string cs = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
            string db = Environment.GetEnvironmentVariable("DBName");
            string orders = Environment.GetEnvironmentVariable("OrdersContainer");
            dbClient = new CosmosClient(cs);
            container = dbClient.GetContainer(db, orders);
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

        public async Task CreateOrderAsync(Order item)
        {
           await container.CreateItemAsync<Order>(item, new PartitionKey(item.Customer.Id));
        }

        public async Task UpdateOrderAsync(string id, Order item)
        {
            await container.UpsertItemAsync<Order>(item, new PartitionKey(item.Customer.Id));
        }
    }
}