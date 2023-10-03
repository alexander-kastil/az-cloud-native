using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace FoodApp.Orders
{
    public class OrdersRepository : IOrdersRepository
    {
        private CosmosClient dbClient;
        private Container container;
        public OrdersRepository()
        {
             string cs = Environment.GetEnvironmentVariable("conCosmosDB");
            string dbName = Environment.GetEnvironmentVariable("DBName");
            string containerName = Environment.GetEnvironmentVariable("ReadContainer");

            dbClient = new CosmosClient(cs);
            container = dbClient.GetContainer(dbName, containerName);
        }
        
        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            var sql = "SELECT * FROM orders o where o.type='order'";
            QueryDefinition qry = new QueryDefinition(sql);
            FeedIterator<Order> feed = container.GetItemQueryIterator<Order>(qry);

            List<Order> orders = new List<Order>();
            while (feed.HasMoreResults)
            {
                FeedResponse<Order> response = await feed.ReadNextAsync();
                foreach (Order od in response)
                {
                    orders.Add(od);
                    Console.WriteLine("\tRead {0}\n", od.CustomerId);
                }
            }
            return orders;
        }

        public async Task<IEnumerable<Order>> GetOrdersByQueryAsync(string queryString)
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

        public async Task AddOrderAsync(Order item)
        {
           await container.CreateItemAsync<Order>(item, new PartitionKey(item.CustomerId));
        }

        public async Task DeleteOrderAsync(Order item)
        {
            await container.DeleteItemAsync<Order>(item.Id , new PartitionKey(item.CustomerId));
        }

        public async Task UpdateOrderAsync(string id, Order item)
        {
            await container.UpsertItemAsync<Order>(item, new PartitionKey(item.CustomerId));
        }
    }
}