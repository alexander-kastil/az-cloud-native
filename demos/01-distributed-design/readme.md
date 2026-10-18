# Module 1: Designing Event-Driven Distributed Systems

Module 1 sets the design vocabulary for the rest of the class. Contoso Logistics begins as one application that books shipments, tracks parcels and assigns vans, and the module asks where its seams belong before anyone writes a service. Afterwards, you can decide when a modular monolith is the right first step, when a service boundary earns its cost, and which messages the services should exchange.

The topics follow the order of a design decision. You compare a monolith with microservices, then choose between synchronous calls and asynchronous messages. Event storming finds the boundaries, event types name what crosses them, reliability patterns keep each flow correct under failure, and the broker comparison matches each flow to an Azure service.

This module has no hands-on demo. Its topics explain concepts with diagrams, tables and small C# records, and the later modules build the patterns named here on the Contoso Logistics app.

## Topics

| Topic | Description |
| --- | --- |
| [Modular Monolith vs. Microservices](./01-monolith-vs-microservices/readme.md) | Module boundaries inside one deployable, service ownership, and the operational cost of each split. |
| [Sync vs. Async: Commands & Events](./02-sync-async/readme.md) | HTTP request/reply with 202 Accepted and polling, command and event messages, and temporal coupling. |
| [Event Storming: Find Service Boundaries](./03-event-storming/readme.md) | Sticky-note grammar, bounded contexts, context maps, and the Contoso Logistics service list. |
| [Event Types: Domain, Integration, CloudEvents](./04-event-types/readme.md) | Domain and integration events, notification versus state transfer, and the CloudEvents 1.0 envelope with C# records. |
| [Reliability Patterns: Outbox, Saga, Idempotency](./05-reliability-patterns/readme.md) | The failures that break event flows and the patterns that prevent them, with links to the topics that implement them. |
| [Azure Messaging: Pick the Broker](./06-pick-the-broker/readme.md) | Service Bus, Event Grid, Event Hubs and Dapr pub/sub compared by delivery, ordering, retention and consumption. |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/review` | Reviews a branch that moves a service boundary and flags event contracts that the change breaks for consumers. |
| `/security-review` | Audits a message handler for trust in payload fields, such as a parcel ID that a scanner sends without a check. |
| `/model` | Switches to a larger model when you weigh several bounded contexts at once and need the whole design in view. |

## Key Topics covered in this module

- [Microservices architecture style](https://learn.microsoft.com/azure/architecture/guide/architecture-styles/microservices)
- [Event-driven architecture style](https://learn.microsoft.com/azure/architecture/guide/architecture-styles/event-driven)
- [Use domain analysis to model microservices](https://learn.microsoft.com/azure/architecture/microservices/model/domain-analysis)
- [Asynchronous Request-Reply pattern](https://learn.microsoft.com/azure/architecture/patterns/asynchronous-request-reply)
- [Choose between Azure Event Grid, Event Hubs, and Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/compare-messaging-services)
