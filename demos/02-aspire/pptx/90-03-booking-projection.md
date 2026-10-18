---
slide: "90-03"
topic: 02-aspire
title: "Projecting the booking in tracking-service"
subtitle: "Step 5 writes the tracking document before it completes the message"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/02-aspire/_images/shipment-booked-processor.svg"
media-file: labs/02-aspire/_images/shipment-booked-processor.svg
visual-prompt: >
  Existing diagram of ShipmentBookedProcessor taking ShipmentBooked from the tracking subscription, upserting the tracking document in Cosmos DB and only then completing the message, while GET /tracking/{trackingNumber} reads the same document, no generation. Referenced at its repo path.
order: 19
source: labs/02-aspire/readme.md
gamma_prompt: >
  Slide titled "Projecting the booking in tracking-service", subtitle "Step 5 writes the tracking
  document before it completes the message". Layout: Two columns. Left column: the bullets. Right
  column, wider than the left: the provided diagram as an inline content image at its natural 16:9
  aspect ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Projecting the booking in tracking-service

## Content

- `ShipmentBookedProcessor` reads `ShipmentBooked` from the `tracking` subscription
- It upserts the tracking document in Cosmos DB, then **completes** the message
- `GET /tracking/{trackingNumber}` returns 200 with the document, or 404
- The `BookingReachesTracking` **gate** is written before the code and passes after it

## Speaker Notes

Step 5 is where the booking from step 4 reaches the read model, and it is the one step of the lab that carries a gate.

The processor: it is a `BackgroundService` with a `ServiceBusProcessor` on the `shipment-events` topic and the `tracking` subscription, so its traffic stays separate from the other consumers.

Order of work: with `AutoCompleteMessages` set to false, it deserializes `ShipmentBooked`, upserts the document inside a `ProjectShipmentBooked` activity and completes the message only after that write.

The lookup: before a booking the endpoint returns 404, and after the booking from step 4 it returns 200 with the tracking number `CL-2026-000142` in the JSON body.

The gate: `BookingReachesTracking` starts the AppHost, posts a booking and polls tracking-service every two seconds for up to 60 seconds. It fails while the tracking endpoint is missing and passes once the code exists.
