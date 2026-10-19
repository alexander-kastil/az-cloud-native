---
slide: "00-02"
topic: 06-event-sourcing-cqrs
title: "Overview"
subtitle: "The path through event sourcing and CQRS"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif in the module's palette (blue #DBEAFE with #1D4ED8, green #DCFCE7 with #15803D, text
  #0F172A, secondary #64748B, arrows #334155), minimum 26px type. No heading inside the image; the slide
  title carries it. No arrow crosses a box or a label. Seven numbered rounded cards read left to right,
  each a two-digit number, the topic name and its tagline from the module readme table. Top row blue: 01
  Transactional Outbox "Atomic Publishing", 02 Change Feed Processor "Projections", 03 CQRS "Separate
  Writes from Reads", 04 Read Models "Indexing, RUs, Consistency". Bottom row green: 05 Managed Redis
  "Change-Feed Cache Invalidation", 06 Event Store "Aggregates & Snapshots", 07 Event Versioning "Evolve
  Schemas Safely". Short arrows between neighbours, one elbow connector from card 04 down through the gap
  between the rows to card 05.
order: 2
source: demos/06-event-sourcing-cqrs/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path through event sourcing and CQRS". Layout: Two columns. Left
  column: the bullets. Right column, wider than the left: the provided diagram as an inline content image
  at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a background
  or side accent. Use exactly the one image given for this card; do not generate or add any other
  images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Save the booking and its outbox event in one batch, relay it with the **change feed**
- Project the events into the **tracking read model** with a change feed processor
- Split commands from queries, then tune indexing, RUs and session tokens
- Serve the tracking page from **Azure Managed Redis**, cleared after each scan
- Store events per shipment, replay with snapshots and **version** the schemas

## Speaker Notes

This slide walks the seven topics once, in the order the module builds them. The outbox comes first because later topics reuse its batch writer and its relay.

Outbox: the ShipmentBooked event and its outbox event commit in one Cosmos DB transactional batch. A change feed processor relays the outbox event to the shipment-events topic with duplicate detection on.

Projections: a change feed processor turns ParcelScanned events into ScanRecord documents in the tracking read model. It survives restarts, shares work across instances and never skips a change.

CQRS and read models: shipping-api accepts commands and tracking-service answers queries from its own container. The read model is then tuned with an indexing policy, request charge measurements and session tokens.

Cache: Azure Managed Redis answers repeat reads of the tracking page with cache-aside, and the change feed projection deletes the cached page after each scan commits.

Event store and versioning: each change is an immutable event, a version in the ID guards appends, and snapshots keep replay short. Schemas evolve through additive changes, upcasting on read and the CloudEvents type and dataschema attributes.
