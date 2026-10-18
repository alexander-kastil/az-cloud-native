---
slide: "07-02"
topic: 07-saga
title: "Pivot, retryable and compensable steps"
subtitle: "The booking has no pivot today"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/07-saga/_images/saga-step-roles.svg"
media-file: demos/08-durable-execution/07-saga/_images/saga-step-roles.svg
visual-prompt: >
  Existing diagram of ReserveCapacity, CapturePayment and PrintLabel as compensable steps reversed by ReleaseCapacity, RefundPayment and VoidLabel, followed by a message to a carrier as a later pivot and retryable steps after it, no generation. Referenced at its repo path.
order: 16
source: demos/08-durable-execution/07-saga/readme.md
gamma_prompt: >
  Slide titled "Pivot, retryable and compensable steps", subtitle "The booking has no pivot today".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Pivot, retryable and compensable steps

## Content

- A **compensable** step can be undone by a transaction with the opposite effect
- The **pivot** is the point of no return, after which compensations no longer apply
- A **retryable** step follows the pivot and must succeed eventually, so it has to be idempotent
- All three booking steps are compensable, so the booking has no pivot
- A message to a carrier would become the pivot, and the steps after it would need retries

## Speaker Notes

The Architecture Center sorts saga steps into three roles, and mapping the booking onto them comes before writing the orchestrator.

Compensable: another transaction with the opposite effect can undo the step.

Pivot: the pivot is the point of no return, after which compensations no longer apply.

Retryable: a step after the pivot must succeed eventually, so it has to be idempotent.

The booking today: ReserveCapacity, CapturePayment and PrintLabel are compensable, because ReleaseCapacity, RefundPayment and VoidLabel reverse them. The booking has no irreversible step, so it has no pivot.

A future pivot: a later step that cannot be recalled, such as a message to a carrier, would become the pivot, and the steps after it would need retries.
