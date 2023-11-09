using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace FoodApp
{
    public static class DurableBankAccount
    {
        [FunctionName(nameof(DurableBankAccount.BankAccount))]
        public static void BankAccount([EntityTrigger] IDurableEntityContext context)
        {
            switch (context.OperationName.ToLowerInvariant())
            {
                case "deposit":
                    context.SetState(context.GetState<decimal>() + context.GetInput<decimal>());
                    break;
                case "withdraw":
                    var balance = context.GetState<decimal>() - context.GetInput<decimal>();
                    context.SetState(balance);
                    break;
            }
        }

        [FunctionName("UpdateBalance")]
        public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, Route = "bankAccount/updateBalance/{entityKey}/{amount}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey, string amount)
        {
            var entityId = new EntityId(nameof(BankAccount), entityKey);

            if (req.Method == HttpMethod.Post)
            {
                await client.SignalEntityAsync(entityId, "deposit", amount);
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            else if (req.Method == HttpMethod.Delete)
            {
                await client.SignalEntityAsync(entityId, "withdraw", amount);
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [FunctionName("GetBalance")]
        public static async Task<decimal> GetBalance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bankAccount/getBalance/{entityKey}")] HttpRequestMessage req,
            string entityKey,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var entityId = new EntityId(nameof(BankAccount), entityKey);
            EntityStateResponse<decimal> stateResponse = await client.ReadEntityStateAsync<decimal>(entityId);
            return stateResponse.EntityState;
        }

        [FunctionName("ProcessPayment")]
        public static async Task<IActionResult> ProcessPayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bankAccount/processPayment")] HttpRequest req,
        [DurableClient] IDurableEntityClient client,
        ILogger logger)
        {
            string jsonPayment = await new StreamReader(req.Body).ReadToEndAsync();
            OrderEvent paymentResponse = await ExecutePayment(jsonPayment, client, logger);            
            return new ObjectResult(paymentResponse);
        }

        public static async Task<OrderEvent> ExecutePayment(string json, IDurableEntityClient client, ILogger logger){
            OrderEvent orderEvent = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderEvent>(json);
            PaymentRequest paymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentRequest>(orderEvent.Data.ToString());

            //Save the incoming payment request to the database - Omitted for brevity

            var entityId = new EntityId(nameof(BankAccount), paymentRequest.PaymentInfo.AccountNumber);
            EntityStateResponse<decimal> stateResponse = await client.ReadEntityStateAsync<decimal>(entityId);

            PaymentResponse paymentResponse = new PaymentResponse(){
                OrderId = paymentRequest.OrderId,
            };

            if(stateResponse.EntityExists)
            {
                if(stateResponse.EntityState >= paymentRequest.Amount)
                {
                    await client.SignalEntityAsync(entityId, "withdraw", paymentRequest.Amount);
                    paymentResponse.Status = "Success";
                    paymentResponse.Data = paymentRequest;
                }
                else
                {
                    var msg = $"Insufficient funds. Current balance: {stateResponse.EntityState}";
                    paymentResponse.Status = "Failed";
                    paymentResponse.Data = msg;
                    logger.LogInformation(msg);
                }
            }
            else
            {
                var msg = $"Bank Account {entityId} does not exist.";
                paymentResponse.Status = "Failed";
                paymentResponse.Data = msg;
                logger.LogInformation(msg);
            }

            //Save the payment response to the database - Omitted for brevity
            OrderEvent result = new OrderEvent(){
                OrderId = paymentRequest.OrderId,
                EventType = "PaymentResponse",
                Data = paymentResponse
            };

            return result;
        }
    }    
}