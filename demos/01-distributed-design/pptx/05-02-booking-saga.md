---
slide: "05-02"
topic: 05-reliability-patterns
title: "The booking saga"
subtitle: "Local transactions with compensations in reverse order"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/07-saga/_images/saga-compensation.svg"
media-file: demos/08-durable-execution/07-saga/_images/saga-compensation.svg
visual-prompt: >
  Existing diagram of the booking saga, where ReserveCapacity and CapturePayment complete, PrintLabel fails after three attempts, and the compensations RefundPayment and ReleaseCapacity run in reverse order, no generation. Referenced at its repo path.
order: 14
source: demos/01-distributed-design/05-reliability-patterns/readme.md
gamma_prompt: >
  Slide titled "The booking saga", subtitle "Local transactions with compensations in reverse order".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The booking saga

## Content

- A **saga** replaces one distributed transaction with local transactions
- Forward steps: ReserveCapacity, CapturePayment, PrintLabel
- **Compensations** release the capacity, refund the payment and void the label
- Compensations run in the reverse order of the forward steps
- Choreography fits a short flow, **orchestration** a growing one

## Speaker Notes

The saga covers the failure the outbox cannot: a step that fails after other services have committed their steps. The diagram shows the Contoso booking saga when PrintLabel fails.

Local transactions: each step commits in its own service, and when a later step fails, compensating transactions undo the earlier ones.

Forward steps: the booking saga reserves capacity, captures the payment and prints the label.

Compensations: ReleaseCapacity, RefundPayment and VoidLabel undo the three forward steps one by one.

Reverse order: in the diagram PrintLabel fails after three attempts, so the saga runs RefundPayment first and ReleaseCapacity second.

Choreography or orchestration: in choreography each service reacts to the others' events with no coordinator, which suits a short flow with few services. An orchestrator sends commands and tracks each step, which suits a growing flow or one whose state someone must see at any moment.
