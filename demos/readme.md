# Designing & Implementing Cloud Native Applications using Microsoft Azure

Das Seminar richtet sich an Azure Entwickler und Software Architects, welche einen Überblick über die Kernelemente, sowie Tooling, für die Entwicklung und Bereitstellung von Cloud Native Applications in Microsoft Azure bekommen wollen. 

Begleitend zu den Theorieteilen der einzelnen Module, modernisieren wir eine App bestehend aus klassischem Monolithen mit UI in eine Cloud Native App mit Microservices (Catalog, Shop, Ordering, Payment, Production, Delivery) und die dazugehörigen Micro-Frontends. Dabei besprechen wir das Cloud Maturity Model und legen Wert auf die Verwendung von Best Practices & Cloud Design Patterns.

Wir vermitteln die Container Essentials und Konzepte, wie Konfiguration Management, Stateful Containers oder SideCar Pattern. Um einen Developer zentrischen Fokus zu garantieren, verteilen wir auf die Kubernetes basierenden Azure Container Apps, und behandeln dabei Themen wie Secrets, Revisions, Config Injection, Health Checks, Kubernetes Event Driven Auto-Scaling - KEDA, Stateful Containers und Jobs. Für die Authentifizierung von Service to Service Kommunikation verwenden wir Managed Identities und Service Connectors. Die hier erworbenen Kenntnisse können auch auf Azure Kubernetes Service (AKS) oder Azure Red Hat OpenShift angewendet werden. 

Wir nutzen Azure Functions, um Microservices zu implementieren, welche wahlweise Serverless aber auch also Container gehostet werden können. Im Speziellen gehen wir hier auf die Themen Durable Functions & Entities, deren Einsatzgebiet, sowie Monitoring ein.

Wir besprechen die Vorteile von NoSQL Datenbanken und begleiten Sie auf Ihrem Weg von Relational DB Design zum Cosmos DB NoSQL Data & Event Stores unter Berücksichtigung von Domain Driven Design (DDD). Dabei behandeln wir die Themen Datenmodellierung, Partitioning & Performance Optimierung, CRUD mit SDK's und Data Api Builder, Change Feed, Materialized Views, Event Sourcing und CQRS.

Wir vermitteln die Grundlagen von Event Driven Applications, Message Flow Design, Orchestration und Saga. Im Kapitel Distributed Application Runtime (Dapr) gehen wir neben Developer Environment Setup & Debugging, auf die Themen Service Invocation, State Management, Secrets, Configuration, Bindings, Pub/Sub, Dapr Actors, Observability & Distributed Tracing ein.

Wir publizieren, sichern und optimieren wir unsere Cloud Native App und deren Microservices mit API Management und Application Gateway und besprechen hier zusätlich die Themen Revision und Versions, Authentication, sowie die Umsatzung einer Backends for Frontend Pattern (BFF) mit Hilfe von GraphQL.

Last but not least verbinden wir unsere Micro Frontends mit Hilfe von Azure Event Grid, um ein Real Time Connected Orderstatus und Production Dashboards zu implementieren.

Die Demos, Lab Starters und Solutions werden größtenteils in .NET und Angular bereitgestellt. Fallweise können aber auch alternative Technologie Stacks verwendet werden, bzw. wird auf deren Docs verwiesen.

## Voraussetzungen und Zielgruppe

Kursteilnehmer, welche die Labs erfolgreich durchführen wollen, sollten praktische Erfahrung der im Seminar AZ-204 vermittelten Kenntnisse erworben haben. DevSecOps relevante Themen werden in einem separaten Kurs behandelt.

Audience: Azure Developers & Software Architects

## Themen

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
- Introduction to Clean Architecture
- Container Orchestration & DevOps
- Microservices Communication Patterns
- Architecture Overview of the Sample App & Services
- Cloud Architecture Design Patterns
- Provisioning of Azure Resources using Azure CLI & Bicep

### Container Essentials & Configuration Management

- Docker Development Workflow: Multistage Build, Run & Debug Recap
- Container builds using Azure Container Registry
- Using docker-compose.yaml to run multiple containers for local development
- Kubernetes Developer Essentials
- Container Configuration Management (Env Variables, Key Vault & Azure App Config Service)
- Understanding the Sidecar Pattern

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