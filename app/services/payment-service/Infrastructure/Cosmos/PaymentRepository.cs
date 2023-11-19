using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using IBankActorInterface;
using Microsoft.Azure.Cosmos;

namespace FoodApp
{
    public class PaymentRepository : IPaymentRepository
    {
        private Container container;
        private DaprClient dapr;

        public PaymentRepository(
            string connectionString,
            string databaseName,
            string containerName,
            DaprClient daprClient)
        {
            CosmosClient client = new CosmosClient(connectionString);
            container = client.GetContainer(databaseName, containerName);
            dapr = daprClient;
        }

        public async Task ExecutePayment(OrderEvent evt)
        {
            Console.WriteLine($"Received payment request for order: {evt.OrderId}", evt);
            PaymentRequest paymentRequest = Newtonsoft.Json.JsonConvert.DeserializeObject<PaymentRequest>(evt.Data.ToString());
            if (paymentRequest != null)
            {
                // When using transactional Outbox pattern, we need to add the payment to the database
                // We then could use Cosmos Change feed to execute the payment against our dapr bank service
                // PaymentTransaction payment = new PaymentTransaction()
                // {
                //     Id = Guid.NewGuid().ToString(),
                //     CustomerId = evt.CustomerId,
                //     OrderId = paymentRequest.OrderId,
                //     PaymentInfo = paymentRequest.PaymentInfo,
                //     Amount = paymentRequest.Amount,
                //     Status = "Pending"
                // };            
                // await this.payment.AddPaymentAsync(payment);

                // To keep things simple we will just execute the payment against our dapr bank service
                // Make sure to created the bank account with the same account number in advance
                // You will find a sample in the bank client of the starter
                var usersBank = ActorProxy.Create<IBankActor>(new ActorId(paymentRequest.PaymentInfo.AccountNumber), "BankActor");
                var withdrawResp = await usersBank.Withdraw(new WithdrawRequest() { Amount = paymentRequest.Amount });
                // Now we could issue a payment response just like we did in 
                PaymentResponse paymentResponse = new PaymentResponse()
                {
                    OrderId = paymentRequest.OrderId,
                    Status = withdrawResp.Status,
                    Data = withdrawResp.Message
                };

                await dapr.PublishEventAsync("food-pubsub", "payment-response-topic", paymentResponse);
            }
        }

        public async Task AddPaymentAsync(PaymentTransaction item)
        {
            await container.CreateItemAsync<PaymentTransaction>(item, new PartitionKey(item.Id));
        }

        public async Task<IEnumerable<PaymentTransaction>> GetAllPaymentsAsync()
        {
            var sql = "SELECT * FROM payments p where p.type='payment'";
            QueryDefinition qry = new QueryDefinition(sql);
            FeedIterator<PaymentTransaction> feed = container.GetItemQueryIterator<PaymentTransaction>(qry);

            List<PaymentTransaction> payments = new List<PaymentTransaction>();
            while (feed.HasMoreResults)
            {
                FeedResponse<PaymentTransaction> response = await feed.ReadNextAsync();
                foreach (PaymentTransaction payment in response)
                {
                    payments.Add(payment);
                }
            }
            return payments;
        }

        public async Task<PaymentTransaction> GetPaymentByIdAsync(string id, string customerId)
        {
            try
            {
                ItemResponse<PaymentTransaction> response = await container.ReadItemAsync<PaymentTransaction>(id, new PartitionKey(customerId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task UpdatePaymentAsync(string id, PaymentTransaction item)
        {
            await container.UpsertItemAsync<PaymentTransaction>(item, new PartitionKey(item.Id));
        }
    }
}