---
slide: "00-02"
topic: 01-distributed-design
title: "Overview"
subtitle: "The topics follow the order of a design decision"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, text #0F172A, secondary text #64748B, arrows #334155
  3px, minimum 26px type. No heading inside the image; the slide title carries it. No arrow crosses a
  box or a label. Two rows of three numbered rounded cards, read left to right, each card a two-digit
  number, the topic title from the module readme and one secondary line. A caption with a coloured bar
  sits in the left margin of each row. Top row blue #DBEAFE with #1D4ED8 borders, caption "Where the
  seams belong": 01 Modular Monolith vs. Microservices "When a service boundary earns its cost", 02 Sync
  vs. Async: Commands & Events "Request/reply, messages and temporal coupling", 03 Event Storming: Find
  Service Boundaries "Bounded contexts and the Contoso service list". Bottom row green #DCFCE7 with
  #15803D borders, caption "Messages across the seams": 04 Event Types: Domain, Integration, CloudEvents
  "Payload styles and the CloudEvents 1.0 envelope", 05 Reliability Patterns: Outbox, Saga, Idempotency
  "The failures that break event flows", 06 Azure Messaging: Pick the Broker "Service Bus, Event Grid,
  Event Hubs, Dapr pub/sub". Short arrows between neighbours, one elbow connector from card 03 down
  through the gap between the rows to card 04, clear of both captions.
order: 2
source: demos/01-distributed-design/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The topics follow the order of a design decision". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Decide when a **modular monolith** is enough and when a split earns its cost
- Choose **HTTP** when the caller needs the answer, messages when the work only has to happen
- Find the boundaries with **event storming**, then name the events that cross them
- Keep each flow correct under failure with **reliability patterns**
- Match each flow to an **Azure broker**, then design the parcel flow in the lab

## Speaker Notes

This slide walks the six topics once, in the order a design decision takes. The top row finds where the seams belong, the bottom row decides which messages cross them.

Monolith or microservices: four criteria decide whether a split pays off, and a modular monolith keeps the same seams inside one deployable.

Sync or async: commands, events and queries differ in what the sender expects. HTTP stays right when the caller needs the answer to continue.

Boundaries and event types: event storming finds the bounded contexts with the domain experts. Event types then decide what crosses them: domain or integration, notification or state transfer, in a CloudEvents envelope.

Reliability: dual writes, duplicates, partial failures, poison messages and transient faults each have a pattern that prevents them, such as the transactional outbox or dead-lettering.

Brokers and the lab: Service Bus, Event Grid, Event Hubs and Dapr pub/sub are compared by delivery, ordering and retention. The lab then turns the parcel delivery flow into `docs/events.md` and `docs/architecture.md`.
