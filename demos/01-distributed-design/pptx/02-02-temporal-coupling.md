---
slide: "02-02"
topic: 02-sync-async
title: "Request/reply and fire-and-forget"
subtitle: "Both styles appear in one booking"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/02-sync-async/_images/temporal-decoupling.svg"
media-file: demos/01-distributed-design/02-sync-async/_images/temporal-decoupling.svg
visual-prompt: >
  Existing diagram of one booking on a time axis, where shipping-api waits for routing-service to answer ReserveCapacity before it returns 202 Accepted, and ShipmentBooked waits in the message broker until tracking-service is running again, no generation. Referenced at its repo path.
order: 6
source: demos/01-distributed-design/02-sync-async/readme.md
gamma_prompt: >
  Slide titled "Request/reply and fire-and-forget", subtitle "Both styles appear in one booking".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Request/reply and fire-and-forget

## Content

- With **request/reply** the caller waits, and HTTP is the default transport
- With **fire-and-forget** the broker stores the message until a consumer is ready
- Booking waits for ReserveCapacity, while ShipmentBooked waits in the broker
- **Temporal coupling** means both services must be up at the same moment
- Set each request/reply **timeout** from how long the customer will wait

## Speaker Notes

A customer who books a shipment expects the answer in the same request, while a parcel scanner sends updates that nobody waits on. The time axis puts both styles into one booking.

Request/reply: the BookShipment call waits until shipping-api validates the request and returns the shipment ID. The caller holds the connection while that happens.

Fire-and-forget: the sender publishes and continues, and a crashed consumer does not lose the message. The producer stays unblocked unless it loses its connection to the broker.

One booking, both styles: the reservation is a synchronous call that Booking waits for. ShipmentBooked travels through the broker and reaches tracking-service once it has restarted.

Temporal coupling: with HTTP, if routing is down while a customer books, the booking fails unless you add a retry or a queue. A broker removes that dependency for commands and events.

Timeouts: set the timeout from how long the customer will wait, so the app shows an error before the customer gives up. Timeouts also release threads and connections that would otherwise pile up during an outage.
