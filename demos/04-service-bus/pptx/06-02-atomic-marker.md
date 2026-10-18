---
slide: "06-02"
topic: 06-idempotent-consumers
title: "Marker and scan in one transactional batch"
subtitle: "Cosmos DB commits both writes or neither"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/06-idempotent-consumers/_images/crash-points.svg"
media-file: demos/04-service-bus/06-idempotent-consumers/_images/crash-points.svg
visual-prompt: >
  Existing diagram of three write orders: marker first loses the scan on a crash, scan first repeats it, and one transactional batch that creates the marker and patches the scan into parcel-{trackingNumber} leaves no gap, no generation. Referenced at its repo path.
order: 14
source: demos/04-service-bus/06-idempotent-consumers/readme.md
gamma_prompt: >
  Slide titled "Marker and scan in one transactional batch", subtitle "Cosmos DB commits both writes or
  neither". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Marker and scan in one transactional batch

## Content

- The **Idempotent Consumer** pattern records each processed key in a store that enforces uniqueness
- Cosmos DB answers a repeated item ID with a `CosmosException` whose status is `Conflict` (409)
- One transactional batch creates `processed-{messageId}` and patches `parcel-{trackingNumber}`
- Marker and parcel share the `/trackingNumber` partition, up to 100 operations and 2 MB
- A 409 on the marker completes the message as a duplicate; any other failure retries

## Speaker Notes

Duplicate detection stops only repeated sends, so the consumer needs its own protection. Contoso Logistics stores a processed-message marker next to the parcel document.

Idempotent Consumer: the consumer treats a write that conflicts with an existing key as a duplicate. The store must enforce uniqueness itself, because two consumer instances can check the same key at the same moment.

409 Conflict: Cosmos DB enforces unique item IDs within a partition, and a create that reuses an ID fails with HTTP 409. The .NET SDK throws a `CosmosException` with `HttpStatusCode.Conflict`, which the handler treats as a duplicate.

One batch: two separate writes leave a window, where a crash after the side effect repeats the scan and a crash after the marker loses it. A transactional batch commits the marker and the scan together, or neither.

Same partition: a batch covers items that share a partition key, so the marker sits in the `tracking` container on the parcel's `/trackingNumber`. One batch allows up to 100 operations and 2 MB.

Outcome: on a repeated marker the batch fails with 409, the scan operation returns 424, and `TryAppendAsync` returns false, so the consumer completes the message. Any other failure throws, Service Bus abandons the message, and it is retried.
