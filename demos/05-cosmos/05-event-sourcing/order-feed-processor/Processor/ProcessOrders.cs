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
            databaseName: "orders-dev",
            collectionName: "order-events",
            ConnectionStringSetting = "conCosmosDB",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            ILogger log)
        {
            OrdersCosmosUtil util = new OrdersCosmosUtil();

            foreach (var document in input)
            {
                var evt = JsonConvert.DeserializeObject<OrderEvent>(document.ToString());
                if (evt != null)
                {
                    log.LogInformation("Received Order Event: " + evt.Id);
                    log.LogInformation("Event is of type: " + evt.EventType);

                    if (evt.EventType == OrderEventType.OrderCreated.ToString())
                    {
                        //Create the order
                        Order order = JsonConvert.DeserializeObject<Order>(evt.EventData.ToString());
                        await util.AddOrderAsync(order);
                    }
                    else
                    {
                        //Get the order and add the current event to history
                        Order existing = await util.GetOrderAsync(evt.OrderId, evt.CustomerId);
                        if (existing != null)
                        {
                            OrderEventMetadata metadata = new OrderEventMetadata
                            {
                                Id = evt.Id,
                                EventType = evt.EventType,
                                OrderId = evt.OrderId,
                                CustomerId = evt.CustomerId,
                                Timestamp = evt.Timestamp
                            };
                            existing.Events.Add(metadata);
                            await util.UpdateOrderAsync(evt.OrderId, existing);
                        }

                    }
                }
            }
        }
    }
}
