# Module 6: Event Sourcing & CQRS with Cosmos DB

Module 6 is where Contoso Logistics keeps its history. `shipping-api` stores every shipment change as an immutable event in Cosmos DB, and the tracking page reads from a separate model that a change feed processor builds from those events. The module follows the event streams of Module 5 and comes before the Dapr portability work of Module 7.

```mermaid
flowchart TB
    Client["Client<br/>Topic 03"]
    Api["shipping-api<br/>POST /shipments<br/>command side<br/>Topic 03"]
    Handler["BookShipmentHandler<br/>Topic 03"]
    Writer["ShipmentOutboxWriter<br/>SaveBookingAsync<br/>Topic 01"]
    Shipments["Cosmos DB shipments<br/>TransactionalBatch<br/>ShipmentBooked event v1<br/>plus its outbox event<br/>partition /shipmentId<br/>Topics 01 and 06"]
    Upcaster["ShipmentBookedUpcaster<br/>schemaVersion 1 read<br/>as ShipmentBookedV2<br/>Topic 07"]
    Repository["ShipmentRepository<br/>replays event documents<br/>Topic 06"]
    Feed["change feed<br/>Topic 02"]
    Relay["OutboxRelay<br/>sets MessageId<br/>Topic 01"]
    Bus["Service Bus topic<br/>shipment-events<br/>duplicate detection<br/>Topic 01"]
    Projection["TrackingProjection<br/>Topics 02 and 05"]
    Redis["Redis cache<br/>tracking:{trackingNumber}<br/>Topic 05"]
    Tracking["tracking read model<br/>partition /trackingNumber<br/>Topics 02 and 04"]
    Query["tracking-service<br/>TrackingQueryHandler<br/>query side<br/>Topic 03"]
    Client -->|"BookShipment"| Api
    Api --> Handler
    Handler --> Writer
    Writer --> Shipments
    Shipments -->|"event documents"| Repository
    Upcaster -.->|"technique, Topic 07"| Repository
    Shipments --> Feed
    Feed --> Relay
    Relay --> Bus
    Feed --> Projection
    Projection -->|"KeyDeleteAsync"| Redis
    Projection -->|"UpsertItemAsync"| Tracking
    Client -->|"GET /tracking/{trackingNumber}"| Query
    Redis -->|"cache-aside"| Query
    Tracking -->|"on a cache miss"| Query
    classDef client fill:#F1F5F9,stroke:#64748B,color:#0F172A
    classDef service fill:#DBEAFE,stroke:#1D4ED8,color:#0F172A
    classDef store fill:#DCFCE7,stroke:#15803D,color:#0F172A
    classDef broker fill:#FEF3C7,stroke:#B45309,color:#0F172A
    classDef cache fill:#F3E8FF,stroke:#6D28D9,color:#0F172A
    class Client client
    class Api,Handler,Writer,Relay,Projection,Query,Upcaster,Repository service
    class Shipments,Feed,Tracking store
    class Bus broker
    class Redis cache
```

After this module you can save a booking and its events in one atomic batch, relay the events to Service Bus without losses, and project the change feed into a read model. You can serve tracking queries from Azure Managed Redis with invalidation from the change feed, and you can evolve event schemas without breaking the consumers that already run.

The topics use the Microsoft.Azure.Cosmos v3 SDK on .NET 10, with Aspire for the cache: a local Redis container in the labs and Azure Managed Redis in Azure. The outbox comes first because later topics reuse its batch writer and its relay.

## Topics

| Topic | Description |
| --- | --- |
| [Transactional Outbox: Atomic Publishing](./01-outbox/readme.md) | TransactionalBatch on one partition key, change feed relay to Service Bus with MessageId and duplicate detection, TTL cleanup |
| [Change Feed Processor: Projections](./02-change-feed-processor/readme.md) | GetChangeFeedProcessorBuilder, lease container, WithInstanceName, change feed estimator, CosmosDBTrigger, latest version and all versions and deletes modes |
| [CQRS: Separate Writes from Reads](./03-cqrs/readme.md) | Command and query handlers, separate containers and models, GetItemQueryIterator, eventual consistency in the UI |
| [Read Models: Indexing, RUs, Consistency](./04-read-models/readme.md) | IndexingPolicy paths and composite indexes, ItemResponse.RequestCharge, ReadItemAsync, session tokens |
| [Managed Redis: Change-Feed Cache Invalidation](./05-redis-cache/readme.md) | Azure Managed Redis, cache-aside with StringSetAsync and KeyDeleteAsync, Microsoft.Azure.StackExchangeRedis, AddAzureManagedRedis, AddRedis for the local lab container |
| [Event Store: Aggregates & Snapshots](./06-event-store/readme.md) | CreateItemAsync conflicts, IfMatchEtag, aggregate replay with GetItemQueryIterator, snapshot documents |
| [Event Versioning: Evolve Schemas Safely](./07-event-versioning/readme.md) | Additive changes, upcasting on read, CloudEvents type and dataschema attributes, consumer tolerance |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/review` | Reviews the outbox relay and projection handlers for missing awaits, retries that could duplicate writes, and upserts that aren't idempotent. |
| `/security-review` | Audits connection settings, Entra role assignments on Cosmos DB and Redis, and any key or connection string that reached code or configuration. |
| `/cost` | Shows token usage and session cost for the current session, which helps when a long refactor of the read model code runs for hours. |
| `/update-config` | Edits `settings.json` through guided prompts, for example to allow the `dotnet test` and `az servicebus` commands that the labs run. |

## Key Topics covered in this module

- [Event Sourcing pattern](https://learn.microsoft.com/azure/architecture/patterns/event-sourcing)
- [Command Query Responsibility Segregation (CQRS) pattern](https://learn.microsoft.com/azure/architecture/patterns/cqrs)
- [Implement the Transactional Outbox pattern by using Azure Cosmos DB](https://learn.microsoft.com/azure/architecture/databases/guide/transactional-out-box-cosmos)
- [Change feed in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/change-feed)
- [What is Azure Managed Redis?](https://learn.microsoft.com/azure/redis/overview)
- [CloudEvents core specification](https://github.com/cloudevents/spec/blob/main/cloudevents/spec.md)
