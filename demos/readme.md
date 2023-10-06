# Designing & Implementing Cloud Native Applications using Microsoft Azure

Das Seminar richtet sich an Azure Entwickler und Software Architects, welche einen Überblick über die Kernelemente, sowie Tooling, für die Entwicklung und Bereitstellung von Cloud Native Applications in Microsoft Azure bekommen wollen. 

Begleitend zu den Theorieteilen der einzelnen Module, modernisieren wir eine App bestehend aus klassischem Monolithen mit UI in eine Cloud Native App mit Microservices (Catalog, Shop, State, Payment, Delivery) und die dazugehörigen Micro-Frontends. Dabei besprechen wir das Cloud Maturity Model und legen Wert auf die Verwendung von Best Practices & Cloud Design Patterns.

Wir vermitteln die Container Essentials und Konzepte, wie Konfiguration Management, Stateful Containers oder SideCar Pattern. Um einen Developer zentrischen Fokus zu garantieren, verteilen wir auf die Kubernetes basierenden Azure Container Apps, und behandeln dabei Themen wie Secrets, Revisions, Config Injection, Health Checks, Kubernetes Event Driven Auto-Scaling - KEDA, Stateful Containers und Jobs. Für die Authentifizierung von Service to Service Kommunikation verwenden wir Managed Identities und Service Connectors. Selbstverständlich können die hier erworbenen Kenntnisse auch auf Azure Kubernetes Service (AKS) oder Azure Red Hat OpenShift angewendet werden. 

Wir nutzen Azure Functions, um Microservices zu implementieren, welche wahlweise Serverless aber auch also Container gehostet werden können. Im Speziellen gehen wir hier auf die Themen Durable Functions & Entities, deren Einsatzgebiet, sowie Monitoring.

Dem Prinzip von Domain Driven Design folgend, vermitteln wir die Vorteile von NoSQL Datenbanken und begleiten Sie auf Ihrem Weg von Relational DB Design zum Cosmos DB NoSQL Api. Dabei behandeln wir die Themen Partitioning & Performance, Change Feed, Event Sourcing und CQRS.

Wir vermitteln die Grundlagen von Event Driven Applications, CloudEvents, Orchestration und Saga. Im Kapitel Distributed Application Runtime (Dapr) gehen wir neben Developer Environment Setup & Debugging, auf die Themen Service Invocation, State Management, Bindings, Pub/Sub, Secrets, Configuration, Observability, Distributed Tracing und  ein.

Last but not least publizieren, sichern und optimieren wir unsere Cloud Native App und deren Microservices mit API Management und Application Gateway und besprechen die Vor- und Nachteile von Micro-Frontends anhand zweiter Implementierungsbeispiele (Angular Real Time Frontend, Teams App).

Beispiele werden größtenteils in .NET und Angular implementiert. Fallweise können aber auch alternative Technologie Stacks verwendet werden, bzw. wird auf deren Docs verwiesen.

## Voraussetzungen und Zielgruppe

Kursteilnehmer, welche die Labs erfolgreich durchführen wollen, sollten praktische Erfahrung der im Seminar AZ-204 vermittelten Kenntnisse erworben haben. Mit Recap gekennzeichnete Themen sind Kurzzusammenfassungen von AZ-204 Inhalten als Refresher. DevSecOps relevante Themen werden in einem separaten Kurs behandelt.

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
- Connecting real time Micro Frontends using Event Grid 

### Introduction to Cloud Native Applications & Cloud Maturity Model

- What are Cloud Native Applications
- Cloud Maturity Model: Monolith vs Microservices Architecture
- Container Orchestration & DevOps
- Microservices Communication Patterns
- Cloud Architecture Design Patterns
- Architecture Overview of the sample app & services
- Provisioning of base class resources using Azure CLI & Bicep

### Container Essentials & Configuration Management

- Docker Development Workflow: Multistage Build, Run & Debug Recap
- Container builds using Azure Container Registry
- Using docker-compose.yaml to run multiple containers for local development
- Kubernetes Developer Essentials
- Container Configuration Management (Env Variables, Key Vault & Azure App Config Service)
- Understanding the Sidecar Pattern

### Developing & Publishing Microservices using Azure Container Apps (ACA)

- Introduction to Azure Container Apps 
- Azure Container Apps vs Kubernetes
- Publish Microservices (Ingress, Egress) and manage Revisions
- Secrets, Managed Identities & Service Connectors
- Using Azure App Configuration in Azure Container Apps
- Scaling & KEDA (Kubernetes Event Driven Auto-Scaling) 
- Stateful Apps using Volume Mounts & Persistent Storage
- Task Automation using Jobs
- Authentication in Azure Container Apps
- Health Probes, Monitoring, Logging & Observability

### Stateful Microservices using Azure Functions

- OData and Open API Support
- Hosting: Serverless vs Containers
- Managed Identities, Key Vault, and App Configuration
- Using Durable Functions to implement long running processes
- Monitoring Durable Functions
- Azure Durable Entities
- Aggregation & Virtual Actors

### NoSQL Data & Event storage using Cosmos DB

- From Relational to NoSQL: Do's and Don’ts
- Domain Driven Design (DDD) Basics & Bounded Context Pattern
- Using SDKs to interact with Cosmos DB
- Partitioning Strategies & Performance Optimization
- Implementing an Event Store using Event Sourcing
- Optimizing Read/Write Performance with Change Feed & CQRS 

### Designing and Implementing Message based & Event Driven Apps

- Messages vs Events & Queues vs Topics
- Common Message Brokers in Azure
- Publish & Subscribe
- Event Driven Architecture (EDA)
- Common Cloud Design Patterns used with Even Driven Architecture
- Domain Events vs Integration Events
- External Communication using Cloud Events
- Orchestration, Choreography, Saga Pattern

### Using Distributed Application Runtime - Dapr

- Introduction to Dapr 
- Understanding Dapr Architecture & Building Blocks
- Developer Environment Setup, Debugging & State Management
- Using Dapr Components in Azure Container Apps
- Service Invocation & Bindings
- Pub/Sub Messaging
- Secrets and Configuration
- Dapr Actors & Saga
- Observability and Distributed Tracing

### Optimizing and Securing Access using Api Management & Application Gateway

- API Management (APIM) Recap
- API Versions and Revisions using Azure Container Apps
- Authenticating to Backend Services using Managed Identity
- Understanding Gateway Pattern and Backends for Frontend Pattern

### Connecting Real Time Micro Frontends using Event Grid 

- Micro Frontends: Introduction & Benefits
- Angular Building Blocks Recap
- Publishing the Shop Micro Frontend using Azure Container Apps
- Progressive Web Apps (PWA) Recap
- Real Time Dashboards using Azure Event Grid 
