---
slide: "04-01"
topic: 04-timers-events
title: "Durable timers and the delivery deadline"
subtitle: "A durable timer races the ParcelDelivered event"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/delivery-deadline-race.svg"
media-file: labs/08-durable-execution/_images/delivery-deadline-race.svg
visual-prompt: >
  Existing diagram of DeliveryDeadlineOrchestrator racing a durable timer against the ParcelDelivered event and publishing DeliveryDelayed through the Dapr pubsub component when the timer wins, no generation. Referenced at its repo path.
order: 9
source: demos/08-durable-execution/04-timers-events/readme.md
gamma_prompt: >
  Slide titled "Durable timers and the delivery deadline", subtitle "A durable timer races the
  ParcelDelivered event". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Durable timers and the delivery deadline

## Content

- `CreateTimer` completes at a future time; the timer is stored with the instance and survives a restart
- Compute the due time from `context.CurrentUtcDateTime`, never `DateTime.UtcNow`
- `WaitForExternalEvent<ParcelDelivered>` and the timer race in `Task.WhenAny`
- When the timer wins, `RaiseDeliveryDelayed` publishes `DeliveryDelayed` to `parcel-events`
- Cancel the timer that loses, because an outstanding timer keeps the orchestration open

## Speaker Notes

Every Grey parcel has a promised delivery window, and when the window closes without a delivery the company must tell the customer. This slide shows how an orchestration waits for time without holding a worker.

Durable timers: the orchestrator awaits CreateTimer and sleeps until the due time, and the scheduler wakes the instance when the timer fires. While the timer is pending, the orchestration keeps processing other events.

Due time: pass the promised window from the booking as input and compute the due time from context.CurrentUtcDateTime. In Durable Functions, .NET and Java apps support timers of any length, while JavaScript and Python apps limit a single timer to six days.

The race: the orchestrator starts the window timer and WaitForExternalEvent for ParcelDelivered, then awaits whichever finishes first with Task.WhenAny. A scan that confirms delivery before the deadline returns delivered.

DeliveryDelayed: when the timer wins, the RaiseDeliveryDelayed activity publishes the event through the Dapr pubsub component to the parcel-events topic. Other services react to that event without knowing about this orchestration.

Cancel what you no longer need: the orchestration does not complete while a timer is still outstanding, so cancel the timer that lost the race.
