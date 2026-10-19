# Transactional Outbox: Atomic Publishing

> **_NOTE:_** Duration: about 10 minutes.

When `shipping-api` books a shipment, two things must happen: the shipment is stored, and `ShipmentBooked` reaches the subscribers of `shipment-events`, such as `tracking-service`. Those writes go to two different systems, so a crash between them either loses the event or announces a shipment that was never saved. Grey Logistics can't accept either outcome, because the tracking page would show a parcel that was never stored.

This topic covers the transactional outbox. The service saves the `ShipmentBooked` event document and its outbox event in one Cosmos DB transactional batch, and a change feed processor relays the events to Service Bus with duplicate detection on. The design follows the Architecture Center guidance, and TTL removes the outbox documents once the relay has published them.

## The Dual-Write Problem

A service that writes to a database and then publishes a message performs a dual write. The database and the message broker don't share one transaction, so one write can succeed while the other fails. The Architecture Center lists the usual causes: a network error, an outage of the message service, or a host failure right after the save.

Reversing the order doesn't fix it. A publish that succeeds followed by a failed save announces a shipment that doesn't exist.

| Approach | Failure between the two writes | Outcome |
| --- | --- | --- |
| Save, then publish | Publish fails | Shipment stored, event lost |
| Publish, then save | Save fails | Event published, shipment missing |
| Outbox in one batch | Batch fails | Nothing stored, nothing relayed |

The outbox removes the first two rows. The relay can still send an event twice, so the design also needs duplicate detection and idempotent consumers, both covered below. The Architecture Center describes the fix as committing the business object and its events in the same transactional batch, with a background processor that relays the events as messages.

## One Transactional Batch on One Partition Key

A Cosmos DB transaction is a transactional batch. A batch runs against one logical partition, holds up to 100 operations, and its request can't exceed 2 MB. The `shipments` container uses `/shipmentId` as its partition key, so the `ShipmentBooked` event document and its outbox event share a partition and commit together.

```csharp
using Microsoft.Azure.Cosmos;

public class ShipmentOutboxWriter(Container container)
{
    public async Task<bool> SaveBookingAsync(ShipmentDocument booked, ShipmentDocument outbox, CancellationToken cancellationToken)
    {
        var batch = container.CreateTransactionalBatch(new PartitionKey(booked.shipmentId))
            .CreateItem(booked)
            .CreateItem(outbox);

        using var response = await batch.ExecuteAsync(cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
```

The batch returns one status for the whole unit. The failing operation carries its own error code, such as 409 Conflict when the ID already exists, and every other operation reports 424 (failed dependency). Check `IsSuccessStatusCode` before the caller receives a success response.

Outbox events live in the same container, marked by a `documentType` value:

```json
{
  "id": "5f0c2a7e-8d43-4b7a-9c61-3e2d1f0a9b44",
  "shipmentId": "SHP-10482",
  "trackingNumber": "TRK-4A7F2C9B01",
  "documentType": "outbox",
  "eventType": "ShipmentBooked",
  "version": 1,
  "occurredAt": "2026-10-18T08:00:00Z",
  "scanType": null,
  "ttl": 864000,
  "data": {
    "ShipmentId": "SHP-10482",
    "TrackingNumber": "TRK-4A7F2C9B01",
    "PickupAddress": "1 Main Street",
    "PromisedBy": "2026-10-22T17:00:00Z"
  }
}
```

The `ttl` value of 864000 is ten days in seconds. The Architecture Center suggests a span of several days, so every consumer has time to recover from an outage before the event expires.

## Relaying With the Change Feed

The change feed is a persistent log of changes in a container, and Cosmos DB keeps the order of changes within one logical partition. A change feed processor reads new documents, sends each outbox event to a Service Bus topic, and then records its position in a lease container. If the relay restarts, it resumes from the last recorded position.

![Outbox relay over time: one transactional batch writes the ShipmentBooked event document (version 1) and its outbox event to partition SHP-10482, a retried batch sends the event twice, the shipment-events topic drops the copy by MessageId within its five-minute window, tracking-service and exception-agent each receive one message, the lease checkpoint advances, and TTL removes only the outbox document after 10 days while the event document stays](_images/outbox-relay.svg)

The relay filters for outbox documents, sets the Service Bus `MessageId` to the document `id`, and uses the shipment ID as the session ID so each shipment stays in order.

```csharp
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public class OutboxRelay(ServiceBusSender sender)
{
    public async Task HandleChangesAsync(ChangeFeedProcessorContext context, IReadOnlyCollection<ShipmentDocument> changes, CancellationToken cancellationToken)
    {
        foreach (var change in changes.Where(change => change.documentType == "outbox"))
        {
            var message = new ServiceBusMessage(BinaryData.FromString(JsonConvert.SerializeObject(change.data)))
            {
                MessageId = change.id,
                SessionId = change.shipmentId,
                Subject = change.eventType,
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
```

The delegate awaits every send before it returns. Learn warns that a processor can checkpoint before unawaited work finishes, and an unawaited send can then disappear.

If the delegate throws, the processor retries the same batch. The relay therefore sends some events twice, and the topic must drop the copies.

Duplicate detection does that by comparing `MessageId` within a history window. The window defaults to 10 minutes, with a minimum of 20 seconds and a maximum of 7 days. Duplicate detection requires the Standard or Premium tier, and the Basic tier doesn't support it.

The lab's AppHost sets the window to five minutes on `shipment-events` in `src/AppHost/Program.cs`, which the Service Bus emulator accepts. The Azure CLI example below uses one day.

```bash
az servicebus topic create --resource-group <resource-group> --namespace-name <namespace-name> --name shipment-events --enable-duplicate-detection true --duplicate-detection-history-time-window P1D
```

## Cleaning Up With TTL

Outbox events have no value after the relay publishes them, so TTL removes them. Cosmos DB deletes a document once its `ttl` seconds have passed since the last modification. The Architecture Center notes that this cleanup runs in the background on leftover request units, while a cleanup job would update each event document and spend request units on every pass.

TTL needs a container default. Learn states that item-level `ttl` applies only when the container's `DefaultTimeToLive` is present and not null. Setting the default to `-1` turns TTL on without expiring items by default.

```csharp
using Microsoft.Azure.Cosmos;

ContainerProperties properties = new("shipments", "/shipmentId")
{
    DefaultTimeToLive = -1
};

Container shipments = await database.CreateContainerAsync(properties);
```

The lab's `ShipmentsContainerSetup` creates the container when it is missing and replaces the properties of an existing one, so the default also applies to a `shipments` container that the AppHost created first.

Event documents leave out the `ttl` property, so they never expire. Outbox events carry their own `ttl`, so they expire after ten days.

## Key Topics

- [Implement the Transactional Outbox pattern by using Azure Cosmos DB](https://learn.microsoft.com/azure/architecture/databases/guide/transactional-out-box-cosmos)
- [Transactional batch operations in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/transactional-batch)
- [Change feed processor in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/change-feed-processor)
- [Duplicate detection in Azure Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/duplicate-detection)
- [Time to live in Azure Cosmos DB](https://learn.microsoft.com/azure/cosmos-db/time-to-live)
