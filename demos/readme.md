# Designing & Implementing Cloud Native Applications using Microsoft Azure

Das Seminar richtet sich an Azure Entwickler und Software Architects, welche einen Überblick über die Kernelemente der Entwicklung und bereitstelle von Cloud Native Applications in Microsoft Azure bekommen wollen. 

Begleitend zu den Theorieteilen der einzelnen Module, modernisieren wir eine App bestehend aus klassischem Monolithen mit UI in eine Cloud Native App mit Microservices (Catalog, State, Payment, Delivery, Purchasing) und Micro Frontends um. Dabei legen wir Wert auf die Verwendung von Best Practices und Cloud Design Patterns, sowie deren Abbildung mit Software Architektur Diagrammen.

Wir vermitteln die Container Essentials, und Konzepte wie Stateful Containers oder SideCar Pattern und besprechen im Detail mögliche Refactorings bezüglich Bereitstellung in den Kubernetes basierenden Azure Container Apps und behandeln dabei Themen wie Secrets, Revisions, Config Injection, Health Checks, Kubernetes Event Driven Auto-Scaling - KEDA.

Dem Prinzip von Domain Driven Design folgend, vermitteln wir die Vorteile von NoSQL Datenbanken und begleiten Sie auf Ihrem Weg von Relational DB Design zum Cosmos DB NoSQL Api. Dabei behandeln wir auch die Themen Change Feed, Event Sourcing und CQRS.

Wir vermitteln die Grundlagen von Event Driven Applications, deren Transaktionsmustern, die wir mittels Saga Pattern implementieren und verbinden die einzelnen Services mittels Distributed Application Runtime (Dapr).

Wir nutzen Durable Functions, um Microservices zu implementieren, welche wahlweise Serverless aber auch also Container gehostet werden können. Im Speziellen gehen wir hier auf die Themen Durable Entities, Durable Monitoring und Durable Saga Pattern ein.

Last but not least publizieren und sichern wir die App, und deren Microservices mit API Management und Application Gateway, um dann noch unser Reactive Angular UI mit Client Side State in Echtzeit mittels Azure Web PubSub aktuell zu halten.

In allen Phasen wird Authentication und Authorization mittels Microsoft Identity sichergestellt und ein automatisiertes Deployment der App ist mittels Azure CLI und / oder BICEP gewährleistet.

Beispiele werden größtenteils in .NET, Angular und React implementiert. Fallweise können aber auch alternative Technologie Stacks (Spring Boot) verwendet werden, bzw. wird auf deren Docs verwiesen.

## Voraussetzungen und Zielgruppe

Kursteilnehmer, welche die Labs erfolgreich durchführen wollen, sollten Kenntnisse und Erfahrung der in AZ-204 vermittelten Kenntnisse erworben haben. Mit RECAP gekennzeichnete Themen sind Kurzzusammenfassungen von AZ-204 Inhalten als Refresher. DevSecOps relevante Themen werden in einem separatem Kurs behandelt.

Audience: Azure Developers & Software Architects

## Themen

- Introduction to Cloud Native Applications
- Recap: Building Blocks & Architecture Overview
- Container Essentials: Optimizing Services & Front Ends
- Schemaless and Event Optimized Data storage using Cosmos DB
- Implementing Microservices using Durable Azure Functions
- Introduction to Azure Container Apps (ACA) and Kubernetes Event-Driven Autoscaling (KEDA)
- Designing & Implementing Event Driven Apps
- Connecting Microservices using Distributed Application Runtime - Dapr
- Optimizing and Securing API Access using Api Management
- Implementing Reactive Micro Frontends using Azure Web PubSub

### Introduction to Cloud Native Applications

- What are Cloud Native Applications
- App Monolith vs Microservices
- Domain Driven Design (DDD) and Bounded Context Pattern
- Microservices Communication Patterns (Sync, Async, Event Driven)
- Api Gateway Pattern, Frontend Aggregation Pattern
- What are Event Driven Applications
- What are Cloud Architecture Design Patterns
- Creating Software Architecture Diagrams

