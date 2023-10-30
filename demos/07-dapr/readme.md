# Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture & Building Blocks
- Developer Environment Setup & Debugging 
- Use StateStore Component in Azure Container Apps
- Service Invocation & Bindings
- Pub/Sub Messaging
- Secrets and Configuration
- Introduction to Dapr Actors
- Observability and Distributed Tracing

This modules demonstrates how to code & debug a Dapr based microservices as well as to deploy it to Azure Container Apps. It is based on the [Dapr quickstarts](https://docs.dapr.io/getting-started/quickstarts/). 

It contains the following projects:

- [food-service-dapr](../00-app/food-service-dapr/) - A .NET Core Web API project that uses State Management to store and retrieve state. in a other demos it will be used to demonstrate features like Secrets, Publish & Subscribe as well as Observability and Distributed tracing. 
- [food-mvc-dapr](../00-app/food-mvc-dapr/) - A .NET MVC project that consumes the api using service invocation.
- [order-events-store-dapr](../00-app/order-events-store-dapr/) - The event store from module 5
- [food-invoices-dapr](../00-app/food-invoices-dapr/) - A .NET Core Web API project that uses Publish & Subscribe to receive food orders, store them in a database and send an invoice to the customer.

Configuration of of [Dapr components](https://docs.dapr.io/concepts/components-concept/) can be stored in the `components` folder of the apps base directory.

## Links & Resources

[Dapr on YouTube](https://www.youtube.com/channel/UCtpSQ9BLB_3EXdWAUQYwnRA)

[eShop on Dapr Reference Solution](https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/reference-application)
