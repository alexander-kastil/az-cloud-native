using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace FoodApp
{
    public class PaymentRepository : IPaymentRepository
    {
        private Container container;
        public PaymentRepository(
                string connectionString,
                string databaseName,
                string containerName)
        {
            CosmosClient client = new CosmosClient(connectionString);
            container = client.GetContainer(databaseName, containerName);
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