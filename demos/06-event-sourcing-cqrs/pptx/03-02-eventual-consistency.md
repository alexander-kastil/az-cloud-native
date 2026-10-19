---
slide: "03-02"
topic: 03-cqrs
title: "Eventual consistency in the ops console"
subtitle: "202 Accepted first, the first scan once the projection has run"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/03-02-eventual-consistency.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif, module palette (client grey #F1F5F9 with #64748B, services blue #DBEAFE with #1D4ED8,
  results green #15803D), minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. Three tall panels left to right joined by arrows at header height, each holding three stacked
  boxes joined by short down arrows. Panel "the write commits": ops console "BookShipment to
  shipping-api", shipping-api "saves the event document and its outbox event", shipping-api "202
  Accepted with a status link" in green. Dashed grey panel "change feed delay": ops console "shipment
  shown as received", arrow "polls", tracking-service "query by trackingNumber, no ScanRecord yet", ops
  console "empty list, keep polling". Green-bordered panel "after the change feed delivers": projection
  "first ScanRecord upserted into the read model", tracking-service "query by trackingNumber returns the
  first ScanRecord", ops console "first scan appears on the next poll" in green.
order: 9
source: demos/06-event-sourcing-cqrs/03-cqrs/readme.md
gamma_prompt: >
  Slide titled "Eventual consistency in the ops console", subtitle "202 Accepted first, the first scan once
  the projection has run". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Eventual consistency in the ops console

## Content

- The projection runs after the write commits, so the read model can lag
- The booking endpoint answers **202 Accepted** with a status link
- The console shows the shipment as received and polls the tracking query
- An empty list means keep polling until the first scan appears
- **Session consistency** can close the gap for the same user

## Speaker Notes

Splitting writes from reads has a cost: the tracking container can lag the event store by the change feed delay. The UI has to show that state instead of assuming the window is zero.

Lag: the projection runs after the write commits, so a customer who books and opens the tracking page at once might see nothing yet. The window is usually short, but the UI must not assume it is zero.

202 Accepted: the booking endpoint returns the accepted booking, usually with HTTP 202 and a status link. That response makes the in-between state explicit.

Received and polling: the console shows the shipment as received and polls the tracking query. While the change feed delay lasts, the query finds no ScanRecord and returns an empty list.

First scan: once the change feed delivers the event, the projection upserts the first ScanRecord and the next poll shows it. The Architecture Center describes the same trade-off and warns that a user can act on stale data.

Session consistency: for the same user, session consistency can close the gap. The next topic shows how to pass the session token between requests.
