---
slide: "05-01"
topic: 05-dead-letter
title: "Delivery limit and dead-letter reasons"
subtitle: "The broker moves a message once its delivery count passes maxDeliveryCount"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/05-dead-letter/_images/dead-letter-bounces.svg"
media-file: demos/04-service-bus/05-dead-letter/_images/dead-letter-bounces.svg
visual-prompt: >
  Existing diagram of an unreadable label scan abandoned until DeliveryCount reaches 10 and dead-lettered with MaxDeliveryCountExceeded, beside scan-processor dead-lettering it on the first delivery with UnreadableLabel, no generation. Referenced at its repo path.
order: 11
source: demos/04-service-bus/05-dead-letter/readme.md
gamma_prompt: >
  Slide titled "Delivery limit and dead-letter reasons", subtitle "The broker moves a message once its
  delivery count passes maxDeliveryCount". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect
  ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Delivery limit and dead-letter reasons

## Content

- Every abandon and every expired lock adds one to the **delivery count**
- Past `maxDeliveryCount`, 10 by default, the broker dead-letters with `MaxDeliveryCountExceeded`
- Other system reasons: `TTLExpiredException`, `HeaderSizeExceeded`, `Session ID is null`, `MaxTransferHopCountExceeded`
- `deadLetteringOnMessageExpiration` moves every expiring message to the dead-letter queue
- Dead-letter a scan yourself on its first delivery when it can't succeed

## Speaker Notes

Some scans can't be read: a label scanner that prints a malformed barcode produces a message that no handler can parse. Every redelivery wastes handler time on it, so Grey Logistics moves these messages to the dead-letter queue of the `tracking` subscription.

Delivery count: each abandon and each expired lock adds one. A handler that never settles its messages exhausts the count, and so does a receiver that closes before it settles.

Delivery limit: each queue and subscription sets `maxDeliveryCount`, which defaults to 10. When the count passes it, the broker moves the message with the reason `MaxDeliveryCountExceeded`; the limit is adjustable and the behaviour is always on.

System reasons: the broker also dead-letters expired messages, oversized headers, messages without a session ID on a session-enabled entity, and auto-forwarding past four hops. The system sets the reason and description, and your code can read both.

Expiry: set `deadLetteringOnMessageExpiration` to true on the queue or subscription to move every expiring message to the dead-letter queue. Deferred messages don't move there when they expire, by design.

Explicit dead-lettering: when a handler knows a scan can't succeed, it dead-letters the scan on the first delivery with its own reason. The right side of the diagram shows `ScanValidator.Validate` failing and `scan-processor` using the reason `UnreadableLabel`.
