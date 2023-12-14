
## Course Overview

The seminar is aimed at Azure developers and software architects who want to get an overview of the core elements of developing and deploying cloud-native applications in Microsoft Azure. 

In addition to the theoretical parts of the individual modules, we modernize an app consisting of a classic monolith with UI into a cloud-native app with microservices (catalog, orders, payment, production delivery) and the related micro frontends. We discuss topics like the Cloud Maturity Model and place value on the use of best practices and cloud design patterns. In the introduction we also present the .NET Aspire framework, which we will partly use to implement the microservices. 

We teach container essentials and concepts such as optimization of Container sizes, security patching, stateful containers or sidecar patterns. To ensure a developer centric focus, we will deploy the microservices to the Kubernetes based Azure Container Apps und cover topics such as secrets, revisions, configuration management, health checks, Kubernetes event-driven auto-scaling - KEDA, as well as stateful containers and container jobs. For service to service authentication, we use managed identities and service connectors. The skills learned can also be applied to other container hosting platforms such as Azure Kubernetes Service (AKS) or Azure Red Hat OpenShift (ARO).

We use Azure Functions to implement stateful microservices, which we publish either as serverless or as containers to Azure Container Apps using .NET isolation. We cover topics such as OData, Open API support, dependency injection, and durable functions. We will use durable entities to implement and discuss virtual actors and durable saga pattern. We also cover topics such as monitoring and Dapr integration.

We will discuss the advantages of NoSQL databases and accompany you on your way from relational DB design to Cosmos DB NoSQL API and thereby introduce fundamentals of Domain Driven Design. We cover data modelling, performance optimization as well as access using SDK's and the Data Api Builder. We also cover topics such as change feed, event sourcing, materialized views and CQRS. 

We teach the basics of message & event-driven applications, their transaction patterns, which we implement using saga pattern. As alternative to .NET Aspire we will connect the individual services using Distributed Application Runtime (Dapr) and cover topics such as service invocation, pub/sub messaging, secrets, configuration, Dapr Actors, Dapr and Function integration as well as distributed tracing and observability.

We publish and secure the app and its microservices with API management and cover topics like versioning, revisions, authentication and BFF pattern using GraphQL. 

Last but not least, we will connect the micro frontends using Azure Event Grid and SignalR to create a real-time connected app.

Audience: Azure Developers & Software Architects

## Course Content

- Introduction to Cloud Native Applications & the Cloud Maturity Model
- Container Essentials & Configuration Management
- Developing & Publishing Microservices using Azure Container Apps (ACA)
- Stateful Microservices using Azure Functions
- NoSQL Data & Event storage using Cosmos DB
- Designing and Implementing Message based & Event Driven Apps
- Using Distributed Application Runtime - Dapr
- Optimizing and Securing Access using Api Management & Application Gateway
- Connecting Real Time Micro Frontends using Event Grid 

### Introduction to Cloud Native Applications & Cloud Maturity Model

- What are Cloud Native Applications
- Cloud Maturity Model: Monolith vs Microservices Architecture
- Clean Architecture
- Introduction to .NET Aspire: Orchestration, Components, Tooling
- Microservices Communication Patterns
- Architecture Overview of the Sample App & Services
- Cloud Architecture Design Patterns
- Container Hosting & DevOps
- Provisioning of Azure Resources using Azure CLI & Bicep
- Introduction to Azure Cost Management

### Container Essentials & Configuration Management

- Docker Development Workflow: Multistage Build, Run & Debug Recap
- Container builds using Azure Container Registry
- Using docker-compose.yaml to run multiple containers for local development
- Kubernetes Developer Essentials
- Container Configuration Management (Env Variables, Key Vault & Azure App Config Service)
- Understanding the Sidecar Pattern
- Optimizing .NET 8 Container Size using chiseled, trim and aot
- Container Security Patching

### Developing & Publishing Microservices using Azure Container Apps (ACA)

- Azure Container Apps Introduction
- Azure Container Apps vs Kubernetes
- Publish Microservices (Ingress, Egress) and manage Revisions
- Secrets, Managed Identities & Service Connectors
- Using Azure App Configuration in Azure Container Apps
- Task Automation using Jobs
- Scaling & KEDA (Kubernetes Event Driven Auto-Scaling) 
- Stateful Apps using Volume Mounts & Persistent Storage
- Microsoft Entra ID Easy Authentication 
- Health Probes, Monitoring, Logging & Observability

### Stateful Microservices using Azure Functions

- OData, Open API Support and Dependency Injection
- Hosting: Serverless vs Containers
- Environment Variables, Key Vault, and App Configuration
- Using Managed Identities and Service Connector to access Azure Resources
- Implementing and monitoring Durable Functions to implement long running processes
- Azure Durable Entities, Aggregation & Virtual Actors
- Publishing Azure Functions to Azure Container Apps

### NoSQL Data & Event storage using Cosmos DB

- From Relational to NoSQL: Do's and Don’ts
- Partitioning Strategies & Performance Optimization
- Domain Driven Design (DDD) Basics & Bounded Context Pattern
- Using SDKs to interact with Cosmos DB
- Using Data Api Builder to expose Cosmos DB
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

### Optimizing and Securing Access using Api Management & Application Gateway

- API Management (APIM) Recap
- API Versions and Revisions using Azure Container Apps 
- Authenticating to Backend Services
- Understanding Gateway Pattern and Backends for Frontend Pattern (BFF)
- Implement BFF using APIM and GraphQL

### Connecting Real Time Micro Frontends using Event Grid 

- Micro Frontends: Introduction & Benefits
- Publish the Shop Micro Frontend to Azure Container Apps
- Real-time connected Micro Frontend using Azure Event Grid and SignalR
- Connect the Real Time Kitchen Dashboard 
- Connect the Order Status Micro Frontend