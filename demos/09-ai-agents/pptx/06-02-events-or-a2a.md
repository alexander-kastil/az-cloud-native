---
slide: "06-02"
topic: 06-agent-choreography
title: "Choosing events or A2A"
subtitle: "Two questions decide each interaction"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/09-ai-agents/pptx/images/06-02-events-or-a2a.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif, module palette, minimum 27px type. No heading inside the image. No arrow crosses a box or a
  label. Two amber #FEF3C7 question boxes on the left, each with an arrow labelled "yes" to an outcome
  box on the right. "Does the caller need an answer now?" leads to purple #EDE9FE "A2A" "one named agent
  per call, the caller waits" "caller owns the timeout, callee owns the task state" with a white chip
  "the ops console could ask the exception-agent to explain one delay (design)". "Will more consumers
  appear later?" leads to blue #DBEAFE "Events (choreography)" "the publisher knows nothing about its
  consumers" "retries, dead-letter queues, idempotent consumers" with a white chip "booking-worker
  publishes DeliveryDelayed; the exception-agent reacts and publishes RerouteRequested". Green #DCFCE7
  band across the bottom: "Most Grey Logistics flows start as choreography; A2A only where one answer
  is needed".
order: 16
source: demos/09-ai-agents/06-agent-choreography/readme.md
gamma_prompt: >
  Slide titled "Choosing events or A2A", subtitle "Two questions decide each interaction". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate or
  add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Choosing events or A2A

## Content

- Does the caller need an answer now? Then **A2A**
- Will more consumers appear later? Then **events**
- Most Grey Logistics flows start as choreography
- A2A 1.0, announced on April 9, 2026, has more than 150 supporting organizations
- Since August 17, 2026, A2A is a hosted project of the Agentic AI Foundation

## Speaker Notes

The decision table in the topic comes down to two questions per interaction. Most flows in the Grey Logistics system should start as choreography, and only interactions that need a single answer should use A2A.

Answer now: a yes points to A2A, one named agent per call, with timeouts and retries in the caller and the task state in the callee. Each agent keeps its task records as the audit trail.

More consumers later: a yes points to events, where the publisher knows nothing about its consumers and new ones join without a change to it. Failures go through retries, dead-letter queues and idempotent consumers, and the event log is the audit trail.

Governance: A2A 1.0 is the first stable release, announced by the Linux Foundation on April 9, 2026 with more than 150 supporting organizations, and since August 17, 2026 it is a hosted project of the Agentic AI Foundation. Agents built on different platforms can rely on the same card format and the same bindings.
