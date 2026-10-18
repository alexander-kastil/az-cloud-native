---
slide: "05-01"
topic: 05-reliability-patterns
title: "Failures that break event flows"
subtitle: "Each failure in one booking and the pattern that prevents it"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/05-reliability-patterns/_images/failure-map.svg"
media-file: demos/01-distributed-design/05-reliability-patterns/_images/failure-map.svg
visual-prompt: >
  Existing diagram of one booking flow from shipping-api through Cosmos DB and the Service Bus topic shipment-events to tracking-service and exception-agent, beside the booking saga in booking-worker, with the six failures and their patterns numbered at the step where each one happens, no generation. Referenced at its repo path.
order: 13
source: demos/01-distributed-design/05-reliability-patterns/readme.md
gamma_prompt: >
  Slide titled "Failures that break event flows", subtitle "Each failure in one booking and the pattern
  that prevents it". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Failures that break event flows

## Content

- The **transactional outbox** stores the change and the event in one transaction
- The **idempotent consumer** records each message it has processed
- A **saga** undoes committed steps when a later step fails
- **Retries** with exponential backoff and jitter handle transient faults
- **Dead-lettering** moves poison messages aside; a claim check carries large payloads

## Speaker Notes

A booking saves a shipment, reserves a van, captures a payment and prints a label, and any step can fail after an earlier one has committed. The failure map numbers each failure at the step where it happens; the linked topics later in the class build each pattern.

Dual write: a service commits its change and then fails to publish the event, or the reverse. The transactional outbox writes both in one local transaction, and a relay publishes the stored event later.

Duplicates: brokers deliver at least once, so a producer retry, a lost acknowledgment or a consumer crash can deliver a message twice. The idempotent consumer keys its records on a producer-assigned message ID or a business-level idempotency key, because one correlation ID covers several related messages.

Partial failure: a step that fails after earlier steps committed cannot roll back across services. A saga undoes the earlier steps with compensating transactions.

Transient faults: a momentary loss of network connectivity or a busy service usually clears when the operation runs again later, so a bounded retry handles it.

Poison messages and large payloads: a message that never succeeds is dead-lettered after the maximum delivery count, so the queue keeps moving. A payload larger than the broker accepts goes to external storage, and only a claim check reference travels through the broker.
