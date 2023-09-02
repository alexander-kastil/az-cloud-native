# Designing & Implementing Cloud Native Applications using Microsoft Azure

Das Seminar richtet sich an Azure Entwickler und Software Architects, welche einen Überblick über die Kernelemente der Entwicklung und bereitstelle von Cloud Native Applications in Microsoft Azure bekommen wollen. 

Begleitend zu den Theorieteilen der einzelnen Module, modernisieren wir eine App bestehend aus klassischem Monolithen mit UI in eine Cloud Native App mit Microservices (Catalog, Shop, State, Payment, Delivery) und Micro Frontends. Dabei legen wir Wert auf die Verwendung von Best Practices und Cloud Design Patterns, sowie deren Abbildung mit Software Architektur Diagrammen.

Wir vermitteln die Container Essentials, und Konzepte wie Stateful Containers oder SideCar Pattern und besprechen im Detail mögliche Refactorings bezüglich Bereitstellung in den Kubernetes basierenden Azure Container Apps und behandeln dabei Themen wie Secrets, Revisions, Config Injection, Health Checks, Kubernetes Event Driven Auto-Scaling - KEDA.

Dem Prinzip von Domain Driven Design folgend, vermitteln wir die Vorteile von NoSQL Datenbanken und begleiten Sie auf Ihrem Weg von Relational DB Design zum Cosmos DB NoSQL Api. Dabei behandeln wir auch die Themen Change Feed, Event Sourcing und CQRS.

Wir vermitteln die Grundlagen von Event Driven Applications, deren Transaktionsmustern, die wir mittels Saga Pattern implementieren und verbinden die einzelnen Services mittels Distributed Application Runtime (Dapr).

Wir nutzen Durable Functions, um Microservices zu implementieren, welche wahlweise Serverless aber auch also Container gehostet werden können. Im Speziellen gehen wir hier auf die Themen Durable Entities, Durable Monitoring und Durable Saga Pattern ein.

Last but not least publizieren und sichern wir die App, und deren Microservices mit API Management und Application Gateway, um dann noch unser Reactive Angular  UI mit Client Side State in Echtzeit mittels Azure Web PubSub aktuell zu halten.

In allen Phasen wird Authentication und Authorization mittels Microsoft Identity sichergestellt und ein automatisiertes Deployment der App ist mittels Azure CLI und / oder BICEP gewährleistet.

Beispiele werden größtenteils in .NET, Angular und React implementiert. Fallweise können aber auch alternative Technologie Stacks (Spring Boot) verwendet werden, bzw. wird auf deren Docs verwiesen.

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
- Implementing Real Time Micro-Frontends 

### Introduction to Cloud Native Applications

- What are Cloud Native Applications
- App Monolith vs Microservices
- What are Cloud Architecture Design Patterns
- Microservices Communication Patterns (Sync, Async, Event Driven)
- Api Gateway Pattern, Frontend Aggregation Pattern
- What are Event Driven Applications

### Building Blocks & Architecture Overview

- Food App - food ordering and delivery application
- Hosting: Containers, Kubernetes and Functions (Serverless / Containers)
- Storage: Azure Cosmos DB, Azure SQL, Blob Storage
- Configuration Management, Secrets: Key Vault, App Config Service
- Messaging Brokers: Service Bus, Event Hub, Event Grid
- Real Time: Azure SignalR Service, Azure Web PubSub
- Access & Management: API Management & Application Gateway
- Authentication & Authorization: Microsoft Identity & Managed Identities
- Provisioning base Ressources using Azure CLI & Bicep

### Container Essentials & Configuration Management

- Container Recap (Multistage Build, Run, Debug, Publish to ACR)
- Docker Development Workflow and Debugging
- Using docker-compose.yaml to locally test multiple containers
- Configuration Management using Environment Variables, Secrets and Azure App Config Service
- Stateful Containers using Azure Blob Storage and Volume Mounts
- Understanding and using Sidecar Pattern

### Introduction to Azure Container Apps (ACA)

- What is Azure Container Apps
- Azure Container Hosts: Azure Container Apps vs Kubernetes
- Deploying a muliti-container App (Ingress, Exgress)
- Working with Secrets & Revisions
- Using Managed Identities & Service Connectors to access services
- Using Azure App Configuration in Azure Container Apps
- Health Probes, Monitoring, Logging & Observability
- Introduction to Scaling & KEDA (Kubernetes Event Driven Auto-Scaling) 
- Authentication and Authorization using Microsoft Identity Platform

### Implementing Microservices using Azure Functions

- OData and Open API Support
- Hosting: Serverless vs Containers
- Hosting and Scaling containerized Functions
- Managed Identites, Key Vault and App Configuration
- Dependency Injection and Data Access using EF Core
- Durable Functions and Patterns
- Monitoring Durable Functions
- Azure Durable Entities & Actors

### NoSQL Data storage using Cosmos DB

- From Relational to NoSQL: Does and Don'ts
- Domain Driven Design (DDD) and Bounded Context Pattern
- Optimize Partitioning & Performance 
- Using SDKs and Entity Framework
- Cosmos DB Change Feed and Event Sourcing
- Understanding the CQRS Pattern

### Designing & Implementing Event Driven Apps

- Introduction to Event Driven Architecture
- Common Message Broker Types in Azure
- Messages vs Events & Queues vs Topics
- Common Cloud Design Patterns used with Even Driven Architecture
- Publishing & subscribing messages with Cloudevents
- Event Sourcing and Integration Events
- Implementing a Saga Pattern using Durable Functions
- Orchestration vs Choreography
- Debugging Event Driven Applications

### Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture
- Environment Setup, Debugging & State Management
- Using Dapr Components in Azure Container Apps
- Secrets and Configuration
- Publish & subscribe
- Service Invocation & Bindings
- Observability and Distributed Tracing
- Introduction to Actors

### Optimizing and Securing API Access using Api Management

- API Management (APIM) Recap
- Understanding Gateway Pattern and Backends for Frontends Pattern
- API Versions and Revisions
- Securing API Access using Authentication & Managed Identities

### Implementing Real Time Micro-Frontends 

- Introduction to Micro Frontends
- Real Time Options: SignalR vs Azure Web PubSub
- Implementing Reactive Real Time Frontends using Event Grid & Azure Web PubSub
- Implementing a Micro Frontend as Teams App.
