---
slide: "99-01"
topic: 05-event-grid-hubs
title: "Takeaways"
subtitle: "What to carry into event sourcing and CQRS"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/05-event-grid-hubs/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Four software engineers, two women and two men in their late twenties to mid thirties in
  modern work-casual clothes, stand at a large whiteboard wall in a bright project room at the end of a
  workshop day. Pastel sticky notes in light blue, mint and lavender are joined into one flow by drawn
  arrows, and one engineer draws the last arrow that connects the two halves while the others watch and
  smile. The sticky notes and the whiteboard carry no writing at all. Group centred in the frame. Modern,
  light, bright and friendly: high-key natural daylight, white and light-oak contemporary interior, soft
  pastel accents in light blue, mint and lavender, green plants, 2020s design. Screens show only soft
  blurred shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary editorial
  photography. Avoid: readable text, words or letters on the sticky notes or whiteboard, logos,
  watermarks, extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 16
source: demos/05-event-grid-hubs/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into event sourcing and CQRS". Side-accent
  layout: title, subtitle and five bullets on the left half on plain white, the provided photograph of a
  team joining a flow of sticky notes on a whiteboard wall filling the right half, no text over it. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Publish `ParcelDelivered` once and keep its event type stable
- Give every subscription filters, a retry policy and a **dead-letter** container
- Offer **pull** delivery to partners that can't open an inbound port
- Key by van ID, read with one consumer group per service, keep handlers idempotent
- Push to browsers through **SignalR Service** in Default mode

## Speaker Notes

Close with the decisions this module made for Contoso Logistics. Each one is a setting or a line of code you can point to in the lab.

One stable event: the label-service publishes `ParcelDelivered` once as a CloudEvent, and its event type is the contract every subscriber relies on.

Delivery that keeps its failures: filters, maximum attempts and time to live are set per subscription, and a dead-letter container keeps the events Event Grid gives up on.

Pull for partners: a namespace topic lets each partner receive, acknowledge and release at its own pace, without opening an inbound port.

Correct streams: the van ID key keeps each van's positions in order, each service reads through its own consumer group, and idempotent handlers absorb the replays of at-least-once delivery.

Push to the browser: telemetry-ingest hosts `VanPositionHub`, `AddAzureSignalR` routes it through Azure SignalR Service in Default mode, and the ops console map shows each position as it arrives.
