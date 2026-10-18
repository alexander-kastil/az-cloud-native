---
slide: "00-02"
topic: 04-service-bus
title: "Overview"
subtitle: "Six reliability features for the parcel event flow"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/04-service-bus/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, in the module's diagram palette: background
  #F8FAFC, 'Segoe UI', Inter, Arial, sans-serif, text #0F172A, secondary #64748B, arrows #334155 3px,
  minimum 28px type. No heading inside the image; the slide title carries it. No arrow crosses a box or
  a label. Two rows of three numbered rounded cards, read left to right, each a two-digit number, a
  title and one secondary line. Top row blue #DBEAFE with #1D4ED8 border, caption above it "Routing,
  triggers and scaling": 01 Topic Subscriptions "SQL and correlation filters", 02 Azure Functions
  "Service Bus triggers", 03 Competing Consumers "KEDA scale rules". Bottom row green #DCFCE7 with
  #15803D border, caption above it "Order, poison messages and duplicates": 04 Sessions "ordered message
  processing", 05 Dead-Letter Queues "poison messages", 06 Idempotent Consumers "duplicate detection".
  Short arrows between neighbours, one elbow connector from card 03 down between the rows to card 04,
  routed in the gap so it crosses no card or caption.
order: 2
source: demos/04-service-bus/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "Six reliability features for the parcel event flow". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Route each parcel event to the **subscriptions** whose filters select it
- Consume `tracking` with a **Functions trigger** that settles after its work
- Scale **competing consumers** with a KEDA rule on the backlog
- Keep each parcel's scans in order with **sessions**
- Dead-letter **poison messages** and drop duplicates with an idempotent consumer

## Speaker Notes

This slide walks the six topics once. Each topic adds one reliability feature to the same parcel event flow, and every example uses Contoso Logistics names.

Routing: the `parcel-events` topic copies each event into the subscriptions whose rules select it, so tracking, notifications and exceptions each receive only their own events.

Triggers: the scan processor runs as an isolated Azure Functions app, and each scan on the `tracking` subscription starts one invocation that settles only after its work succeeds.

Scaling: more consumer instances share the subscription, and a KEDA rule in Container Apps adds replicas as the backlog grows and scales to zero when no scans wait.

Order: sessions use the tracking number as the session ID, so the scans of one parcel apply in the order they were sent.

Poison messages and duplicates: unreadable label scans move to the dead-letter queue with a reason. Duplicate detection and a Cosmos DB marker keep a repeated scan from being stored twice.
