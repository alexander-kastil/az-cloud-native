---
slide: "90-02"
topic: 03-hosting
title: "Following one booking with KQL"
subtitle: "One query returns every row of the booking"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/03-hosting/_images/booking-trace.svg"
media-file: labs/03-hosting/_images/booking-trace.svg
visual-prompt: >
  Existing diagram of one POST /shipments through shipping-api, the shipment-events topic and tracking-service, and the booking-trace.kql result rows that share one OperationId, no generation. Referenced at its repo path.
order: 15
source: labs/03-hosting/readme.md
gamma_prompt: >
  Slide titled "Following one booking with KQL", subtitle "One query returns every row of the booking".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural wide aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Following one booking with KQL

## Content

- `POST /shipments` with tracking number `CL-2026-000142` returns 201 Created
- shipping-api writes to `shipments` and sends `ShipmentBooked` to `shipment-events`
- `ShipmentBookedProcessor` in tracking-service writes the booking to `tracking`
- `booking-trace.kql` takes the **OperationId** of the latest shipping-api POST
- It unions `AppRequests`, `AppDependencies` and `AppTraces` in time order

## Speaker Notes

The last lab step proves that the telemetry from step 4 works: one query follows one booking across both services.

The booking: send the JSON booking with tracking number `CL-2026-000142` to `https://<SHIPPING_API_FQDN>/shipments` with any HTTP client, and the response is 201 Created.

Shipping side: `POST /shipments` writes the shipment to the Cosmos DB container `shipments` and sends a `ShipmentBooked` message to the `shipment-events` topic.

Tracking side: `ShipmentBookedProcessor` handles the message from the tracking subscription and writes to the `tracking` container. The Service Bus SDK carries the `OperationId` across that hop.

Finding the operation: the query takes the most recent POST from the shipping-api role in the last 30 minutes. `AppRoleName` carries the environment name as a prefix, so it matches with `endswith`.

The union: it returns every `AppRequests`, `AppDependencies` and `AppTraces` row with that `OperationId` in time order, and projects `Message` as `Name` because `AppTraces` has no `Name` column. Give Application Insights a few minutes to ingest the telemetry before you run it in Logs.
