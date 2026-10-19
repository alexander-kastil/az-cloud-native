---
slide: "03-01"
topic: 03-cqrs
title: "Command side and query side"
subtitle: "shipping-api writes the aggregate, tracking-service reads its own model"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/06-event-sourcing-cqrs/_images/command-query-split.svg"
media-file: labs/06-event-sourcing-cqrs/_images/command-query-split.svg
visual-prompt: >
  Existing diagram of the BookShipment command in shipping-api and the GET /tracking/{trackingNumber} query in tracking-service with no project reference between them, meeting only through the shipments change feed and tracking-projection, no generation. Referenced at its repo path.
order: 8
source: demos/06-event-sourcing-cqrs/03-cqrs/readme.md
gamma_prompt: >
  Slide titled "Command side and query side", subtitle "shipping-api writes the aggregate, tracking-service
  reads its own model". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Command side and query side

## Content

- A **command** names a business task such as `BookShipment` and returns only a success flag
- A **query** never changes state and returns a view shaped for one screen
- The event store partitions by `/shipmentId`, the read model by `/trackingNumber`
- The read model can be rebuilt from the event store at any time
- Plain handler classes registered in DI, no mediator library

## Speaker Notes

Shipments are written by customers and dispatchers and read by the tracking page, the ops console and notification emails. This topic separates the two sides so that each can change for its own reasons.

Command side: BookShipmentHandler builds the ShipmentBooked event document and its outbox document and saves both through the outbox writer from the first topic. It returns only a success flag, which POST /shipments turns into 202 Accepted.

Query side: TrackingQueryHandler reads only the tracking read model, with a query scoped to the tracking number. It knows nothing about the aggregate and never calls shipping-api.

Separate containers: each side picks its own partition key, because every write touches one shipment and every tracking page asks for one parcel. The two sides meet only through the change feed and tracking-projection, with no project reference between the services.

Rebuildable read model: the event store stays the source of truth, so the read model can be rebuilt at any time. A second read model, such as an ops search index, can subscribe to the same events without any change to the write side.

No mediator: the handlers are plain classes that the DI container registers, so a unit test can pass in a fake outbox writer or container. MediatR 13 and later is commercially licensed, so the module doesn't depend on it.
