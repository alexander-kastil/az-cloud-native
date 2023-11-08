using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FoodApp
{
    public class HandlePaymentRequest
    {
        [FunctionName("HandlePaymentRequest")]
        public void Run([ServiceBusTrigger("payment-requests", Connection = "ConnectionServiceBus")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }
    }
}
