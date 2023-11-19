using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Client;

namespace FoodApp
{
    public class CookingRepository : ICookingRepository
    {

        AILogger logger;
        DaprClient client;

        public CookingRepository(DaprClient daprClient, AILogger aILogger)
        {
            logger = aILogger;
            client = daprClient;
        }

        public async Task AddOrderAsync(Order item)
        {
            logger.LogEvent("Order adding", item );
            await client.SaveStateAsync<Order>("food-state", item.Id, item);
        }

        public async Task UpdateOrderStatusAsync(string id, Order item)
        {
            var existing = await client.GetStateAsync<Order>("food-state", id);
            if (existing != null)
            {
                existing.Status = item.Status;
                await client.SaveStateAsync<Order>("food-state", id, item);
                logger.LogEvent($"Order {id} updated", item);
            }
            else
            {
                logger.LogEvent($"Order {id} not found", item);
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await client.GetStateAsync<IEnumerable<Order>>("food-state", "all");
        }

        public async Task<Order> GetOrderAsync(string id, string customerId)
        {
            return await client.GetStateAsync<Order>("food-state", id);
        }
    }
}