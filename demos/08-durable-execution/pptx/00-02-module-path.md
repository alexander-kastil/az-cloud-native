---
slide: "00-02"
topic: 08-durable-execution
title: "Overview"
subtitle: "The path through durable execution"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, borders 3px, headings #0F172A, secondary text #64748B,
  minimum 26px type. No heading inside the image; the slide title carries it. No arrow crosses a box or
  a label. Two rows of numbered rounded cards, read left to right, each card a two-digit number, the
  topic name and one secondary line from the module readme topic table. Top row blue #DBEAFE with
  #1D4ED8 border: 01 Durable Execution "replay and determinism", 02 Durable Task Scheduler "managed
  backend", 03 Durable Functions "orchestrations and entities", 04 Durable Timers "external events and
  approvals". Bottom row of three, centred, green #DCFCE7 with #15803D border: 05 Durable Task SDK
  "KEDA-scaled workers", 06 Dapr Workflow "portable orchestrations", 07 Saga Orchestration
  "compensating activities". Short arrows between neighbours, one elbow connector from card 04 down
  between the rows to card 05, routed in the gap so it crosses no card.
order: 2
source: demos/08-durable-execution/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path through durable execution". Layout: Two columns. Left
  column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Start with **replay and determinism**, the rules every later topic depends on
- Keep orchestration state in the **Durable Task Scheduler** and run it with Durable Functions
- Wait for deadlines and approvals with **durable timers** and external events
- Scale SDK workers with **KEDA**, or pick **Dapr Workflow** as the portable option
- Combine the pieces into the **booking saga** with compensating activities

## Speaker Notes

This slide walks the path through the module's seven topics once, following the three themes of the title slide.

Replay and determinism: topic 1 explains how the event history rebuilds state and which calls an orchestrator must avoid. Every later topic depends on it.

The managed backend: topic 2 covers schedulers, task hubs, the two SKUs and the emulator, and topic 3 runs orchestrations and entities on that backend with Durable Functions.

Timers and events: topic 4 pauses an orchestration for a delivery deadline or a customer's reroute approval without holding a worker.

Workers and portability: topic 5 runs Durable Task SDK workers on Container Apps with a KEDA scale rule, and topic 6 shows Dapr Workflow as the portable alternative on AKS.

The booking saga: topic 7 combines the pieces into a saga that undoes completed steps in reverse order when a later step fails.
