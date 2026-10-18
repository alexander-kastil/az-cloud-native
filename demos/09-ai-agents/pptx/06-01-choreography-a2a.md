---
slide: "06-01"
topic: 06-agent-choreography
title: "Choreography and A2A"
subtitle: "Published events or a direct call to one agent"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/09-ai-agents/06-agent-choreography/_images/choreography-vs-a2a.svg"
media-file: demos/09-ai-agents/06-agent-choreography/_images/choreography-vs-a2a.svg
visual-prompt: >
  Existing diagram of choreography and A2A side by side, booking-worker publishing DeliveryDelayed through the broker to the exception-agent, and the ops console reading the exception-agent agent card and sending one task, no generation. Referenced at its repo path.
order: 15
source: demos/09-ai-agents/06-agent-choreography/readme.md
gamma_prompt: >
  Slide titled "Choreography and A2A", subtitle "Published events or a direct call to one agent".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Choreography and A2A

## Content

- **Choreography**: a producer publishes a fact, and every interested service subscribes
- Guard against event storms with event filters, consumer concurrency limits and explicit rules
- **A2A**: the callee publishes an agent card at `/.well-known/agent-card.json`
- A2A 1.0 defines JSON-RPC 2.0, gRPC and HTTP with JSON bindings, and permits custom ones
- The caller owns the timeout, the callee owns the task state

## Speaker Notes

Topic 06 asks how agents coordinate once a delayed parcel touches several services: by reacting to published events, or by one agent calling another and waiting for the answer. The diagram puts both styles side by side, with the parts the lab does not build tagged design.

Choreography: booking-worker publishes DeliveryDelayed, the exception-agent reacts, and no central component tells each service what to do next. Add a reporting consumer next quarter, and the publisher changes nothing.

Guardrails: choreography brings duplicate and out-of-order messages, schema changes that break consumers, and event storms, where one event triggers a chain of reactions. Event filters, consumer concurrency limits and explicit rules keep those chains in check.

Agent cards: the callee publishes a card at /.well-known/agent-card.json with its name, skills, interfaces and accepted authentication. A caller reads the card, picks one of the advertised bindings, JSON-RPC 2.0, gRPC or HTTP with JSON, and sends a task.

Ownership: A2A fits when the caller needs one answer and waits, such as the ops console asking the exception-agent to explain one delay. The caller owns the timeout, and the callee owns the task state.
