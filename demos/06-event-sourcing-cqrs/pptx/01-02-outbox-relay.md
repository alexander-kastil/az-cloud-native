---
slide: "01-02"
topic: 01-outbox
title: "Relaying outbox events with the change feed"
subtitle: "MessageId and duplicate detection drop the copies a retry sends"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/06-event-sourcing-cqrs/01-outbox/_images/outbox-relay.svg"
media-file: demos/06-event-sourcing-cqrs/01-outbox/_images/outbox-relay.svg
visual-prompt: >
  Existing diagram of the outbox relay: one batch into partition SHP-10482, OutboxRelay sending twice after a retry, shipment-events dropping the copy by MessageId, the lease checkpoint and TTL removing the outbox event after 10 days, no generation. Referenced at its repo path.
order: 4
source: demos/06-event-sourcing-cqrs/01-outbox/readme.md
gamma_prompt: >
  Slide titled "Relaying outbox events with the change feed", subtitle "MessageId and duplicate detection
  drop the copies a retry sends". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Relaying outbox events with the change feed

## Content

- A change feed processor reads new outbox documents and **checkpoints** in a lease container
- `MessageId` is the document `id`, `SessionId` is the shipment ID
- Every send is awaited; a throw retries the same batch, so some events go out twice
- **Duplicate detection** on `shipment-events` drops the copies (lab window: five minutes)
- TTL removes the outbox event after ten days, the event document stays

## Speaker Notes

With both documents stored, the outbox event still has to reach Service Bus. The diagram follows one booking from the batch to tracking-service and exception-agent.

Change feed: the change feed is a persistent log of changes, ordered within one logical partition. The relay records its position in a lease container and resumes from there after a restart.

Message fields: the relay filters for outbox documents and sets MessageId to the document id and SessionId to the shipment ID. The session ID keeps each shipment in order.

Await and retry: the delegate awaits every send, because a processor can checkpoint before unawaited work finishes. If the delegate throws, the processor retries the same batch and sends some events twice.

Duplicate detection: the topic compares MessageId within a history window, 10 minutes by default and at most 7 days, on the Standard or Premium tier. The lab's AppHost sets five minutes, which the Service Bus emulator accepts.

TTL cleanup: Cosmos DB deletes the outbox event in the background on leftover request units once its ttl has passed. Item-level ttl needs DefaultTimeToLive on the container, and -1 turns TTL on without expiring other items.
