---
slide: "06-01"
topic: 06-idempotent-consumers
title: "Duplicate detection at the broker"
subtitle: "A stable MessageId lets the broker drop a producer retry"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/06-idempotent-consumers/_images/duplicate-stops.svg"
media-file: demos/04-service-bus/06-idempotent-consumers/_images/duplicate-stops.svg
visual-prompt: >
  Existing sequence diagram for PKG-100482.AtHub.3: the broker drops a producer retry inside its five-minute MessageId window, and a redelivery after lock expiry reaches ProcessScan and stops at the 409 on the marker, no generation. Referenced at its repo path.
order: 13
source: demos/04-service-bus/06-idempotent-consumers/readme.md
gamma_prompt: >
  Slide titled "Duplicate detection at the broker", subtitle "A stable MessageId lets the broker drop a
  producer retry". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Duplicate detection at the broker

## Content

- Service Bus delivers each message **at least once**, so the same scan can arrive twice
- The broker keeps `MessageId` values for a window: 10 minutes by default, 20 seconds to 7 days
- The `parcel-events` topic uses a five-minute window
- A stable `MessageId` such as `PKG-100482.AtHub.3` makes a retry match
- The broker checks only sends, so a redelivery still reaches the handler

## Speaker Notes

A lock can expire during a slow write, a consumer can crash after it updates the tracking read model, and a producer can retry a send that already succeeded. The handler must produce the same result both times.

At least once: Service Bus delivers each message at least once, so the same scan can reach a handler twice.

The window: when a send carries a `MessageId` that is already in the history, the send succeeds and the broker drops the new message. A larger window keeps more IDs to match, which lowers throughput.

Grey Logistics: you turn on duplicate detection when you create the queue or topic, and it needs the Standard or Premium tier. The `parcel-events` topic sets `requiresDuplicateDetection` with a five-minute `duplicateDetectionHistoryTimeWindow`.

Stable MessageId: the producer composes it from the parcel ID, the scan type and a sequence number, so a retry of the same scan produces the same ID.

Consumer side: a redelivery of a message the consumer already processed passes the broker and reaches the handler. The consumer has to stay idempotent on its own, which is the second stop in the diagram and the next slide.
