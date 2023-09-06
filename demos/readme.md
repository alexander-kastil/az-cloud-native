# Designing & Implementing Cloud Native Applications using Microsoft Azure

Das Seminar richtet sich an Azure Entwickler und Software Architects, welche einen Überblick über die Kernelemente, sowie des Toolings, für die Entwicklung und Bereitstellung von Cloud Native Applications in Microsoft Azure bekommen wollen. 

Begleitend zu den Theorieteilen der einzelnen Module, modernisieren wir eine App bestehend aus klassischem Monolithen mit UI in eine Cloud Native App mit Microservices (Catalog, Shop, State, Payment, Delivery) und die dazu gehörigen Micro-Frontends. Dabei besprechen wir das Cloud Maturity Model und legen Wert auf die Verwendung von Best Practices & Cloud Design Patterns, sowie deren Abbildung mit Software Architektur Diagrammen.

Wir vermitteln die Container Essentials und Konzepte, wie Konfiguration Management, Stateful Containers oder SideCar Pattern. Um einen Developer zentrischen Fokus zu garantieren, verteilen wir auf die Kubernetes basierenden Azure Container Apps, und behandeln dabei Themen wie Secrets, Revisions, Config Injection, Health Checks, Kubernetes Event Driven Auto-Scaling - KEDA, Stateful Containers und Jobs. Für die Authentifizierung von Service to Service Kommunikation verwenden wir Managed Identities und Service Connectors. Selbstverständlich können die Apps auch nach AKS verteilt werden.

Wir nutzen Azure Functions, um Microservices zu implementieren, welche wahlweise Serverless aber auch also Container gehostet werden können. Im Speziellen gehen wir hier auf die Themen Durable Functions & Entities, deren Einsatzgebiet, sowie Monitoring.

Dem Prinzip von Domain Driven Design folgend, vermitteln wir die Vorteile von NoSQL Datenbanken und begleiten Sie auf Ihrem Weg von Relational DB Design zum Cosmos DB NoSQL Api. Dabei behandeln wir die Themen Partitioning & Performance, Change Feed, Event Sourcing und CQRS.

Wir vermitteln die Grundlagen von Event Driven Applications, CloudEvents, Orchestration und Saga. Im Kapitel Distributed Application Runtime (Dapr) gehen wir neben Developer Environment Setup & Debugging, auf die Themen  Service Invocation, Pub/Sub, State Management, Secrets, Configuration, Observability, Distributed Tracing und Actors ein.

Last but not least publizieren, sichern und optimieren wir unsere Cloud Native App und deren Microservices mit API Management und Application Gateway und besprechen die Vor- und Nachteile von Micro-Frontends Anhand zweiter Implementierungsbeispiele (Angular Real Time Frontend, Teams App).

In allen Phasen wird Authentication und Authorization mittels Microsoft Identity sichergestellt und ein automatisiertes Deployment der App ist mittels Azure CLI und / oder BICEP gewährleistet.

Beispiele werden größtenteils in .NET und Angular implementiert. Fallweise können aber auch alternative Technologie Stacks verwendet werden, bzw. wird auf deren Docs verwiesen.

## Voraussetzungen und Zielgruppe

Kursteilnehmer, welche die Labs erfolgreich durchführen wollen, sollten Kenntnisse und Erfahrung der in AZ-204 vermittelten Kenntnisse erworben haben. Mit RECAP gekennzeichnete Themen sind Kurzzusammenfassungen von AZ-204 Inhalten als Refresher. DevSecOps relevante Themen werden in einem separatem Kurs behandelt.

Audience: Azure Developers & Software Architects

## Themen

- Introduction to Cloud Native Applications
- Building Blocks & Architecture Overview
- Container Essentials & Configuration Management
- Introduction to Azure Container Apps (ACA)
- Implementing Microservices using Azure Functions
- NoSQL Data storage using Cosmos DB
- Designing & Implementing Event Driven Apps
- Using Distributed Application Runtime - Dapr
- Optimizing and Securing API Access using Api Management
- Implementing Real Time & Micro-Frontends 

