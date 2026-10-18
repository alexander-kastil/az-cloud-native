---
slide: "02-01"
topic: 02-agent-subscribers
title: "The agent as an event consumer"
subtitle: "A filtered subscription and the CloudEvents envelope"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/delayed-subscription-filter.svg"
media-file: labs/09-ai-agents/_images/delayed-subscription-filter.svg
visual-prompt: >
  Existing diagram of the exception-agent subscription on parcel-events, whose SQL rule passes only DeliveryDelayed to DelayedParcelWorker, which reads the event id and parcel id from the CloudEvents body, no generation. Referenced at its repo path.
order: 5
source: demos/09-ai-agents/02-agent-subscribers/readme.md
gamma_prompt: >
  Slide titled "The agent as an event consumer", subtitle "A filtered subscription and the CloudEvents
  envelope". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The agent as an event consumer

## Content

- A **topic subscription** with a SQL rule passes only DeliveryDelayed, the filter pattern from Module 4
- The body is a **CloudEvents 1.0** envelope: `id` is the event id, `data` holds shipmentId and trackingNumber
- The publisher owns the schema; the agent reads only the fields it needs
- A new optional field breaks nothing, a removed or renamed field does
- Customer details enter a prompt only when the draft needs them

## Speaker Notes

Topic 02 treats the agent as one more subscriber: when a parcel misses its window, the durable deadline raises DeliveryDelayed, and the exception-agent reacts the way any other service does. The diagram is the lab's first step, the subscription, its rule and the worker that reads the event.

Filtered subscription: the exception-agent subscription on parcel-events has the rule delivery-delayed, a SQL filter on the application property type, so ShipmentBooked never reaches the worker. It is the same filter pattern as Module 4.

CloudEvents input: the handler reads a CloudEvents 1.0 envelope whose id is the event id and whose data holds shipmentId and trackingNumber. In the lab the tracking number is the parcel id.

Schema ownership: the publisher owns the schema and the agent declares the subset it reads, so a new optional field breaks nothing while a removed or renamed field does. Choreography guidance asks for a schema registry and backward-compatible changes for this reason.

Minimal prompt data: customer details enter a prompt only when the draft needs them, so the prompt carries no field that the task does not use.
