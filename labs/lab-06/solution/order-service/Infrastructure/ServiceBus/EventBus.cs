using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;

namespace FoodApp
{
    public class EventBus
    {
        static string connectionString = "";
        static string queue = "";

        public EventBus(string ConnectionString, string Queue)
        {
            connectionString = ConnectionString;
            queue = Queue;
        }
        public async void Publish(OrderEvent @event)        
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender Sender = client.CreateSender(queue);
            var json = JsonConvert.SerializeObject(@event);
            var message = new ServiceBusMessage(json);
            
            using ServiceBusMessageBatch messageBatch = await Sender.CreateMessageBatchAsync();
            if (!messageBatch.TryAddMessage(message))
            {
                throw new Exception($"The message is too large to fit in the batch.");
            }
            await Sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"Sent a single message to the queue: {queue}");
        }
    }
}