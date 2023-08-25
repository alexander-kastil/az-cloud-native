# Building Blocks & Architecture Overview

- Class Sample Architecture Diagram & Introduction to the Azure Building Blocks used in this class
- Hosting: Containers, Kubernetes and Functions (Serverless / Containers)
- Configuration Management, Secrets: Key Vault, App Config Service
- Authentication & Authorization: Microsoft Identity & Managed Identities
- Data Storage: Azure Cosmos DB, Azure SQL, Blob Storage
- Messaging Brokers: Service Bus, Event Hub, Event Grid
- Access & Management: API Management & Application Gateway
- Real Time: Azure SignalR Service, Azure Web PubSub

## Food App - A food delivery application

[food-catalog-api](/app/food-catalog-api/) provides a REST API to manage a food catalog.

[food-shopp-ui](/app/food-shop-ui//) consumes it and provides an Online Food Shop implemented in Angular.

[graph-mail-demon-api](/app/graph-mail-demon-api/) provides a REST API to send emails via Microsoft Graph.

[food-invoices](/app/food-invoices/) provides a REST API to manage invoices.


![food-app](/_images/app.png)

## Links and Resources

[Azure Architecture Center](https://docs.microsoft.com/en-us/azure/architecture/browse/)

[Cloud Design Patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/)

## Tools and Extensions

[Visio Online](https://www.microsoft.com/de-de/microsoft-365/visio/flowchart-software)

[Lucid Chard](https://www.lucidchart.com/)

[Draw.io](https://www.diagrams.net/)
