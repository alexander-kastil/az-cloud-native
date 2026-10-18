---
slide: "07-01"
topic: 07-saga
title: "The booking saga and its compensations"
subtitle: "Completed steps are undone in reverse order"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/07-saga/_images/saga-compensation.svg"
media-file: demos/08-durable-execution/07-saga/_images/saga-compensation.svg
visual-prompt: >
  Existing diagram of the booking saga pushing ReleaseCapacity and RefundPayment onto a compensation stack, PrintLabel failing after three attempts, and the stack running RefundPayment before ReleaseCapacity, no generation. Referenced at its repo path.
order: 15
source: demos/08-durable-execution/07-saga/readme.md
gamma_prompt: >
  Slide titled "The booking saga and its compensations", subtitle "Completed steps are undone in reverse
  order". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card; do
  not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The booking saga and its compensations

## Content

- Capacity, payment and the label each change their own database, so no single transaction covers them
- Each forward step has one compensation: `ReleaseCapacity`, `RefundPayment`, `VoidLabel`
- The orchestrator pushes a compensation onto a stack as each step completes
- A `TaskFailedException` pops the stack, so the refund runs before the capacity release
- A compensation that throws fails the orchestration, so make each one retryable

## Speaker Notes

Contoso books a shipment across three services, and if printing the label fails, the company must release the capacity and refund the payment. The saga pattern does this with a sequence of local transactions and a compensating action for each one.

Separate databases: capacity, payment and the label each change state in their own database, so no single transaction covers all three. The system still has to reach a consistent state when a step fails halfway.

Pairs: ReleaseCapacity reverses ReserveCapacity, RefundPayment reverses CapturePayment, and VoidLabel reverses PrintLabel.

The stack: the orchestrator records each compensation on a Stack<Func<Task>> as its forward step succeeds, so the last one pushed runs first.

Failure at PrintLabel: a TaskFailedException from any forward step pops the stack, so RefundPayment runs before ReleaseCapacity and the orchestrator returns compensated. VoidLabel never runs in this booking, because PrintLabel is the last forward step.

Retryable compensations: a compensation that throws fails the orchestration, so design each compensation to be retryable as well. Compensations can still fail, so alert in the dashboard on any orchestration that ends compensated or failed.
