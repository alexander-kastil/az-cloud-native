using System.Collections.Generic;
using Newtonsoft.Json;  
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace FoodApp.Orders
{
    public static class ProcessOrders
    {
        [FunctionName("ProcessOrders")]
        public static void Run([CosmosDBTrigger(
            databaseName: "orders-dev",
            collectionName: "orders",
            ConnectionStringSetting = "conCosmosDB",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input,
            ILogger log)
        {
             foreach (var document in input)
            {
                var order = JsonConvert.DeserializeObject<Order>(document.ToString());
                log.LogInformation("Changed food " + order.Id);
            }
        }
    }
}
