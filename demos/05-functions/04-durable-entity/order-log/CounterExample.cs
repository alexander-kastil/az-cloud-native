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
                // "myCounter" is the ID here.
                // We want only one instance of the counter. So a fixed ID
                // In cases, where we need several counter, we can have different IDs.
                // The ID of the counter can also be passed as input to the orchestration.
                var entityId = new EntityId("Counter", "myCounter");

                // Perform the requested operation on the entity
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
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            var input = new CounterParameter { OperationName = "Increment" };
            string instanceId = await starter.StartNewAsync("FunctionOrchestrator", input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        /// <summary> HTTP Trigger Function to decrement the counter value by 1. </summary>
        [FunctionName("decrement")]
        public static async Task<HttpResponseMessage> HttpDecrementCounter(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            // Function input comes from the request content.
            var input = new CounterParameter { OperationName = "Decrement" };
            string instanceId = await starter.StartNewAsync("FunctionOrchestrator", input);

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
            return starter.CreateCheckStatusResponse(req, instanceId);
        }

        [FunctionName("getCounter")]
        public static async Task<HttpResponseMessage> HttpGetCounter(
            [HttpTrigger(AuthorizationLevel.Function)] HttpRequestMessage req,
            [DurableClient] IDurableEntityClient client)
        {
            // We want to return the value of counter defined by "Counter@myCounter"
            // The IDs could be different if we want to manage multiple counters.
            var entityId = new EntityId("Counter", "myCounter");

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
