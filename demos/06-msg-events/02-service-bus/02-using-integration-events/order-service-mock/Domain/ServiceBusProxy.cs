using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace FoodApp.Common
{
    public class ServiceBusProxy
    {
        static string connectionString = "";
        static string queue = "";

        public ServiceBusProxy(string ConnectionString, string Queue)
        {
            connectionString = ConnectionString;
            queue = Queue;
        }
        public async void AddEvent(IntegrationEvent evt)        
        {
            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender Sender = client.CreateSender(queue);

            using ServiceBusMessageBatch messageBatch = await Sender.CreateMessageBatchAsync();
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(JsonSerializer.Serialize(evt))))
            {
                throw new Exception($"The message is too large to fit in the batch.");
            }
            await Sender.SendMessagesAsync(messageBatch);
            Console.WriteLine($"Sent a single message to the queue: {queue}");
        }
    }
}