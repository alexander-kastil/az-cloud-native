# Lab 06 - Designing and Implementing Message based & Event Driven Apps

In this lab we will take a look at the message flow between the services and design the message data structures. Most of the practical work will be done in the next lab as we will use Dapr to implement the message flow.

## Task: Examine the Domain Message Flow Model and Design the Message Data Structure

- Examine the `Food App Domain Message Flow Model`. 

    ![message-flow-model](_images/message-flow.png)

- All messages / events will be wrapped in an `OrderEvent`. The OrderEvent will be used to route the messages to the correct service. When using `Dapr Pub/sub`, which will be introduced in the next module, the `OrderEvent` will be wrapped in a `CloudEvent` by Dapr.

    ![order-event](_images/order-event.png)
    
    >Note: Although some messages can container fields like `OrderId` or `CustomerId` and might not be a required field in some services or represent duplicate fields, we include them in in the `OrderEvent` envelope to avoid costly lookups for the processing of response messages in other services which might require them.

- Design the data structures for the messages that will be exchanged between the services for the following commands and events:

    - PaymentRequest    
    - PaymentResponse: PaymentSuccess | PaymentFailed

- Possible Message Data Structure

    ![message-flow-data-model](_images/message-flow-data-model.png)

## Task: Review the payment process

- Examine the payment process. We will use this process as a reference for the implementation of the message flow. Note that as a result of the payment process we might send an order confirmation / cancellation to the customer and might use a topic for this. We will not implement this in this lab but instead focus on the message flow between order service and payment service.

- To keep things simple we will integrate the bank account into the payment service. In a real world scenario we would consume a separate external service for this.

    ![payment-process](_images/payment-process.png)

## Task: Provision the required infrastructure to connect Order Service to the Payment Service

- Create a container `cooking-service` with the partition key `/orderId` in the `food-nosql-$env` database in the `az-native-cosmos-nosql-$env` Cosmos DB account using IaC (Azure CLI or Bicep). We will store all incoming requests as the entities of the bounded context in this container. In a real world scenario you would create a database for each service and might have an advanced physical design with multiple containers for each service.

- Create a `payment-requests` and a `payment-response` queue in the `aznativesb$env` Service Bus namespace using IaC (Azure CLI or Bicep).

## Task: Implement the payment process

- Take the [Order Service CQRS](./starter/orders-service-cqrs/) from the previous lab and connect it to the `Payment Service` using Azure Service Bus and a queue. 
    
    - Implement the class for `Domain/PaymentRequest.cs` messages.

    ```c#
    public class PaymentRequest{
        [JsonProperty("orderId")]
        public string OrderId {get;set;}
        [JsonProperty("amount")]
        public decimal Amount {get;set;}
        [JsonProperty("paymentInfo")]
        public PaymentInfo PaymentInfo {get;set;}
    }
    ```

    - Implement `Infrastructure/ServiceBus/EventBus.cs`

    ```c#
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
    ```
    - Add the `ServiceBusConfig` to reflect `appsettings.json` file.

    ```json
    "ServiceBus": {
        "ConnectionString": "<connection-string>",
        "QueueName": "payment-requests"
    },
    ``` 

    ```c#
    public class ServiceBusConfig
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
    ```

    - Use the `OrdersController` to publish the event. 

    ```c#
    var paymentRequest = new PaymentRequest
    {
        OrderId = order.Id,
        Amount = order.Total,
        PaymentInfo = order.Payment
    };
    
    // Wrap it into our Integration Event
    eb.Publish(new OrderEvent
    {
        OrderId = order.Id,
        CustomerId = order.Customer.Id,
        EventType = "PaymentRequested",
        Data = JsonConvert.SerializeObject(paymentRequest)
    });
    ```

- Take the [Payment Service](./starter/payment-service/) from module 04 as a starting point and implement the `Payment Service`. Do not implement the response messages yet. In the next lab we will use Dapr to implement the message flow and the response messages.

    - Add `BankAccount/HandlePaymentRequest.cs` and call `DurableBankAccount.ExecutePayment`

    ```c#
    public class HandlePaymentRequest
    {
        [FunctionName(nameof(HandlePaymentRequest))]
        [return: ServiceBus("payment-response", Connection = "ConnectionServiceBus")]
        public static async Task<OrderEvent> Run([ServiceBusTrigger("payment-requests", Connection = "ConnectionServiceBus")]string jsonPayment, 
        [DurableClient] IDurableEntityClient client, 
        ILogger logger)
        {
            logger.LogInformation($"C# ServiceBus queue trigger function processed message: {jsonPayment}");
            var resp = await DurableBankAccount.ExecutePayment(jsonPayment, client, logger)
                .ConfigureAwait(false);
            return resp;
        }
    }   
    ```

- Use [Visual Studio Code REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client) to test the `Order Service` and the `Payment Service`.

## Task: Update your deployment

- Redeploy order service to Azure Container Apps
- Deploy Payment Service to a Fuctions App