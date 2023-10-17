# Azure Durable Entities: Aggregation & Actors

A Durable Entity can be used:

-   As an actor like object to manage state and perform operations on the object, like a bank account.
-   To implement the Aggregator pattern to aggregate data from multiple sources like IOT devices

## Links & Resources

[Entity Functions on Microsoft Learn](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities?tabs=csharp)

## Demos

### Bank Account

-   Scaffold using the following the DurableFunctionsEntityOrchestration template:

    ![scaffold](_images/scaffold.png)

-   The Durable Entity is defined in the [BankAccount.cs](../04-durable-entity/bank-account/BankAccount.cs) file:

    ```csharp
    [FunctionName(nameof(BankAccount))]
    public static void BankAccount([EntityTrigger] IDurableEntityContext context)
    {
        switch (context.OperationName.ToLowerInvariant())
        {
            case "deposit":
                context.SetState(context.GetState<int>() + context.GetInput<int>());
                break;
            case "withdraw":
                var balance = context.GetState<int>() - context.GetInput<int>();
                context.SetState(balance);
                break;
            case "reset":
                context.SetState(0);
                break;
            case "get":
                context.Return(context.GetState<int>());
                break;
        }
    }
    ```

    >Note: For simplicity, the BankAccount Entity only stores an integer value for the balance. We cloud also implement the Entity using [a class](https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-entities?tabs=csharp#example-class-based-syntax---c) with properties. This is helpful when the Entity needs to store multiple values and get more complex.

-   [BankAccount.cs](../04-durable-entity/bank-account/BankAccount.cs) also defines an HttpTrigger to interact with the Entity.

    ```c#
    [FunctionName("DurableFunctionsEntityCSharp_CounterHttpStart")]
    public static async Task<HttpResponseMessage> Run(
    [HttpTrigger(AuthorizationLevel.Function, Route = "BankAccount/{entityKey}/amount")] HttpRequestMessage req,
    [DurableClient] IDurableEntityClient client,
    string entityKey, int amount)
    {
        var entityId = new EntityId(nameof(BankAccount), entityKey);

        if (req.Method == HttpMethod.Post)
        {
            await client.SignalEntityAsync(entityId, "deposit", amount);
            return req.CreateResponse(HttpStatusCode.Accepted);
        }
        else if (req.Method == HttpMethod.Delete)
        {
            await client.SignalEntityAsync(entityId, "withdraw", amount);
            return req.CreateResponse(HttpStatusCode.Accepted);
        }
        else if (req.Method == HttpMethod.Get)
        {
            EntityStateResponse<int> stateResponse = await client.ReadEntityStateAsync<int>(entityId);
            var resp = req.CreateResponse(HttpStatusCode.OK, stateResponse.EntityState);
            return resp;
        }
        return req.CreateResponse(HttpStatusCode.OK);
    }
   ```

- [test-bank-account.http](test-bank-account.http) contains a set of HTTP requests to test the BankAccount Entity.
