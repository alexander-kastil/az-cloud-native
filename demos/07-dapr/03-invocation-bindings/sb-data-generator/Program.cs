using Microsoft.Extensions.Configuration;
using Azure.Messaging.ServiceBus;
using Azure.Messaging;

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

            var connectionString = configuration["ConnectionString"];
            var topic = configuration["Queue"];
            
            PaymentRequest paymentRequest = new PaymentRequest{
                OrderId = "123",
                CustomerId = "456",
                CustomerName = "Alexander Pajer",
                PaymentMethod = "Apple Pay",
                PaymentAccount = "alexander.pajer@integrations.at",
                PaymentAmount = "123.45",
                PaymentStatus = "Requested",
                PaymentDate = DateTime.Now.ToString()
            };

            var cloudEvent = new CloudEvent(
                "FoodApp",
                "PaymentRequest",
                paymentRequest
            );

            ServiceBusClient client = new ServiceBusClient(connectionString);
            ServiceBusSender Sender = client.CreateSender(topic);

            using ServiceBusMessageBatch messageBatch = await Sender.CreateMessageBatchAsync();
            messageBatch.TryAddMessage(new ServiceBusMessage(new BinaryData(cloudEvent)));

            try
            {
                await Sender.SendMessagesAsync(messageBatch);
                Console.WriteLine($"A messages has been published to the queue.");
            }
            finally
            {
                await Sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }
    }
}
