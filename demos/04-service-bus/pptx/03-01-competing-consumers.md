---
slide: "03-01"
topic: 03-competing-consumers
title: "Competing consumers"
subtitle: "Replicas share the tracking subscription, and sessions keep each parcel in order"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/03-competing-consumers/_images/competing-consumers.svg"
media-file: demos/04-service-bus/03-competing-consumers/_images/competing-consumers.svg
visual-prompt: >
  Existing diagram of three scan-processor replicas sharing the session-enabled tracking subscription, replica 1 locking session PKG-100482 so the tracking page lists PickedUp before AtHub, no generation. Referenced at its repo path.
order: 7
source: demos/04-service-bus/03-competing-consumers/readme.md
gamma_prompt: >
  Slide titled "Competing consumers", subtitle "Replicas share the tracking subscription, and sessions
  keep each parcel in order". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled
  to fit the column, never cropped, never used as a background or side accent. Use exactly the one
  image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Competing consumers

## Content

- Several instances receive from one subscription, and each message reaches one of them
- More instances raise throughput, and a failed instance doesn't block the others
- Parallel consumers give up ordering, so `tracking` requires **sessions**
- `ServiceBusProcessor`: `MaxConcurrentCalls` defaults to 1, `PrefetchCount` to 0
- Set `PrefetchCount` at least as high as `MaxConcurrentCalls`, and keep it small for slow handlers

## Speaker Notes

A depot scan burst can fill the `tracking` subscription faster than one consumer drains it. Grey Logistics adds consumer instances that share the subscription, so each scan reaches exactly one of them.

One message, one consumer: in the competing consumers pattern, several instances receive from one queue or subscription, and the broker delivers each message to one of them.

Throughput and resilience: adding instances raises throughput, and the remaining instances keep receiving when one fails. Microsoft Learn recommends the Premium tier for production workloads with varied throughput.

Ordering: the order in which instances receive messages isn't guaranteed, so handlers should process messages idempotently. The `tracking` subscription requires sessions, so its replicas keep each parcel's scans in order.

Processor concurrency: `ServiceBusProcessor` runs a message handler on a pool of callbacks inside one process. `MaxConcurrentCalls` sets how many handlers run at once, and `PrefetchCount` sets how many messages wait in a local buffer.

Prefetch: the buffer is a strict upper limit, so a `PrefetchCount` below `MaxConcurrentCalls` leaves some handlers without work. Prefetched messages can lose their locks while they wait, so keep the buffer small when handlers are slow.
