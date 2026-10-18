---
slide: "00-01"
topic: 08-durable-execution
title: "Durable execution, workflows and sagas"
subtitle: "Code that lets a long-running process survive restarts"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/08-durable-execution/pptx/images/00-01-durable-execution.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers at a white standing desk in the bright engineering room of a modern
  parcel delivery company: a young woman points at one step of a softly blurred chain of connected
  workflow boxes on a large monitor, while a young man beside her, holding a coffee mug, follows her
  finger and smiles. Behind them, through a wide interior window, a light warehouse area with neatly
  stacked plain cardboard parcels on light-oak shelves; the parcels carry no labels, print or barcodes.
  The two people are in the centre of the frame. Modern, light, bright and friendly: high-key natural
  daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint and
  lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never readable
  UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos,
  watermarks, extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 1
source: demos/08-durable-execution/readme.md
gamma_prompt: >
  Title slide "Durable execution, workflows and sagas" with subtitle "Code that lets a long-running
  process survive restarts". Side-accent layout: title and subtitle on the left half on plain white, the
  provided photograph of two engineers at a standing desk in a parcel company's engineering room filling
  the right half, no text over the photo. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Durable execution, workflows and sagas

## Content

- **Replay and determinism**: the event history that lets an orchestration resume where it stopped
- **Backends and hosts**: the Durable Task Scheduler, Durable Functions, KEDA-scaled workers and Dapr Workflow
- **The booking saga**: delivery deadlines, customer approvals and compensations that undo finished steps

## Speaker Notes

This opening slide sets out the three themes of the module. Contoso Logistics books a parcel across capacity, payment and label services, and a process that restarts halfway has to resume where it stopped and undo the steps it already finished.

Replay and determinism: the runtime stores the result of each completed step in an event history and rebuilds state by replaying the orchestrator code. Every later topic depends on these rules.

Backends and hosts: the Durable Task Scheduler keeps orchestration state for Durable Functions and the Durable Task SDKs, and SDK workers scale with KEDA on Container Apps. Dapr Workflow is the portable option for services that already run on AKS with Dapr.

The booking saga: durable timers and external events wait for a delivery deadline or a customer decision without holding a worker. The saga refunds the payment and releases the capacity when a later step fails.
