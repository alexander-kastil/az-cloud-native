---
slide: "04-02"
topic: 04-pub-sub
title: "Dead-letter topics"
subtitle: "Messages that still fail after the retries land on poison-scans"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/04-02-dead-letter-paths.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette, minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. Two lane panels, light slate with a grey border. Top panel "Dapr": blue box "Message fails",
  arrow to blue box "Dapr retries, controlled by the resiliency spec", arrow labelled "still fails" to a
  purple box "poison-scans, dead-letter topic" with a white chip "deadLetterTopic: poison-scans", the
  topic name in pink. Bottom panel "Service Bus broker": amber box "Message exceeds the default
  MaxDeliveryCount of 10", arrow to amber box "broker dead-letter queue with the reason
  MaxDeliveryCountExceeded".
order: 10
source: demos/07-dapr/04-pub-sub/readme.md
gamma_prompt: >
  Slide titled "Dead-letter topics", subtitle "Messages that still fail after the retries land on
  poison-scans". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dead-letter topics

## Content

- Declare a subscription in a `dapr.io/v2alpha1` `Subscription` file or in code
- `deadLetterTopic: poison-scans` takes what still fails after the retries
- The resiliency spec controls how Dapr retries
- Service Bus dead-letters a message after the default `MaxDeliveryCount` of 10
- `maxDeliveryCount` takes effect only when Dapr creates the subscription

## Speaker Notes

A scan message that keeps failing needs a place to go, and two mechanisms decide where: Dapr's own dead-letter topic and the Service Bus broker's dead-letter queue.

Subscription file: the Dapr 1.18 documentation uses the `dapr.io/v2alpha1` form of the `Subscription` kind, which routes `parcel-events` to `/parcel-events`. The `Topic` attribute overload with a dead-letter topic sets the same value in code.

Dead-letter topic: a message that still fails after the retries lands on `poison-scans`.

Dapr retries: the resiliency spec controls them, which the resiliency topic covers next.

Broker limit: Service Bus has its own rule, and a message that exceeds the default `MaxDeliveryCount` of 10 moves to the broker dead-letter queue with the reason `MaxDeliveryCountExceeded`.

Creation-time settings: component settings apply to every topic that uses the component. `maxDeliveryCount` and the other subscription settings apply only when Dapr creates the subscription, so a later change does not alter an existing one.
