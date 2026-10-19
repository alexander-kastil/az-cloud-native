---
slide: "03-02"
topic: 03-namespaces-pull
title: "Lock tokens in pull delivery"
subtitle: "Every received event stays locked until the consumer settles it"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/05-event-grid-hubs/pptx/images/03-02-lock-tokens.svg
visual-prompt: >
  Opus 5.5, hand-written SVG in the module's diagram style, viewBox 0 0 1920 1080, slate #F8FAFC canvas
  with one white panel bordered #94A3B8 2px, 'Segoe UI', Inter, Arial, sans-serif, headings #0F172A,
  secondary text #64748B, minimum 26px type, arrows #334155. No heading inside the image; the slide
  title carries it. No arrow crosses a box or a label. A state diagram: a start dot, "ParcelDelivered
  published", to blue #DBEAFE "Available, in the subscription"; "Receive, ReceiveAsync" to amber #FEF3C7
  "Locked, by the consumer"; "Acknowledge, AcknowledgeAsync" to green #DCFCE7 "Deleted, for this
  subscription". A loop on top of Locked "Renew lock, RenewLocksAsync". An elbow back from Locked to
  Available below the row, "Release, ReleaseAsync, after 0, 10, 60, 600 or 3600 s". From Locked down
  "Reject, RejectAsync" to a grey choice diamond: "dead-letter destination exists" to purple #EDE9FE
  "Dead-lettered", "no dead-letter destination" to grey "Dropped". A note box at the bottom left:
  "Receive: up to 100 events per call, waits 10 to 120 s, 60 s by default" and "Renew lock: within one
  hour of the first receive, each renewal extends the lock again".
order: 9
source: demos/05-event-grid-hubs/03-namespaces-pull/readme.md
gamma_prompt: >
  Slide titled "Lock tokens in pull delivery", subtitle "Every received event stays locked until the
  consumer settles it". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Lock tokens in pull delivery

## Content

- `ReceiveAsync` reads up to 100 events and locks them for the consumer
- `AcknowledgeAsync` deletes the event for this subscription only
- `ReleaseAsync` makes it available again after 0, 10, 60, 600 or 3600 seconds
- `RejectAsync` dead-letters it, or drops it when no destination exists
- `RenewLocksAsync` extends the lock while the consumer still works on it

## Speaker Notes

Each received event carries a lock token, which identifies the event and controls its state. The five operations of pull delivery all work on those tokens.

Receive: one call reads up to 100 events and waits 60 seconds by default, within a range of 10 to 120 seconds. Each `ReceiveDetails` item holds the CloudEvent in `Event` and the lock token in `BrokerProperties`.

Acknowledge: it deletes the event from the subscription, so no consumer receives it again. Acknowledgment applies only to the subscription that received the event, and other subscriptions keep their own copy.

Release: the sample releases events it can't process yet with `ReleaseDelay.OneMinute`. A release delay must be 0, 10, 60, 600 or 3600 seconds.

Reject: the sample rejects events without a parcel ID, which dead-letters them when a dead-letter destination exists and drops them otherwise.

Renew lock: a lock can be renewed within one hour of the first receive, and each renewal can extend it again. The partner's receive loop stops only after it has acknowledged, released or rejected every lock token it holds.