### Introduction to Cloud Native Applications

- What are Cloud Native Applications
- Cloud Matury Model: From Monolith to Microservices
- What are Cloud Architecture Design Patterns
- Microservices Communication Patterns (Sync, Async, Event Driven)
- API gateway pattern versus the Direct client-to￾microservice communication

### Building Blocks & Architecture Overview

- Architecture and overwiew ofthe sample app building blocks
- Hosting: Azur Container Apps and Functions (Serverless / Containers)
- Authentication & Authorization: Microsoft Identity & Managed Identities
- State & Data: Azure Cosmos DB, Azure SQL, Blob Storage, Redis
- Configuration Management, Secrets: Key Vault, App Config Service
- Messaging Brokers: Service Bus, Event Hub, Event Grid
- Real Time: Azure SignalR Service, Azure Web PubSub
- Access & Management: API Management & Application Gateway
- Provisioning of base building blocks using Azure CLI & Bicep

### Container Essentials & Configuration Management

- Docker Development Workflow: Multistage Build, Run & Debug
- Publish images to Azure Container Registry
- Using docker-compose.yaml to locally test multiple containers
- Kubernetes Developer Essentials
- Configuration Management (Env Variables, KeyVault & Azure App Config Service)
- Understanding the Sidecar Pattern

### Introduction to Azure Container Apps (ACA)

- What is Azure Container Apps
- Azure Container Hosts: Azure Container Apps vs Kubernetes
- Deploying a multi-container App (Ingress, Exgress)
- Working with Secrets & Revisions
- Using Managed Identities & Service Connectors to access Azure resources
- Using Azure App Configuration in Azure Container Apps
- Health Probes, Monitoring, Logging & Observability
- Introduction to Scaling & KEDA (Kubernetes Event Driven Auto-Scaling) 
- Stateful Apps using Volume Mounts & Persistent Storage
- Using Jobs in Azure Container Apps

### Implementing Microservices using Azure Functions

- OData and Open API Support
- Hosting: Serverless vs Containers
- Hosting and Scaling containerized Functions
- Managed Identities, Key Vault and App Configuration
- Dependency Injection and Data Access using EF Core
- Using Durable Functions to implement long running processes
- Monitoring Durable Functions
- Azure Durable Entities: Aggregation & virtual Actors

### NoSQL Data storage using Cosmos DB

- From Relational to NoSQL: Does and Don'ts
- Domain Driven Design (DDD) and Bounded Context Pattern
- Optimize Partitioning & Performance 
- Using SDKs and Entity Framework
- Cosmos DB Change Feed and Event Sourcing
- Understanding the CQRS Pattern

### Designing & Implementing Event Driven Apps

- Introduction to Event Driven Architecture
- Messages vs Events & Queues vs Topics
- Common Message Brokers in Azure
- Common Cloud Design Patterns used with Even Driven Architecture
- Domain Events vs Integration Events
- External Communication using CloudEvents
- Orchestration vs Choreography, Saga Pattern

### Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture & Building Blocks
- Environment Setup, Debugging & State Management
- Secrets and Configuration
- Publish & Subscribe
- Service Invocation & Bindings
- Observability and Distributed Tracing
- Introduction to Dapr Actors
- Using Dapr Components in Azure Container Apps

### Optimizing and Securing API Access using Api Management

- API Management (APIM) Recap
- Understanding Gateway Pattern and Backends for Frontend Pattern
- API Versions and Revisions
- Securing API Access using Managed Identities

### Implementing Real Time & Micro-Frontends 

- Introduction to Micro Frontends
- Implementing Reactive Real Time Frontends using Event Grid & Azure Web PubSub
- Implementing a Micro Frontend as Teams App.
