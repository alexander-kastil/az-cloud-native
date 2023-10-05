using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;

namespace FoodApp.DataGenerator
{
    public class Program
    {
        private const int numOfMessages = 3;

        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();

            var connectionString = configuration["sbConnectionString"];
            var topic = configuration["sbTopic"];
            var msg = configuration["sbMessage"];

            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender Sender = client.CreateSender(topic);

            using ServiceBusMessageBatch messageBatch = await Sender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(msg));

            try
            {
                await Sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A batch of {numOfMessages} messages has been published to the topic.");
            }
            finally
            {
                await Sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
