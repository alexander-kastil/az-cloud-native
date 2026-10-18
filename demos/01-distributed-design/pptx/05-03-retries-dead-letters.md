---
slide: "05-03"
topic: 05-reliability-patterns
title: "Retries and dead-letters"
subtitle: "Bounded retries for transient faults, a side queue for the rest"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/05-reliability-patterns/_images/backoff-jitter.svg"
media-file: demos/01-distributed-design/05-reliability-patterns/_images/backoff-jitter.svg
visual-prompt: >
  Existing diagram of four clients that fail against a busy service and retry near 1, 2, 4 and 8 seconds, each retry shifted by its own random delay, until the retry limit, no generation. Referenced at its repo path.
order: 15
source: demos/01-distributed-design/05-reliability-patterns/readme.md
gamma_prompt: >
  Slide titled "Retries and dead-letters", subtitle "Bounded retries for transient faults, a side queue
  for the rest". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Retries and dead-letters

## Content

- Retry transient faults with **exponential backoff and jitter**
- Keep a finite retry count and a retry budget per dependency
- Give every outbound call a **timeout**
- Service Bus dead-letters a message after **ten** failed deliveries by default
- Event Grid dead-letters to a storage account once you configure it

## Speaker Notes

A retry suits a transient fault, because the fault clears on its own. The chart shows four clients that fail against a busy service and then retry on their own schedules.

Backoff and jitter: the Azure Architecture Center recommends exponential backoff plus a random delay, so many clients do not retry in the same instant.

Bounded retries: use a finite retry count, a retry budget per dependency and at most one immediate retry. Retries without a limit keep a struggling service down.

Timeouts: retries multiply latency, so every outbound call also needs a timeout.

Service Bus: after ten failed deliveries by default it moves the message to the dead-letter queue and records the reason, such as MaxDeliveryCountExceeded. The queue holds the evidence for an operator to inspect, replay or discard.

Event Grid: it sends undelivered events to a storage account after its retry policy finishes, once you configure dead-lettering on the subscription. It does not turn dead-lettering on by default.
