---
slide: "06-01"
topic: 06-pick-the-broker
title: "Messages, events and streams"
subtitle: "Service Bus, Event Grid, Event Hubs and Dapr pub/sub compared"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/06-01-broker-comparison.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, text #0F172A, secondary text #64748B, cell borders
  #94A3B8 2px, minimum 26px type. No heading inside the image. A comparison grid with four coloured
  column headers, each a broker name with its shape underneath: Service Bus "messages" (blue #DBEAFE),
  Event Grid "events" (amber #FEF3C7), Event Hubs "stream" (green #DCFCE7), Dapr pub/sub "portability
  layer" (purple #EDE9FE). Three rows labelled on the left with the cell text verbatim from the topic
  readme table. Ordering: "FIFO within a session", "No guarantee", "Per partition",
  "Component-dependent". Retention: "Until the message time to live expires", "Custom topic TTL of 1 to
  1440 minutes; namespace topic up to 7 days", "1 to 90 days, depending on tier",
  "Component-dependent". Dead-letter: "Dead-letter sub-queue", "Storage account, when configured", "None
  built in", "Component-dependent".
order: 16
source: demos/01-distributed-design/06-pick-the-broker/readme.md
gamma_prompt: >
  Slide titled "Messages, events and streams", subtitle "Service Bus, Event Grid, Event Hubs and Dapr
  pub/sub compared". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Messages, events and streams

## Content

- A **message** carries data under a contract, the shape Service Bus fits
- An **event** is a lightweight notification of a state change, for Event Grid
- A **stream** is a time-ordered series each consumer reads at its own pace, for Event Hubs
- **Dapr pub/sub** is a portability layer that sits on top of brokers
- Ordering, retention and dead-lettering decide most designs

## Speaker Notes

Contoso carries four kinds of traffic, and a wrong broker choice costs ordering, retention or replay later. This slide uses the terms Microsoft Learn uses and compares the rows that decide most designs.

Messages: the publisher expects a consumer to act on the message. Service Bus offers transactions, sessions, duplicate detection and dead-lettering for high-value payloads in enterprise workflows.

Events: the publisher expects no particular reaction. Event Grid routes discrete events to subscribers, filters them and retries delivery on a schedule.

Streams: Event Hubs keeps events in partitions for a configured retention period, and each consumer group keeps its own position. Telemetry, log aggregation and real-time analytics are its main scenarios.

Dapr pub/sub: services publish to the Dapr API, and a component decides which broker carries the message. The Dapr component index lists Service Bus topics, Service Bus queues and Event Hubs as Azure components, and no Event Grid component.

The deciding rows: read the grid by column to choose a service and by row to compare two. A missing feature in ordering, retention or dead-lettering is expensive to build around later.
