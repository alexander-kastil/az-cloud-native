---
slide: "04-01"
topic: 04-pub-sub
title: "Publishing and subscribing"
subtitle: "The code names the component pubsub and the topic parcel-events"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/07-dapr/04-pub-sub/_images/pubsub-delivery.svg"
media-file: demos/07-dapr/04-pub-sub/_images/pubsub-delivery.svg
visual-prompt: >
  Existing diagram of a ParcelScanned event wrapped in a CloudEvents 1.0 envelope by the tracking-service sidecar and delivered through the parcel-events topic to notification-service, no generation. Referenced at its repo path.
order: 9
source: demos/07-dapr/04-pub-sub/readme.md
gamma_prompt: >
  Slide titled "Publishing and subscribing", subtitle "The code names the component pubsub and the
  topic parcel-events". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Publishing and subscribing

## Content

- `PublishEventAsync("pubsub", "parcel-events", scan)` names no broker
- `[Topic("pubsub", "parcel-events")]` or `WithTopic` declares the subscription
- `MapSubscribeHandler` lists the subscriptions, `UseCloudEvents` unwraps the **CloudEvents** envelope
- Swap the broker in the component file: Service Bus, Event Hubs, Redis or Kafka
- Use the native Service Bus SDK when the logic depends on session state

## Speaker Notes

Every scan at a hub produces a `ParcelScanned` event that `tracking-service` and `notification-service` both need. The scanner should not know who listens or wait for them, and the code should not be tied to one broker.

Publish: the publisher passes the component name, the topic and the payload, so the code holds no connection string and no broker type. An overload takes publish metadata, such as the `SessionId` the Service Bus component reads.

Subscribe: the `Topic` attribute on a controller action, or `WithTopic` on a minimal API endpoint, takes the component name and the topic. The sidecar posts each matching message to that action.

Startup and envelope: the sidecar calls the endpoint from `MapSubscribeHandler` at startup to read the subscriptions. Dapr wraps each payload in a CloudEvents 1.0 envelope whose id, source and type support tracing and deduplication.

Broker swap: replace the component file and keep the code. Service Bus topics and Event Hubs are Tier 1 on Container Apps, Redis Streams and Kafka are Tier 2, and a local run can use the Redis container that `dapr init` starts.

What Dapr hides: the component renews message locks and exposes settings such as `maxConcurrentHandlers`. Session state and the session receiver are not documented for Dapr, so use the native SDK when the processing depends on them.
