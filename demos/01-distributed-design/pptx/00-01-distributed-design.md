---
slide: "00-01"
topic: 01-distributed-design
title: "Designing event-driven distributed systems"
subtitle: "Where the seams of Contoso Logistics belong before anyone writes a service"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/01-distributed-design/pptx/images/00-01-distributed-design.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers, a woman and a man in their late twenties in modern work-casual
  clothes, at a light-oak standing desk by floor-to-ceiling windows, leaning over a large sheet of white
  paper on which she draws connected rectangles linked by arrows with a dark marker, while he points at
  one arrow between two of the rectangles. An open laptop beside them shows a softly blurred diagram of
  coloured boxes. On a white shelf behind them stand three plain cardboard parcel boxes. The sketch holds
  only rectangles and arrows with no letters, and the parcel boxes carry no labels, printing or tape
  marks. People on the right half, bright calm window wall on the left half as space for a title.
  Modern, light, bright and friendly: high-key natural daylight, white and light-oak contemporary
  interior, soft pastel accents in light blue, mint and lavender, green plants, 2020s design. Screens
  show only soft blurred shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary
  editorial photography. Avoid: readable text, letters on the paper sketch, printed labels on the
  parcels, logos, watermarks, extra people, dark or moody lighting, vintage or retro decor, grime,
  clutter.
order: 1
source: demos/01-distributed-design/readme.md
gamma_prompt: >
  Title slide "Designing event-driven distributed systems" with subtitle "Where the seams of Contoso
  Logistics belong before anyone writes a service". Side-accent layout: title and subtitle on the left
  half on plain white, the provided photograph of two engineers sketching connected boxes at a bright
  standing desk filling the right half, no text over the photo. Use exactly the one image given for this
  card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Designing event-driven distributed systems

## Content

- **Boundaries**: a modular monolith or microservices, and event storming to find the seams
- **Messages**: HTTP or asynchronous commands and events, named and wrapped in CloudEvents
- **Reliability and brokers**: outbox, saga and idempotency, then one Azure broker per flow

## Speaker Notes

This module sets the design vocabulary for the rest of the class, on Contoso Logistics, one application that books shipments, tracks parcels and assigns vans. It has no hands-on demo: the topics use diagrams, tables and small C# records, and the later modules build the patterns named here.

Boundaries: you learn when a modular monolith is the right first step and when a service boundary earns its cost. Event storming then finds those boundaries with the domain experts in the room.

Messages: you choose between synchronous calls and asynchronous messages, and you name the commands and events the services exchange. CloudEvents 1.0 gives those events a common envelope.

Reliability and brokers: outbox, saga and idempotency keep each flow correct under failure. The broker comparison then matches each flow to Service Bus, Event Grid, Event Hubs or Dapr pub/sub.
