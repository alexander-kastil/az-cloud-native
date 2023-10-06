using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace FoodApp.Orders
{
    public class OrdersCosmosUtil 
    {
        private CosmosClient dbClient;
        private Container container;
        public OrdersCosmosUtil()
        {
            string cs = Environment.GetEnvironmentVariable("conCosmosDB");
            string dbName = Environment.GetEnvironmentVariable("DBName");
            string containerName = Environment.GetEnvironmentVariable("ReadContainer");
            dbClient = new CosmosClient(cs);
            container = dbClient.GetContainer(dbName, containerName);
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

        public async Task AddOrderAsync(Order item)
        {
           await container.CreateItemAsync<Order>(item, new PartitionKey(item.CustomerId));
        }

        public async Task UpdateOrderAsync(string id, Order item)
        {
            await container.UpsertItemAsync<Order>(item, new PartitionKey(item.CustomerId));
        }
    }
}