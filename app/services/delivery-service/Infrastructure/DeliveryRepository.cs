using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace FoodApp
{
    public class DeliveryRepository : IDeliveryRepository
    {
        AILogger logger;
        DaprClient client;
        Container container;

        public DeliveryRepository(IConfiguration config,
                DaprClient daprClient, AILogger aILogger)
        {
            logger = aILogger;
            client = daprClient;
            AppConfig cfg = config.Get<AppConfig>();
            CosmosClient cosmosClient = new CosmosClient(cfg.CosmosDB.ConnectionString);
            container = cosmosClient.GetContainer(cfg.CosmosDB.DBName, cfg.CosmosDB.PaymentsContainer);
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
            await container.CreateItemAsync<Order>(item, new PartitionKey(item.Customer.Id));
        }

        public async Task DeleteOrderAsync(Order item)
        {
            await container.DeleteItemAsync<Order>(item.Id , new PartitionKey(item.Customer.Id));
        }

        public async Task UpdateOrderAsync(string id, Order item)
        {
            await container.UpsertItemAsync<Order>(item, new PartitionKey(item.Customer.Id));
        }
    }
}