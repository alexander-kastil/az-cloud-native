---
slide: "04-01"
topic: 04-event-types
title: "Domain events and integration events"
subtitle: "The same fact inside one service or across a broker"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/01-distributed-design/_images/event-kind-style.svg"
media-file: labs/01-distributed-design/_images/event-kind-style.svg
visual-prompt: >
  Existing diagram of event kind by style, with ProofOfDeliveryUploaded as a domain notification, ParcelScanned as an integration notification and ParcelDelivered as an integration state transfer, each with its key fields, no generation. Referenced at its repo path.
order: 10
source: demos/01-distributed-design/04-event-types/readme.md
gamma_prompt: >
  Slide titled "Domain events and integration events", subtitle "The same fact inside one service or
  across a broker". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Domain events and integration events

## Content

- A **domain event** is raised and handled inside one bounded context
- An **integration event** crosses a service boundary through a broker
- Integration events leave only after the entity is persisted
- The publisher owns the contract; the labs keep it in `Grey.Contracts`
- Every event also has a style: notification or state transfer

## Speaker Notes

When Booking confirms a shipment, the same fact travels in different forms. The grid sorts events by kind, inside one service or across a broker, and by style, which the next slide covers.

Domain event: the Shipment aggregate raises ShipmentBooked, and a handler in the booking module updates the customer account in the same process. The eShop reference application sends its domain events to an in-process dispatcher in the same way.

Integration event: it travels through a broker to other services, such as Tracking and the exception agent.

After the commit: the .NET architecture guidance says integration events occur only after the entity is persisted, because otherwise the operation never happened for the consumers. ShipmentBooked leaves Booking only after the Shipment commits.

Contracts: integration event contracts belong to the publishing service. The labs keep every record in one `Grey.Contracts` project, so a record change reaches every consumer at once.

Style: in the grid, ProofOfDeliveryUploaded is a domain notification, ParcelScanned an integration notification and ParcelDelivered an integration state transfer.
