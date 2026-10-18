# Event Versioning: Evolve Schemas Safely

> **_NOTE:_** Duration: about 9 minutes.

`ShipmentBooked` events have sat in the event store since the first release. The business now wants a `serviceLevel` on every booking, and `billing-service` must price express deliveries differently.

Old events can't change, and consumers that were deployed last year still read them. A schema change that ignores this breaks replay, projections, and the billing consumer at once.

This topic explains how to evolve event schemas without breaking anyone. Past events stay immutable, changes are additive, each event carries a version, older events are upcast on read, and every event envelope uses the CloudEvents `type` and `dataschema` attributes so that consumers can tell versions apart.

## Past Events Are Immutable

The event store is the permanent record, so an event is never rewritten. A bug in an old event stays in the store, and a fix in the code doesn't repair it. Learn offers two responses: a compensating event that records the correction, or an upcaster that interprets the old data during replay.

Learn lists in-place migration as the last resort, because rewriting history breaks the audit trail. Treat every schema change as a new version of an event, and keep the old version readable forever.

Old and new versions coexist in one stream, so every reader must handle both shapes.

## Additive Changes and Tolerant Readers

An additive change adds an optional field, and every existing consumer keeps working. Learn describes the tolerant reader approach for this case: consumers ignore fields they don't know, and they use defaults for fields that are missing. System.Text.Json ignores unknown properties by default, so a reader that deserializes the newer shape also reads the older shape.

```csharp
using System.Text.Json;

public sealed record ShipmentBookedV2(string shipmentId, string customerId, DateTimeOffset promisedBy, string? serviceLevel);

ShipmentBookedV2 booked = JsonSerializer.Deserialize<ShipmentBookedV2>(json, JsonSerializerOptions.Web)!;

string serviceLevel = booked.serviceLevel ?? "standard";
```

A missing `serviceLevel` deserializes as null, and the reader applies the default. The reader doesn't need a second class for the old shape when the new field is optional.

## Version Fields and Upcasting on Read

Some changes can't be additive, such as a field that changes meaning. The event envelope then carries a `schemaVersion`, and the reader upcasts each stored event to the newest shape on read.

An upcaster is a function from an old shape to the next shape. Learn notes that upcasters can chain, so each step handles one version change and the application code only sees the latest shape.

```csharp
using System.Text.Json;

public static class ShipmentBookedUpcaster
{
    public static ShipmentBookedV2 Upcast(int schemaVersion, JsonElement data)
    {
        ShipmentBookedV2 current = data.Deserialize<ShipmentBookedV2>(JsonSerializerOptions.Web)!;

        return schemaVersion switch
        {
            1 => current with { serviceLevel = "standard" },
            _ => current
        };
    }
}
```

Upcasting leaves the stored event untouched, so the history stays immutable. The cost is that every read runs the upcaster, so a long chain of versions slows replay. Learn notes that the stored events remain unchanged, which preserves immutability.

## CloudEvents Type and Dataschema

CloudEvents 1.0 gives every event a standard envelope. The required context attributes are `id`, `source`, `specversion`, and `type`. 

The optional attributes include `datacontenttype`, `dataschema`, `subject`, and `time`. The specification defines `type` as the value that describes the kind of event, and `dataschema` as the identifier of the schema that the data adheres to.

```json
{
  "specversion": "1.0",
  "id": "e7c1d9a2-4f3b-4c1e-9a7e-2b8f5d6c1a90",
  "source": "/contoso/logistics/shipping-api",
  "type": "com.contoso.logistics.shipment.booked",
  "subject": "shipments/SHP-10482",
  "time": "2026-10-18T08:00:00Z",
  "datacontenttype": "application/json",
  "dataschema": "https://schemas.contoso.example/shipment-booked/2.0.json",
  "data": {
    "shipmentId": "SHP-10482",
    "customerId": "CUST-2231",
    "promisedBy": "2026-10-22T17:00:00Z",
    "serviceLevel": "express"
  }
}
```

The `type` stays the same across versions, so a subscriber that handles every booking still matches.

The `dataschema` changes with each version, so a reader can select the right deserializer. A subscriber that filters on `type` alone receives every version and must check `dataschema`. The `source` and `id` pair identifies the event, and the idempotent consumer from the first topic uses that pair as its deduplication key.

## Consumer Tolerance Rules

Consumers and producers deploy at different times, so each change must be safe for the consumers that already run. The table lists the common changes and the rule that keeps each one safe.

| Change | Safe for old consumers | Rule |
| --- | --- | --- |
| Add an optional field | Yes | Consumers ignore unknown fields |
| Add a required field | No | Make it optional first, then require it in a later version |
| Rename a field | No | Write both names for one release, then remove the old name |
| Remove a field | No | Stop writing it, keep reading it, and remove it only after consumers move on |
| Change the meaning of a field | No | Publish a new event type or a new `dataschema` |

Check this table before every producer release. Consumers that already run keep working when every change falls in the safe rows, and the rest need a version bump.

A consumer that receives a `dataschema` it doesn't know should dead-letter the message. Module 4 covers the dead-letter queue that holds such messages for review.

## Key Topics

- [Event Sourcing pattern](https://learn.microsoft.com/azure/architecture/patterns/event-sourcing)
- [Idempotent Consumer pattern](https://learn.microsoft.com/azure/architecture/patterns/idempotent-consumer)
- [CloudEvents](https://cloudevents.io/)
- [CloudEvents core specification](https://github.com/cloudevents/spec/blob/main/cloudevents/spec.md)
