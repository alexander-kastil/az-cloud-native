# Building Blocks & Architecture Overview

- Class Sample Architecture Diagram & Introduction to the Azure Building Blocks used in this class
- Deploying App Ressources to Azure using Bicep
- Hosting: Azure Container Apps and Functions (Serverless / Containers)
- Configuration Management, Secrets: Key Vault, App Config Service
- Authentication & Authorization: Microsoft Identity & Managed Identities
- Data Storage: Azure Cosmos DB, Azure SQL, Blob Storage
- Messaging Brokers: Service Bus, Event Hub, Event Grid
- Access & Management: API Management & Application Gateway
- Real Time: Azure SignalR Service, Azure Web PubSub

## Links & Resources

[Azure Architecture Center](https://docs.microsoft.com/en-us/azure/architecture/browse/)

[Cloud Design Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)

## Tools and Extensions

[Visio Online](https://www.microsoft.com/de-de/microsoft-365/visio/flowchart-software)

[Lucid Chard](https://www.lucidchart.com/)

[Draw.io](https://www.diagrams.net/)

## Food App - Architecture Overview

Food App is a food delivery application that is used to demonstrate how to combine the different Azure building blocks, in order to build a modern cloud native application. It consists of several Micro Frontends and uses a Microservice Architecture which will be hosted on Azure Container Apps to allow focusing on the development of the application and not on the infrastructure. It could also be deployed to Azure Kubernetes Service (AKS).

Dapr will be used to connect the different microservices and to implement the Saga Pattern to handle distributed transactions. It will also provide Service Discovery, Observability and Distributed Tracing.


![food-app](_images/app.png)

### Food Shop Frontend

A Food Shop implemented in Angular. It requests the menu from Food Catalog API (1) and then uses the Food Order API to place orders (2).

### Catalog Service

An API that returns a list of food items from a relational SQL Server database (1). It could also be implemented using a NoSQL database like Cosmos DB, but for the sake of simplicity we are using a relational database.

### Orders Service

An API that uses Cosmos DB as an event store (2). The order events will be processed using the change feed and will be stored in another container where they are aggregated and optimized for reads in order to demonstrate Event Sourcing & CQRS (Command and Query Responsibility Segregation). The initial order event will be submitted to Service Bus to start the ordering process (3). 

Some of the messages from Service Bus will be picked up by Event Grid and will be available to the Micro Frontend as real time data provided by SignalR. From this moment on the real-time state of the order is visible to the customer (4).

Whenever a change of the order state is published by a microservice, it will be picked up by the Orders service, which uses it to keep track of the current order state. 

After delivery, the order will be marked as completed (7)

Later, we will upgrade Order Service to implement a Saga Pattern using Dapr Actors to handle distributed transactions.

### Payment Service

The Payment Api picks up orders from a Service Bus and processes them. When the payment is processed it sends a message to a Service Bus topic to notify the Food Order Api that the payment was processed (4).

### Kitchen Dashboard & Production Service

When payment has been processed the order will be displayed by the Kitchen Dashboard (5) which is implemented as an Angular standalone app. It is implemented as Progressive Web App (PWA) and can be installed on industry tablets. 

The state of the cooking progress is saved in the production service (5) which is using Redis. When preparation is done, a message indication this state is published to Service Bus.

### Delivery Service

The Delivery service picks up prepared food orders from a Service Bus and delivers them (6). When the delivery is done it sends a message to a Service Bus to notify that the delivery was made.

### Graph Mail Daemon

A daemon that sends notification e-mails to confirm orders after they are placed and paid (5) and sends the final invoice after the delivery is done (7). 

It uses the Microsoft Graph API to send e-mails. In real life one could also use Twilio SendGrid or other e-mail service.