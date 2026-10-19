---
slide: "00-01"
topic: 06-event-sourcing-cqrs
title: "Event sourcing and CQRS with Cosmos DB"
subtitle: "Grey Logistics keeps every shipment change as an event"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/06-event-sourcing-cqrs/pptx/images/00-01-event-sourcing.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers, a woman and a man around thirty in modern work-casual clothes, sit side
  by side at a shared light-oak desk in the bright software team room of a modern parcel delivery
  company. She holds a plain brown cardboard parcel up in both hands beside the open laptop in front of
  them, which is turned halfway towards the camera and shows one long horizontal row of small soft pastel
  blocks, the newest block at the end glowing mint green, with no letters, words or numbers anywhere. He
  traces the row along the screen with one finger, and both smile. The two engineers sit close together
  in the middle of the frame, faces fully visible. Modern, light, bright and friendly: high-key natural
  daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint and
  lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never readable
  UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos,
  watermarks, labels, barcodes or printed tape on the parcel, extra people, dark or moody lighting,
  vintage or retro decor, grime, clutter.
order: 1
source: demos/06-event-sourcing-cqrs/readme.md
gamma_prompt: >
  Title slide "Event sourcing and CQRS with Cosmos DB" with subtitle "Grey Logistics keeps every
  shipment change as an event". Side-accent layout: title, subtitle and three bullets on the left half on
  plain white, the provided photograph of two engineers holding a parcel beside its row of events on a
  laptop filling the right half, no text over the photo. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Event sourcing and CQRS with Cosmos DB

## Content

- **Atomic publishing**: the booking and its outbox event in one batch, relayed to Service Bus
- **Read models**: change feed projections, tuned queries and a Redis cache
- **Event store**: version-checked appends, replay, snapshots and safe schema changes

## Speaker Notes

This module is where Grey Logistics keeps its history: shipping-api stores every shipment change as an immutable event in Cosmos DB, and the tracking page reads from a separate model built from those events. It follows the event streams of Module 5 and comes before the Dapr portability work of Module 7.

Atomic publishing: a booking and its events are saved in one atomic batch, and a change feed relays the events to Service Bus without losses. The later topics reuse this batch writer and its relay.

Read models: a change feed processor projects the events into a tracking read model, which you tune for indexing, request units and consistency. Azure Managed Redis serves the tracking queries, and the change feed invalidates the cached page.

Event store: the Shipment aggregate is rebuilt by replaying its events, with snapshots for long streams. Event schemas evolve without breaking the consumers that already run.
