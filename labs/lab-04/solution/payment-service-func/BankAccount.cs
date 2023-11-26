using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
                    context.SetState(context.GetState<int>() + context.GetInput<int>());
                    break;
                case "withdraw":
                    var balance = context.GetState<int>() - context.GetInput<int>();
                    context.SetState(balance);
                    break;
            }
        }

        [FunctionName("UpdateBalance")]
        public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, Route = "bankAccount/updateBalance/{entityKey}/{amount}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey, string amount)
        {
            var entityId = new EntityId(nameof(DurableBankAccount), entityKey);

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
        public static async Task<int> GetBalance(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bankAccount/getBalance/{entityKey}")] HttpRequestMessage req,
            string entityKey,
            [DurableClient] IDurableEntityClient client,
            ILogger log)
        {
            var entityId = new EntityId(nameof(DurableBankAccount), entityKey);
            EntityStateResponse<int> stateResponse = await client.ReadEntityStateAsync<int>(entityId);
            return stateResponse.EntityState;
        }

        [FunctionName("ExecutePayment")]
        public static async Task<int> ExecutePayment(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "bankAccount/executePayment/{entityKey}/{amount}")] HttpRequestMessage req,
        string entityKey,
        string amount,
        [DurableClient] IDurableEntityClient client,
        ILogger log)
        {
            int intAmount = int.Parse(amount);
            var entityId = new EntityId(nameof(DurableBankAccount), entityKey);
            EntityStateResponse<int> stateResponse = await client.ReadEntityStateAsync<int>(entityId);

            if(stateResponse.EntityExists)
            {
                if(stateResponse.EntityState >= intAmount)
                {
                    await client.SignalEntityAsync(entityId, "withdraw", amount);
                }
                else
                {
                    log.LogInformation($"Insufficient funds. Current balance: {stateResponse.EntityState}");
                }
            }
            else
            {
                log.LogInformation($"Entity {entityKey} does not exist.");
            }
            return stateResponse.EntityState;
        }
    }
}