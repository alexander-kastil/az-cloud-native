---
slide: "90-01"
topic: 06-event-sourcing-cqrs
title: "Lab: event-source shipments with an outbox"
subtitle: "From two separate calls to an event store, an outbox and a cached read model"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/06-event-sourcing-cqrs/_images/end-state.svg"
media-file: labs/06-event-sourcing-cqrs/_images/end-state.svg
visual-prompt: >
  Existing diagram of the lab at the start, shipping-api saving the shipment and sending ShipmentBooked in two separate calls, and at the end, with the event store, the booking batch, outbox-relay, tracking-projection, the indexed tracking container, the Redis cache and both endpoints each labelled with the step that adds them, no generation. Referenced at its repo path.
order: 17
source: labs/06-event-sourcing-cqrs/readme.md
gamma_prompt: >
  Slide titled "Lab: event-source shipments with an outbox", subtitle "From two separate calls to an event
  store, an outbox and a cached read model". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the one
  image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Lab: event-source shipments with an outbox

## Content

- Steps 1 and 2: an event store that rejects a second event per version, booking and outbox in one batch
- Step 3: `outbox-relay` publishes the outbox documents to `shipment-events`
- Steps 4 and 5: `tracking-projection` fills the indexed `tracking` container
- Step 6: Redis caches the tracking page and drops it after each scan
- Step 7: `BookShipment` answers 202 Accepted, `GET /tracking/{trackingNumber}` reads the model

## Speaker Notes

In about 60 minutes the lab takes Grey Logistics from the start state at the top of the diagram to the end state below it. You open grey-logistics with an AI coding agent, and Docker Desktop runs the Cosmos DB and Redis emulators from the AppHost.

Event store and batch: ShipmentEventStore creates each event with the ID {shipmentId}-v{version:D6} and returns false on 409 Conflict. ShipmentOutboxWriter saves the ShipmentBooked event and its outbox document in one transactional batch.

Relay: outbox-relay is a change feed processor on shipments with the leases container and a stable instance name. It sends each outbox document with MessageId, SessionId and Subject set, and the topic's duplicate detection drops the copies a retry creates.

Projection and index: tracking-projection upserts a ScanRecord for each ParcelScanned event and starts from the beginning of the container. The tracking container gets a consistent indexing policy with a composite index on scanType and occurredAt.

Cache: the AppHost adds a Redis container named cache, and TrackingCache stores the page for five minutes plus up to 30 seconds. The projection deletes the key after each upsert, never before.

Command and query: BookShipmentHandler returns only a success flag, which the endpoint maps to 202 Accepted with the shipment ID and tracking number. The tracking endpoint returns TrackingCache.GetScansAsync, and the two services share no project reference.
