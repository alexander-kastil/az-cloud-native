---
slide: "04-02"
topic: 04-timers-events
title: "Reroute approval with external events"
subtitle: "The approval races a 24-hour timer, RaiseEventAsync delivers it"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/04-timers-events/_images/timer-vs-event.svg"
media-file: demos/08-durable-execution/04-timers-events/_images/timer-vs-event.svg
visual-prompt: >
  Existing diagram of three runs of DeliveryDeadlineOrchestrator: delivered before the deadline, rerouted after RerouteApproved, and reroute declined after the 24 h timer, no generation. Referenced at its repo path.
order: 10
source: demos/08-durable-execution/04-timers-events/readme.md
gamma_prompt: >
  Slide titled "Reroute approval with external events", subtitle "The approval races a 24-hour timer,
  RaiseEventAsync delivers it". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Reroute approval with external events

## Content

- An **external event** is a named message, and the name must match on both sides
- Early events are buffered; events for unknown instances are discarded
- Delivery is at least once, so put a unique ID in the payload
- `RerouteApproved` races a 24-hour timer; a timeout returns reroute declined
- `RaiseEventAsync` or the HTTP `raiseEvent` API delivers the approval behind a function key

## Speaker Notes

After a delayed delivery the customer decides whether to accept a reroute, and that decision may never arrive. This slide pairs the wait for a person with a timer and shows how the decision reaches the orchestration.

Named events: the orchestrator waits with WaitForExternalEvent<T>, and the event payload must serialize to T. A misspelled event name waits forever.

Buffered and discarded: an event that arrives before the orchestrator waits is buffered until the orchestrator asks for it. An event sent to an unknown instance is discarded, so check the instance ID before you raise an event.

At least once: a duplicate event can reach the orchestrator, so put a unique ID in the payload and check it where a duplicate would cause harm.

The approval race: after DeliveryDelayed the orchestrator waits up to 24 hours for RerouteApproved, runs RerouteParcel when the approval wins, and returns reroute declined on a timeout. On the Functions Consumption plan, no billing charges accrue while an orchestrator awaits an external event.

Raising the event: a web handler or a queue consumer calls RaiseEventAsync with the instance ID, the event name and the payload, and Durable Functions exposes the same operation as an HTTP API. Protect that endpoint with a function key and validate the approval before you send the event.