### Recap: Building Blocks & Architecture Overview

- Hosting: Containers, Kubernetes and Functions (Serverless / Containers)
- Storage: Azure Cosmos DB, Azure SQL, Blob Storage
- Configuration Management, Secrets: Key Vault, App Config Service
- Messaging Brokers: Service Bus, Event Hub, Event Grid
- Real Time: Azure SignalR Service, Azure Web PubSub
- Access & Management: API Management & Application Gateway
- Authentication & Authorization: Microsoft Identity & Managed Identities

## Container Essentials & Patterns

- Container Recap (Multistage Build, Run, Debug, Publish to ACR)
- Configuration Management Options (Env Variables, ConfigMaps, Azure App Config Service)
- Docker Development Workflow and Debugging
- Using docker-compose.yaml to locally test multiple containers
- Stateful Containers using Azure Blob Storage and Volume Mounts
- Understanding and using Sidecar Pattern
- Azure Container Hosts: Azure Container Apps vs Kubernetes

## Schemaless and Event Optimized Data storage using Cosmos DB

- Cosmos DB Partitioning Strategies
- From Relational to Schemaless: Does and Don'ts
- Implementing Domain Driven Design for microservices
- Cosmos DB Change Feed and Event Sourcing
- Understanding and implementing the CQRS Pattern

## Implementing Microservices using Durable Azure Functions

- Serverless and Azure Functions Recap
- Hosting: Serverless vs Containers
- Implementing OData and Open API Support
- Hosting and Scaling Function Apps in Containers
- Durable Functions and Patterns
- Using Azure Durable Entities for Long running processes and background Tasks
- Monitoring Durable Functions
- Implementing a Microservice using Azure Durable Functions
- Implementing a Saga Pattern using Durable Functions
- Changing Storage Providers in Azure Durable Functions

# Introduction to Azure Container Apps (ACA)

- What is Azure Container Apps
- Deploying a muliti-container App (Ingress, Exgress)
- Working with Secrets
- Introduction to KEDA (Kubernetes Event Driven Auto-Scaling) 
- Working with Revisions
- Container Apps Authentication and Authorization using Managed Identities
- Container Apps Monitoring and Logging (Observability)

## Connecting Microservices using Distributed Application Runtime - Dapr

- Introduction to Dapr
- Dapr Environment Setup & Tooling
- Understanding Dapr Architecture
- Understanding Dapr Pub/Sub
- Using Dapr Components to interact with Azure Services
- Enhance Performance using Dapr State Management

## Designing & Implementing Event Driven Apps

- Introduction to Event Driven Architecture
- Common Message Broker Types in Azure
- Messages vs Events
- Message Patterns: Queues vs Topics (Pub/Sub)
- What to choose when: Service Bus vs Event Hub vs Event Grid
- Choosing the Messaging Broker: Features and Use-Cases
- Common Cloud Design Patterns used with Even Driven Architecture
- Event Sourcing and Integration Events
- Publishing & Subscribing Event in Microservices
- Implementing Transactions using the Saga Pattern
- Orchestration vs Choreography
- Refactor Microservices to support Event Based Communication
- Debugging Event Driven Applications

## Optimizing and Securing API Access using Api Management

- API Management (APIM) Recap
- APIM Policies Recap (Quotas, Throttling, Mock Response, Retry, ...)
- Understanding Gateway Pattern and Backends for Frontends Pattern
- API Versions and Revisions
- Securing API Access using Authentication & Managed Identities- 
- Using Redis Cache in API Management
- Intro to GraphQL and Use Cases
- Introduction to APIM Synthetic GraphQL

## Implementing Real Time Micro-Frontends & User Interfaces

- Event Grid Recap
- Real Time Options: SignalR vs Azure Web PubSub
- Introduction to Micro Frontends
- Implementing Reactive Real Time Frontends using Event Grid & Azure Web PubSub
- Implementing a Micro Frontend as Teams App.
