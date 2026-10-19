---
slide: "03-02"
topic: 03-event-storming
title: "From contexts to the Grey service list"
subtitle: "A microservice spans at most one bounded context"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/03-event-storming/_images/storm-to-services.svg"
media-file: demos/01-distributed-design/03-event-storming/_images/storm-to-services.svg
visual-prompt: >
  Existing diagram of an event storming wall in four stages: events and a hotspot on a timeline, the booking flow in which BookShipment issues ReserveCapacity and the BookingSagaOrchestrator policy issues CapturePayment, the Booking, Routing and Billing contexts, and their services shipping-api, routing-service and billing-service, no generation. Referenced at its repo path.
order: 9
source: demos/01-distributed-design/03-event-storming/readme.md
gamma_prompt: >
  Slide titled "From contexts to the Grey service list", subtitle "A microservice spans at most one
  bounded context". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# From contexts to the Grey service list

## Content

- In the **booking flow**, BookShipment invokes ReserveCapacity and the saga issues CapturePayment
- Aggregates that share one language form a **bounded context**
- A **context map** names the relationship between two contexts
- Eight Grey contexts, each with its own service, such as Booking on shipping-api
- Each candidate passes the boundary tests or goes back to the storm

## Speaker Notes

Once the wall is full, clusters of aggregates turn into contexts and then into services. The diagram shows that path for Booking, Routing and Billing.

The booking flow: a customer issues BookShipment, the Shipment aggregate emits ShipmentBooked, and the booking request itself invokes ReserveCapacity on routing. The booking saga issues CapturePayment, and each step names one aggregate that decides and becomes the unit of ownership.

Bounded contexts: the team draws a context around each cluster of aggregates that share one language. The Azure Architecture Center states that a microservice shouldn't span more than one bounded context.

Context map: between contexts, the map names the relationship, such as customer-supplier, open host service or anti-corruption layer.

The Grey list: Booking on shipping-api owns the Shipment aggregate and its event store, Routing runs one Dapr actor per van, and Billing owns payments. Labels, Tracking, Notifications, Telemetry and Exceptions complete the list, and the ops console reads from these contexts.

Boundary tests: one responsibility, no chatty calls, a team that can own it, deployment without its neighbours and loose coupling. A candidate that fails a test goes back to the storm for another round.
