# CQRS: Separate Writes from Reads

> **_NOTE:_** Duration: about 8 minutes.

Shipments are written by customers and dispatchers, and read by the tracking page, the ops console, and notification emails. Writes must enforce business rules, such as applying a reroute only after the customer approves it.

This topic separates the two sides. `shipping-api` accepts commands and writes the Shipment aggregate, and `tracking-service` answers queries from the `tracking` read model, which lives in its own container with its own classes. The UI must show that the read model can lag behind a successful write.

## Commands Change State, Queries Return Views

Reads need fast answers shaped for the screen. One model that serves both makes each side harder to change. A command names a business task, such as `BookShipment` or `RerouteParcel`.

The handler validates the command, applies the business rules, and records the result. A query never changes state. It returns a view shaped for one screen.

| Concern | Command side (`shipping-api`) | Query side (`tracking-service`) |
| --- | --- | --- |
| Model | Shipment aggregate | Scan records shaped for the tracking page |
| Store | `shipments` event store, partition `/shipmentId` | `tracking` read model, partition `/trackingNumber` |
| Rules | Validation and invariants | None, only projection and filtering |
| Scaling | Fewer writers, guarded by concurrency checks | More readers behind the same model |

Both sides change for different reasons, such as a new screen on the read side and a new business rule on the write side. Each side can be deployed and scaled on its own schedule.

The command handler receives the command, builds the aggregate, and saves it through the transactional outbox writer from the first topic.

```csharp
public sealed record BookShipment(string ShipmentId, string CustomerId, string PickupAddress, string DeliveryAddress, DateTimeOffset PromisedBy);

public sealed class BookShipmentHandler(ShipmentOutboxWriter writer)
{
    public async Task<bool> HandleAsync(BookShipment command, CancellationToken cancellationToken)
    {
        Shipment shipment = Shipment.Book(command);
        OutboxEvent booked = OutboxEvent.ShipmentBooked(shipment);

        return await writer.SaveBookingAsync(shipment, booked, cancellationToken);
    }
}
```

The command returns only a success flag, and the shipment view comes from a query. That split lets the tracking team change the query shape without touching the booking rules.

The query handler reads only the read model. It knows nothing about the aggregate, and it never calls `shipping-api`.

```csharp
using Microsoft.Azure.Cosmos;

public sealed class TrackingQueryHandler(Container tracking)
{
    public async Task<IReadOnlyList<ScanRecord>> HandleAsync(string trackingNumber, CancellationToken cancellationToken)
    {
        QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.trackingNumber = @trackingNumber ORDER BY c.occurredAt ASC")
            .WithParameter("@trackingNumber", trackingNumber);

        FeedIterator<ScanRecord> iterator = tracking.GetItemQueryIterator<ScanRecord>(
            query,
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(trackingNumber) });

        List<ScanRecord> scans = new();

        while (iterator.HasMoreResults)
        {
            FeedResponse<ScanRecord> page = await iterator.ReadNextAsync();
            scans.AddRange(page);
        }

        return scans;
    }
}
```

This topic uses plain handler classes that the DI container registers. No mediator library is required. MediatR 13 and later is commercially licensed, so the module doesn't depend on it.

Each handler depends only on the classes it uses, so a unit test can pass in a fake outbox writer or a fake container.

## Separate Containers and Models

The write model and the read model never share a class. The Shipment aggregate holds behavior and invariants. The `ScanRecord` holds only what the tracking page needs, and the projection creates each record from a change feed event.

The read model can be rebuilt from the event store at any time, because the event store stays the source of truth. A second read model, such as an ops search index, can subscribe to the same events without any change to the write side. Each read model answers the questions of one screen.

Separate containers also let each side choose its partition key. The event store partitions by `/shipmentId`, because every write touches one shipment. The read model partitions by `/trackingNumber`, because every tracking page asks for one parcel.

The Architecture Center notes that the read store can use a schema optimized for queries, and that a read-only replica or a different structure works too.

## Eventual Consistency in the Ops Console

The projection runs after the write commits, so the tracking container can lag the event store by the change feed delay. A customer who books a shipment and opens the tracking page immediately might see nothing yet. The UI must show that state to the user.

The window between a successful write and the first visible read is usually short, but the UI must not assume it is zero. A 202 response with a status link makes that state explicit.

The booking endpoint returns the accepted booking, usually with HTTP 202 Accepted. The console then shows the shipment as received and polls the tracking query until the first scan appears. The Architecture Center describes the same trade-off and warns that a user can act on stale data.

Session consistency can close the gap for the same user, and the next topic shows how to pass the session token between requests.

## Key Topics

- [Command Query Responsibility Segregation (CQRS) pattern](https://learn.microsoft.com/azure/architecture/patterns/cqrs)
- [Idempotent Consumer pattern](https://learn.microsoft.com/azure/architecture/patterns/idempotent-consumer)
- [Change feed design patterns in Azure Cosmos DB for NoSQL](https://learn.microsoft.com/azure/cosmos-db/change-feed-design-patterns)
- [Consistency levels in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/consistency-levels)
- [Transactional batch operations in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/transactional-batch)
