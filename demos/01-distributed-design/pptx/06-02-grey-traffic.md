---
slide: "06-02"
topic: 06-pick-the-broker
title: "Grey traffic on each service"
subtitle: "Three Azure brokers side by side, Dapr pub/sub on the notification path"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/06-pick-the-broker/_images/grey-traffic.svg"
media-file: demos/01-distributed-design/06-pick-the-broker/_images/grey-traffic.svg
visual-prompt: >
  Existing diagram of the Grey traffic in four lanes: ParcelScanned in one session on the Service Bus topic parcel-events with a dead-letter queue, VanPositionReported in an Event Hubs partition keyed by van ID, ParcelDelivered pushed by Event Grid to three partners, and DeliveryDelayed through Dapr pub/sub to notification-service, no generation. Referenced at its repo path.
order: 17
source: demos/01-distributed-design/06-pick-the-broker/readme.md
gamma_prompt: >
  Slide titled "Grey traffic on each service", subtitle "Three Azure brokers side by side, Dapr
  pub/sub on the notification path". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Grey traffic on each service

## Content

- **Service Bus** carries the booking commands and ParcelScanned, one session per tracking number
- **Event Hubs** carries VanPositionReported over Kafka, keyed by van ID
- **Event Grid** sends ParcelDelivered to partners after the driver's photo upload
- **Dapr pub/sub** carries DeliveryDelayed to notification-service
- Check ordering first, then retention

## Speaker Notes

Each lane shows one Grey flow on the service that fits its shape. Together they explain why the system runs three Azure brokers side by side.

Service Bus: ReserveCapacity and PrintLabel travel as messages, and each parcel scan is a ParcelScanned message with the tracking number as its session ID, so scans of one parcel run in order. An unreadable label scan that keeps failing ends in the dead-letter queue, where an operator can inspect it.

Event Hubs: the van simulator publishes VanPositionReported over the Kafka endpoint with the van ID as the message key, which keeps each van's positions in order. telemetry-ingest reads the stream with EventProcessorClient and pushes each position to the ops console map through SignalR.

Event Grid: a driver's photo uploaded to Blob Storage raises a system event, and partners receive ParcelDelivered through a custom topic or by pulling from a namespace. Event Grid suits this shape because the publisher is an Azure resource and the consumers are reactive.

Dapr pub/sub: the publishing service calls the Dapr API with DeliveryDelayed, and notification-service subscribes without a dependency on a broker SDK. Swapping Service Bus topics for Event Hubs then becomes a component change.

Order of checks: ordering is the hardest property to add later, and replay depends on retention.
