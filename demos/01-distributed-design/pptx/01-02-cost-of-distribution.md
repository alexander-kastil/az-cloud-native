---
slide: "01-02"
topic: 01-monolith-vs-microservices
title: "The cost of distribution"
subtitle: "Each arrow between Grey services is a call that can fail"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/01-02-grey-boundaries.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, services blue #DBEAFE with #1D4ED8 borders, external
  sources grey #F1F5F9 with #64748B borders, text #0F172A, service names #64748B, arrows #334155 3px,
  minimum 26px type. No heading inside the image. No arrow crosses a box or a label. The Grey
  boundaries from the topic readme flowchart, three lanes read left to right. Top: Booking
  (shipping-api) fans out through one junction to Routing (routing-service) with "ReserveCapacity",
  Billing (billing-service) with "CapturePayment" and Labels (label-service) with "PrintLabel". Middle:
  Parcel scanners to Tracking (tracking-service) with "ParcelScanned", Tracking to Notifications
  (notification-service) with "ParcelScanned". Bottom: Van telematics to Telemetry (telemetry-ingest)
  with "GPS positions", Telemetry to Ops console (ops-console) with "SignalR".
order: 4
source: demos/01-distributed-design/01-monolith-vs-microservices/readme.md
gamma_prompt: >
  Slide titled "The cost of distribution", subtitle "Each arrow between Grey services is a call that
  can fail". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The cost of distribution

## Content

- With **eventual consistency**, a booking can be confirmed before its van is assigned
- Booking calls routing and billing, and each **network call** can fail on its own
- Every service needs its own pipeline, dashboards and rollback path
- A new field on **ShipmentBooked** must not break consumers of the old shape
- Draw each boundary where the business language changes

## Speaker Notes

Every split adds costs that a single process never pays. The diagram shows the Grey boundaries, and each arrow is a place where a boundary costs something.

Eventual consistency: a booking that captures a payment and then fails to reserve a van cannot roll back as one ACID transaction across two services. The Azure guidance for that case is to embrace eventual consistency where possible.

Network calls: Booking calls routing and billing, and each call can fail on its own. Latency also adds up along every call chain.

Operations: every service needs its own pipeline, dashboards and rollback path.

Versioning: a new field on ShipmentBooked must not break the consumers that still read the old shape.

Business language: Booking changes when booking rules change and Routing when dispatch rules change, so that is where the boundary sits. Every boundary also passes a test: the service does one job, avoids chatty calls and deploys without its neighbours.
