using Dapr.Client;
using Newtonsoft.Json;

namespace FoodApp
{
    public class DaprEventBus : IDaprEventBus
    {
        private string DAPR_PUBSUB_NAME = "";
        private DaprClient daprClient;

        public DaprEventBus(DaprClient daprClient, IConfiguration config)
        {
            this.daprClient = daprClient;
            this.DAPR_PUBSUB_NAME = config.GetValue<string>("PUBSUB_NAME");
        }        

        public async void Publish(OrderEvent @event)        
        {
            string topicName = @event.EventType;
            await daprClient.PublishEventAsync(DAPR_PUBSUB_NAME, topicName, @event);         
        }
    }
}