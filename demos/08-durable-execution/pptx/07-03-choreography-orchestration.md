---
slide: "07-03"
topic: 07-saga
title: "Choreography and orchestration"
subtitle: "The booking uses one orchestrator for its three services"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/07-saga/_images/choreography-vs-orchestration.svg"
media-file: demos/08-durable-execution/07-saga/_images/choreography-vs-orchestration.svg
visual-prompt: >
  Existing diagram of choreography, where the Capacity, Payment and Label services pass events with no coordinator, beside orchestration, where BookingSagaOrchestrator in booking-worker calls three activities and keeps the saga state, no generation. Referenced at its repo path.
order: 17
source: demos/08-durable-execution/07-saga/readme.md
gamma_prompt: >
  Slide titled "Choreography and orchestration", subtitle "The booking uses one orchestrator for its
  three services". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Choreography and orchestration

## Content

- In **choreography**, each service publishes events and reacts to the events of others
- It suits few services and simple flows, but grows hard to follow and to test end to end
- In **orchestration**, one controller calls each participant and stores the saga state
- It suits complex flows and growing service counts, and needs coordination logic
- The booking uses orchestration; the Module 6 tracking projection uses choreography

## Speaker Notes

The Architecture Center describes two ways to coordinate a saga, and this slide places the booking on one side of that choice.

Choreography: each service publishes events and reacts to the events of others, and no coordinator exists.

Its trade-offs: choreography fits few services and simple flows, with no coordinator and no single point of failure. As the steps grow it gets hard to follow, risks cyclic dependencies and is hard to test end to end.

Orchestration: one controller calls each participant and stores the state of the saga.

Its trade-offs: orchestration fits complex flows and growing service counts, with clear responsibilities and no cyclic dependencies. It needs coordination logic, and the orchestrator manages the whole flow.

The booking: it has three services, a fixed order and compensations that must run in reverse, so it suits orchestration. The tracking projection from Module 6 suits choreography, because each event feeds an independent read model.
