using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace FoodApp
{
    public class HandlePaymentRequest
    {
        [FunctionName(nameof(HandlePaymentRequest))]
        [return: ServiceBus("payment-response", Connection = "ConnectionServiceBus")]
        public static async Task<OrderEvent> Run([ServiceBusTrigger("payment-requests", Connection = "ConnectionServiceBus")]string jsonPayment, 
        [DurableClient] IDurableEntityClient client, 
        ILogger logger)
        {
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {jsonPayment}");
            var resp = await DurableBankAccount.ExecutePayment(jsonPayment, client, logger)
                .ConfigureAwait(false);
            return resp;
        }
    }
}
