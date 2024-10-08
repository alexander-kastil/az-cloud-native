# Designing & Implementing Cloud Native Applications using Microsoft Azure

The seminar is aimed at Azure developers and software architects who want an overview of the core elements and tools for developing and deploying cloud-native applications in Microsoft Azure. It also prepares for the following Applied Skills:

- Deploy cloud-native apps using Azure Container Apps (AZ-2003)
- Build distributed apps with .NET Aspire ()

Alongside the theoretical parts of the individual modules, we will modernize an app from a classic monolith to a cloud-native app with microservices and the associated micro-frontends. In doing so, we will discuss the Cloud Maturity Model and emphasize the use of best practices and cloud design patterns. We will also introduce you to the .NET Aspire Framework, which we will partially use for the implementation of the microservices.

We will cover container essentials and concepts such as configuration management, stateful containers, and the sidecar pattern. To ensure a developer-centric focus, we will distribute on Kubernetes-based Azure Container Apps and cover topics such as secrets, revisions, config injection, health checks, Kubernetes Event Driven Auto-Scaling (KEDA), stateful containers, and jobs. For the authentication of service-to-service communication, we use managed identities and service connectors. The knowledge gained here can also be applied to Azure Kubernetes Service (AKS) or Azure Red Hat OpenShift.

We use Azure Functions to implement microservices, which can be hosted either serverless or as containers. Specifically, we will cover topics such as durable functions and entities, their use cases, and monitoring.

We will discuss the advantages of NoSQL databases and guide you from relational DB design to Cosmos DB NoSQL data and event stores, considering Domain Driven Design (DDD). We will cover topics such as data modeling, partitioning and performance optimization, CRUD with SDKs and Data API Builder, change feed, materialized views, event sourcing, and CQRS.

We will teach the basics of event-driven applications, message flow design, orchestration, and saga. In the chapter on Distributed Application Runtime (Dapr), we will cover topics such as service invocation, state management, secrets, configuration, bindings, pub/sub, Dapr actors, observability, and distributed tracing, alongside developer environment setup and debugging.

We will publish, secure, and optimize our cloud-native app and its microservices with API Management and Application Gateway, and discuss topics such as revision and versions, authentication, and implementing a backends for frontend pattern (BFF) using GraphQL.

Last but not least, we will connect our micro frontends using Azure Event Grid to implement real-time connected order status and production dashboards.

The demos, lab starters, and solutions will mostly be provided in .NET and Angular. Occasionally, alternative technology stacks may be used or referenced in their documentation.

Prerequisites and Target Audience
Course participants who want to successfully complete the labs should have practical experience with the knowledge imparted in the AZ-204 seminar. DevSecOps relevant topics are covered in a separate course.

Audience: Azure Developers & Software Architects

Topics

- Introduction to Cloud Native Applications & the Cloud Maturity Model
- Deploy cloud-native apps using Azure Container Apps (ACA)
- Build distributed apps with .NET Aspire
- Stateful Microservices using Azure Functions
- NoSQL Data & Event storage using Cosmos DB
- Designing and Implementing Message based & Event Driven Apps
- Using Distributed Application Runtime - Dapr
- Optimizing and Securing Access using Api Management & Application Gateway
- Connecting Real Time Micro Frontends using Event Grid 

### Introduction to Cloud Native Applications & Cloud Maturity Model

- What are Cloud Native Applications
- Cloud Maturity Model: Monolith vs Microservices Architecture
- Introduction to .NET Aspire: Orchestration, Components, Tooling
- Microservices Communication Patterns
- Architecture Overview of the Sample App & Services
- Cloud Architecture Design Patterns
- Container Hosting & DevOps
- Provisioning of Azure Resources using Azure Developer CLI (AZD) & Bicep
- Introduction to Azure Cost Management

### Deploy cloud-native apps using Azure Container Apps (ACA)

- Get started with cloud native apps and containerized deployments
- Configure Azure Container Registry for container app deployments
- Configure a container app in Azure Container Apps
- Configure continuous deployment for container apps
- Secrets, Managed Identities & Service Connectors
- Using Azure App Configuration in Azure Container Apps
- Task Automation using Jobs
- Scale and manage deployed container apps using KEDA (Kubernetes Event Driven Auto-Scaling) 
- Stateful Apps using Volume Mounts & Persistent Storage
- Health Probes, Monitoring, Logging & Observability

### Build distributed apps with .NET Aspire

- Introduction to .NET Aspire & Creating Projects
- Use telemetry & service discovery in a .NET Aspire project
- Use databases in .NET Aspire (SQL Server, Cosmos DB, MongoDB, ...)
- Improve performance with a cache in a .NET Aspire project
- Send messages with RabbitMQ / Service Bus in a .NET Aspire project
- Deploy a .NET Aspire solution to Azure Container Apps / Kubernetes

### Stateful Microservices using Azure Functions

- OData, Open API Support and Dependency Injection
- Hosting: Serverless vs Containers
- Environment Variables, Key Vault, and App Configuration
- Using Managed Identities and Service Connector to access Azure Resources
- Durable Functions - Orchestrate Long Running Processes
- Azure Durable Entities, Aggregation & Virtual Actors
- Publishing Azure Functions to Azure Container Apps
- Distributed Tracing in Azure Functions

### NoSQL Data & Event storage using Cosmos DB

- From Relational to NoSQL: Do's and Don’ts
- Partitioning Strategies & Performance Optimization
- Domain Driven Design (DDD) Basics & Bounded Context Pattern
- Using SDKs and Data Api Builder to expose Cosmos DB
- 
- Implementing an Event Store using Event Sourcing
- Creating Materialized Views using Materialized Views Builder
- Optimizing Read/Write Performance with Change Feed & CQRS 

### Designing and Implementing Message- & Event Driven Apps

- Introduction to Messaging
- Message Types and Channels
- Introduction to Event Driven Architecture (EDA)
- Event Types: Domain-, Integration-, Cloud Events
- Publishing & Subscribing Events using an Event Bus
- Distributed Transactions
- Saga: Orchestration, Choreography
- Common Message Brokers in Azure

### Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture & Building Blocks
- Developer Environment Setup, Debugging & State Management
- Using Dapr Components in Azure Container Apps
- Service Invocation & Bindings
- Pub/Sub Messaging
- Secrets and Configuration
- Azure Functions & Dapr Bindings
- Dapr Actors & Saga
- Observability and Distributed Tracing
- Dapr & .NET Aspire Integration

### Optimizing and Securing Access using Api Management & Application Gateway

- API Management (APIM) Recap
- API Versions and Revisions using Azure Container Apps 
- Authenticating to Backend Services
- Understanding Gateway Pattern and Backends for Frontend Pattern (BFF)
- Implement BFF using APIM and GraphQL

### Connecting Real Time Micro Frontends using Event Grid 

- Micro Frontends: Introduction & Benefits
- Publish the Shop Micro Frontend to Azure Container Apps
- Event Grid Basics
- Partner Events
- Real-time connected Micro Frontend using Azure Event Grid and SignalR
- Connect the Real Time Kitchen Dashboard 
- Connect the Order Status Micro Frontend
