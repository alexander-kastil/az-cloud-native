# Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture & Building Blocks
- Developer Environment Setup & Debugging 
- Use StateStore Component in Azure Container Apps
- Service Invocation & Bindings
- Pub/Sub Messaging
- Secrets and Configuration
- Introduction to Actors
- Implement a Saga using Dapr
- Observability and Distributed Tracing

This modules demonstrates how to code & debug a Dapr based microservices as well as to deploy it to Azure Container Apps. It is based on the [Dapr quickstarts](https://docs.dapr.io/getting-started/quickstarts/). 

It contains the following projects:

- [food-service-dapr](../00-app/food-service-dapr/) - A .NET Core Web API project that uses State Management to store and retrieve state. in a other demos it will be used to demonstrate features like Secrets, Publish & Subscribe as well as Observability and Distributed tracing. 
- [food-mvc-dapr](../00-app/food-mvc-dapr/) - A .NET MVC project that consumes the api using service invocation.
- [order-events-store-dapr](../00-app/order-events-store-dapr/) - The event store from module 5
- [food-invoices-dapr](../00-app/food-invoices-dapr/) - A .NET Core Web API project that uses Publish & Subscribe to receive food orders, store them in a database and send an invoice to the customer.

Configuration of of [Dapr components](https://docs.dapr.io/concepts/components-concept/) is stored in the `components` folder of the apps base directory. During development it will use `Redis` as the default state store. When deploying it will use Azure Blob Storage. We could also use Azure Cosmos DB as a state store just by changing the state store configuration.

- `statestore.yaml` - Configures the state store to use Azure Blob Storage.

    ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
    name: foodstore
    spec:
    type: state.redis
    version: v1
    metadata:
    - name: redisHost
        value: localhost:6379
    - name: redisPassword
        value: ""
    ```

    ![dapr-state](_images/dapr-state.png)

## Tools and Extensions

[Dapr - Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-dapr)

[Tye - Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-tye)