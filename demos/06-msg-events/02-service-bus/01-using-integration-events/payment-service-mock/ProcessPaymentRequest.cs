using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FoodApp
{
    public class ProcessPaymentRequest
    {
        [FunctionName("ProcessPaymentRequest")]
        public void Run([ServiceBusTrigger("payment-requests", Connection = "ServiceBusConnection")]string item, ILogger log)
        {
            IntegrationEvent<FoodOrder> order = JsonConvert.DeserializeObject<IntegrationEvent<FoodOrder>>(item);
            log.LogInformation($"Payment Service Mock received a payment request: {item}");
        }
    }
}
