using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FoodApp
{
    public class OrdersRepository : IOrdersRepository 
    {
        private CosmosClient dbClient;
        private Container container;

        private ILogger logger;
        public OrdersRepository(ILogger _logger)
        {
            string cs = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
            string db = Environment.GetEnvironmentVariable("DBName");
            string orders = Environment.GetEnvironmentVariable("OrdersContainer");
            dbClient = new CosmosClient(cs);
            container = dbClient.GetContainer(db, orders);
            logger = _logger;
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

        public async Task CreateOrderAsync(Order order)
        {
            order.Status = OrderEventType.Created.ToString();
            logger.LogInformation($"Processing: Create order {order.Id}", order);
            await container.CreateItemAsync<Order>(order, new PartitionKey(order.Customer.Id));
        }

        public async Task UpdateOrderAsync(Order order, OrderEvent orderEvent)
        {
            order.Events.Add(orderEvent);
            order.Status = orderEvent.EventType;
            logger.LogInformation($"Processing: Updating order {order.Id}", order);
            await container.UpsertItemAsync<Order>(order, new PartitionKey(order.Customer.Id));
        }
    }
}