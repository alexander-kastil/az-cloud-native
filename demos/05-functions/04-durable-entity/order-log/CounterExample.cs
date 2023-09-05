using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DurableCounter.Model;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace DurableCounter
{
    public static class CounterExample
    {
        [FunctionName("FunctionOrchestrator")]
        public static async Task<int> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            int currentValue = -1;
            var input = context.GetInput<CounterParameter>();

            if (input != null && !string.IsNullOrWhiteSpace(input.OperationName))
            {
                var entityId = new EntityId("Counter", input.EntityKey);
                currentValue = await context.CallEntityAsync<int>(entityId, input.OperationName);
            }
            return currentValue;
        }


        [FunctionName("Counter")]
        public static int Counter([EntityTrigger] IDurableEntityContext ctx, ILogger log)
        {
            log.LogInformation($"Request for operation {ctx.OperationName} on entity.");

            switch (ctx.OperationName.Trim().ToLowerInvariant())
            {
                case "increment":
                    ctx.SetState(ctx.GetState<int>() + 1);
                    break;
                case "decrement":
                    ctx.SetState(ctx.GetState<int>() - 1);
                    break;
                case "get":
                    // default value of integer, 0, is returned if counter is unset
                    ctx.Return(ctx.GetState<int>());
                    break;
            }

            // Return the latest value
            return ctx.GetState<int>();
        }

        /// <summary> HTTP Trigger Function to increment the counter value by 1. </summary>
        [FunctionName("increment")]
        public static async Task<HttpResponseMessage> HttpIncrementCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "counter/increment/{entityKey}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log,
            string entityKey)
        {
            // Function input comes from the request content.
            var input = new CounterParameter { OperationName = "Increment", EntityKey = entityKey };
            string instanceId = await starter.StartNewAsync("FunctionOrchestrator", input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        /// <summary> HTTP Trigger Function to decrement the counter value by 1. </summary>
        [FunctionName("decrement")]
        public static async Task<HttpResponseMessage> HttpDecrementCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "counter/decrement/{entityKey}")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log,
            string entityKey)
        {
            // Function input comes from the request content.
            var input = new CounterParameter { OperationName = "Decrement", EntityKey = entityKey };
            string instanceId = await starter.StartNewAsync("FunctionOrchestrator", input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("getCounter")]
        public static async Task<HttpResponseMessage> HttpGetCounter(
            [HttpTrigger(AuthorizationLevel.Function, Route = "counter/get/{entityKey}")] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client,
            string entityKey)
        {

            var entityId = new EntityId("Counter", entityKey);
            try
            {
                // An error will be thrown if the counter is not initialised.
                var stateResponse = await client.ReadEntityStateAsync<int>(entityId);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(stateResponse.EntityState.ToString())
                };
            }
            catch (System.NullReferenceException)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent("Counter is not yet initialised. " +
                    "Initialise it by calling increment or decrement HTTP Function.")
                };
            }
        }
    }
}
