using System;
using System.Text;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FoodApp
{
    public class GraphProcessor
    {
        private readonly ILogger<GraphProcessor> logger;

        public GraphProcessor(ILogger<GraphProcessor> lg)
        {
            logger = lg;
        }

        [Function(nameof(GraphProcessor))]
        public async Task Run([EventHubTrigger("graphevents-hub-dev", Connection = "EventHubSharedKey")] EventData[] events)
        {
            foreach (EventData eventData in events)
            {
                try
                {
                    string messageBody = Encoding.UTF8.GetString(eventData.Body.ToArray());
                    logger.LogInformation($"C# Event Hub trigger function processed a message: {messageBody}");
                    await Task.Yield();
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception thrown: {ex.Message}");
                }
            }
        }
    }
}
