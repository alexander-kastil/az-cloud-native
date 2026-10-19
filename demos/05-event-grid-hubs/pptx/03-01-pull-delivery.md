---
slide: "03-01"
topic: 03-namespaces-pull
title: "Pull delivery with Event Grid namespaces"
subtitle: "Partners without an inbound port call out when they are ready"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/05-event-grid-hubs/03-namespaces-pull/_images/push-vs-pull.svg"
media-file: demos/05-event-grid-hubs/03-namespaces-pull/_images/push-vs-pull.svg
visual-prompt: >
  Existing diagram of push delivery into the partner-delivered storage queue next to pull delivery, where the partner application calls out to the Event Grid namespace with ReceiveAsync and AcknowledgeAsync, no generation. Referenced at its repo path.
order: 8
source: demos/05-event-grid-hubs/03-namespaces-pull/readme.md
gamma_prompt: >
  Slide titled "Pull delivery with Event Grid namespaces", subtitle "Partners without an inbound port
  call out when they are ready". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Pull delivery with Event Grid namespaces

## Content

- A **namespace topic** receives CloudEvents 1.0; each subscription picks `queue` or `push`
- With pull, the partner calls out, so its network needs no inbound port
- Each partner gets its own subscription and its own state for every event
- Limits: 7 days retention, 500 subscriptions per topic, 1 MB per event
- Push mode delivers to Event Hubs, the only destination supported today

## Speaker Notes

Not every partner can open an inbound HTTPS port: a regional carrier might run its own scheduler, or its security team might refuse webhooks. The diagram puts push into a storage queue next to pull from a namespace.

Namespaces: a namespace is a container for topics, clients, client groups, topic spaces and permission bindings, and a namespace topic receives CloudEvents 1.0 events. Each event subscription selects `queue` for pull or `push` for delivery to a destination.

Pull: the consumer controls the rate and timing, and calls `ReceiveAsync` and then `AcknowledgeAsync` from its own network. The `Azure.Messaging.EventGrid.Namespaces` package holds `EventGridSenderClient` for publishing and `EventGridReceiverClient` for pulling.

One subscription per partner: the label-service publishes once, every partner pulls at its own pace, and a slow partner delays only its own subscription. Entra ID roles give each partner an identity that you can revoke on its own.

Limits: a namespace topic keeps events for at most 7 days, accepts up to 500 subscriptions and takes events of up to 1 MB. Ingress and egress limits are set per TU, so measure the expected throughput before you size the partner traffic.

Push to Event Hubs: use it when the analytics team needs the same `ParcelDelivered` stream in an event hub, for example to archive it with Capture. The namespace also includes an MQTT broker, which this module only points to for later work.
