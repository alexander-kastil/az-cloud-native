using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Integrations
{
    public static class DurableFunctionsEntityHttpCSharp
    {
        [FunctionName("DurableFunctionsEntityCSharp_CounterHttpStart")]
        public static async Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Function, Route = "BankAccount/{entityKey}/amount")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey, int amount)
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
            else if (req.Method == HttpMethod.Get)
            {
                EntityStateResponse<int> stateResponse = await client.ReadEntityStateAsync<int>(entityId);
                var resp = req.CreateResponse(HttpStatusCode.OK, stateResponse.EntityState);
                return resp;
            }
            return req.CreateResponse(HttpStatusCode.OK);
        }

        [FunctionName(nameof(BankAccount))]
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
                case "reset":
                    context.SetState(0);
                    break;
                case "get":
                    context.Return(context.GetState<int>());
                    break;
            }
        }
    }
}