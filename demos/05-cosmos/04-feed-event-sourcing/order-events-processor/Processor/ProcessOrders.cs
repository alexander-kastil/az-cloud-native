using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FoodApp.Orders
{
    public static class ProcessOrders
    {
        [FunctionName("ProcessOrders")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "food-nosql-dev",
            collectionName: "order-events",
            ConnectionStringSetting = "CosmosDBConnectionString",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]
            IReadOnlyList<Document> input,
            ILogger log)
        {
            OrdersRepository repo = new OrdersRepository();

            foreach (var document in input)
            {
                var evt = JsonConvert.DeserializeObject<OrderEvent>(document.ToString());
                if (evt != null)
                {
                    log.LogInformation("Received Order Event: " + evt.Id);
                    log.LogInformation("Event is of type: " + evt.EventType);

                    if (evt.EventType == OrderEventType.Created.ToString())
                    {
                        //Create the order
                        Order order = JsonConvert.DeserializeObject<Order>(evt.Data.ToString());
                        await repo.CreateOrderAsync(order);
                    }
                    else
                    {
                        //Get the order and add the current event to history
                        Order existing = await repo.GetOrderAsync(evt.OrderId, evt.CustomerId);
                        if (existing != null)
                        {
                            existing.Events.Add(evt);
                            await repo.UpdateOrderAsync(evt.OrderId, existing);
                        }

                    }
                }
            }
        }
    }
}
