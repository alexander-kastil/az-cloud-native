---
slide: "00-02"
topic: 05-event-grid-hubs
title: "Overview"
subtitle: "The path from ParcelDelivered to the live map"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/05-event-grid-hubs/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG in the module's diagram style, viewBox 0 0 1920 1080, slate #F8FAFC canvas
  with one white panel bordered #94A3B8 2px, 'Segoe UI', Inter, Arial, sans-serif, headings #0F172A,
  secondary text #64748B, minimum 26px type, arrows #334155. No heading inside the image; the slide
  title carries it. No arrow crosses a box or a label. Two rows of three numbered rounded cards, read
  left to right, each card a two-digit number, the product in secondary text, the topic title in bold
  and one secondary line. Top row amber #FEF3C7 with #B45309 border, caption "Discrete events" with
  "Event Grid, for partners and functions": 01 Event Grid "Custom Topics & Subscriptions" "publish
  ParcelDelivered as a CloudEvent", 02 Event Grid "Filters, Retries, Dead-Letter" "keep the events that
  Event Grid gives up on", 03 Event Grid Namespaces "Pull Delivery" "partners pull ParcelDelivered when
  they are ready". Bottom row blue #DBEAFE with #1D4ED8 border, caption "Telemetry" with "Event Hubs and
  SignalR, from the vans to the ops console": 04 Event Hubs "Partitions & Checkpoints" "van positions in
  order, resume from the checkpoint", 05 Event Hubs "Kafka-Compatible Streaming" "telematics vendors keep
  their Kafka clients", 06 SignalR Service "Push to Browsers" "van positions on the ops console map".
  Short arrows between neighbours in each row.
order: 2
source: demos/05-event-grid-hubs/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path from ParcelDelivered to the live map". Layout: Two columns.
  Left column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Publish `ParcelDelivered` to a **custom topic** and validate each partner endpoint
- Filter and retry deliveries, and **dead-letter** the ones that fail
- Let partners **pull** from a namespace topic when they are ready
- Stream van positions through **partitions**, checkpoints and the Kafka endpoint
- Push each position to the ops console with **SignalR Service**

## Speaker Notes

This slide walks the path through the six topics once, following on from the three themes of the title slide. Every topic uses the same Contoso Logistics events, so the pieces fit together by the end.

Custom topics: the label-service publishes `ParcelDelivered` as a CloudEvent, and Event Grid validates each partner endpoint before it sends the first event.

Filters, retries and dead-letter: filters decide which events reach a subscription, the retry policy decides how long Event Grid keeps trying, and dead-lettering keeps the events it gives up on.

Pull delivery: partners that can't open an inbound port pull from a namespace topic, with queue-like semantics and explicit acknowledgment.

Partitions and Kafka: van positions keyed by van ID stay in order in Event Hubs, consumers track their progress with checkpoints, and telematics vendors produce over the Kafka endpoint.

SignalR: the telemetry-ingest service pushes each position through Azure SignalR Service to the live map in the ops console.
