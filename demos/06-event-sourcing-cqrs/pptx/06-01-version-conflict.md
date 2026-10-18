---
slide: "06-01"
topic: 06-event-store
title: "Appending events with a version check"
subtitle: "The event ID carries the version, so a second append gets 409 Conflict"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/06-event-sourcing-cqrs/_images/version-conflict.svg"
media-file: labs/06-event-sourcing-cqrs/_images/version-conflict.svg
visual-prompt: >
  Existing diagram of two writers appending version 2 of SHP-10482 with the same ID SHP-10482-v000002: the first create is stored and AppendAsync returns true, the second gets 409 Conflict and returns false, no generation. Referenced at its repo path.
order: 13
source: demos/06-event-sourcing-cqrs/06-event-store/readme.md
gamma_prompt: >
  Slide titled "Appending events with a version check", subtitle "The event ID carries the version, so a
  second append gets 409 Conflict". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled
  to fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Appending events with a version check

## Content

- One immutable event document per change, partition key `/shipmentId`, no `ttl`
- A correction is a new event: `PaymentRefunded` follows `PaymentCaptured`
- The ID `{shipmentId}-v{version:D6}` makes a second append for the same version collide
- `CreateItemAsync` answers **409 Conflict** and `AppendAsync` returns false
- `IfMatchEtag` protects documents that get replaced; append-only events don't need it

## Speaker Notes

The Shipment has no row that holds its current state: each change is an immutable event such as ShipmentBooked, CapacityReserved or LabelPrinted. The event store is the write side, so it must reject two conflicting writes to the same shipment.

Event documents: each one holds the shipment ID, a version that counts events per shipment, the event type, the time and the data of the change. All events of one shipment live in one logical partition, and event documents never carry a ttl.

Corrections: Learn advises events that record business intent, and a mistaken PaymentCaptured is never edited. The PaymentRefunded event that follows records the reversal, and both events stay in the store.

Version in the ID: two dispatchers can both read version 6 and both try to append version 7. The ID encodes the shipment and the version, so the second create collides with the first.

409 Conflict: the store returns false and doesn't retry, and POST /shipments answers 409 Conflict so the caller decides what to do next. In the lab diagram CapacityReserved is stored as version 2, and LabelPrinted is not stored.

ETags: a replace that sends the last ETag it read with IfMatchEtag succeeds only while the stored item still has it, and a stale replace returns 412 Precondition Failed. Use it for a document that a process rewrites, such as a current-state document; append-only events don't need it.
