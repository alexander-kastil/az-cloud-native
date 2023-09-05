using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core;
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
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "Counter/{entityKey}")] HttpRequestMessage req,
        [DurableClient] IDurableEntityClient client,
        string entityKey)
        {
            var content = req.Content;
            string requestBody = await content.ReadAsStringAsync();
            JObject data = JsonConvert.DeserializeObject<JObject>(requestBody);
            CounterParam param = data.ToObject<CounterParam>();

            var entityId = new EntityId(nameof(Counter), entityKey);
            if (req.Method == HttpMethod.Post)
            {
                await client.SignalEntityAsync(entityId, param.operation, param?.value);
                return req.CreateResponse(HttpStatusCode.OK);
            }

            EntityStateResponse<JToken> stateResponse = await client.ReadEntityStateAsync<JToken>(entityId);
            return req.CreateResponse(HttpStatusCode.OK, stateResponse.EntityState);
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
                    var state = context.GetState<int>();
                    context.Return(state);
                    break;
            }
        }
    }

    public class CounterParam
    {
        public string operation { get; set; }
        public int? value { get; set; }
    }
}