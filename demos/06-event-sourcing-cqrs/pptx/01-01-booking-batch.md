---
slide: "01-01"
topic: 01-outbox
title: "Transactional batch for the booking"
subtitle: "The event and its outbox document commit together or not at all"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/06-event-sourcing-cqrs/_images/booking-batch.svg"
media-file: labs/06-event-sourcing-cqrs/_images/booking-batch.svg
visual-prompt: >
  Existing diagram of SaveBookingAsync writing the ShipmentBooked event and its outbox document in one transactional batch on partition SHP-10482, with the commit and 409 Conflict outcomes, no generation. Referenced at its repo path.
order: 3
source: demos/06-event-sourcing-cqrs/01-outbox/readme.md
gamma_prompt: >
  Slide titled "Transactional batch for the booking", subtitle "The event and its outbox document commit
  together or not at all". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Transactional batch for the booking

## Content

- A **dual write** to Cosmos DB and Service Bus can lose the event or announce a missing shipment
- The ShipmentBooked event and its outbox event share the partition key `/shipmentId`
- One **transactional batch**: one logical partition, up to 100 operations, at most 2 MB
- Check `IsSuccessStatusCode` before the caller hears success
- The outbox event carries `ttl` 864000 (ten days), the event document never expires

## Speaker Notes

Booking a shipment has to store the shipment and get ShipmentBooked to the subscribers of shipment-events. This slide shows how the outbox turns those two writes into one.

Dual write: the database and the message broker don't share a transaction, so one write can succeed while the other fails. Save then publish can lose the event, and publish then save announces a shipment that doesn't exist.

Shared partition: the shipments container uses /shipmentId as its partition key. The ShipmentBooked event document and its outbox event therefore sit in one logical partition and can commit together.

One batch: a Cosmos DB transaction is a transactional batch on one logical partition, with up to 100 operations and a request of at most 2 MB. SaveBookingAsync adds both documents with CreateItem and runs ExecuteAsync.

One status: the batch returns one status for the whole unit, and when one operation fails, for example with 409 Conflict, every other operation reports 424. In the lab, POST /shipments answers 409 Conflict when the batch fails.

TTL: the outbox event carries a ttl of 864000 seconds, ten days, so every consumer has time to recover from an outage first. Event documents leave out ttl, so the history never expires.
