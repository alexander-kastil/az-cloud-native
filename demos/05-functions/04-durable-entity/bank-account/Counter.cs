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
        [HttpTrigger(AuthorizationLevel.Function, Route = "Counter/{entityKey}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
        {
            var entityId = new EntityId(nameof(Counter), entityKey);

            if (req.Method == HttpMethod.Post)
            {
                await client.SignalEntityAsync(entityId, "get", 1);
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

        [FunctionName(nameof(Counter))]
        public static void Counter([EntityTrigger] IDurableEntityContext context)
        {
            switch (context.OperationName.ToLowerInvariant())
            {
                case "add":
                    context.SetState(context.GetState<int>() + context.GetInput<int>());
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