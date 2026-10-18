---
slide: "90-01"
topic: 08-durable-execution
title: "Orchestrate the booking saga"
subtitle: "What the lab builds in nine steps"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/lab-end-state.svg"
media-file: labs/08-durable-execution/_images/lab-end-state.svg
visual-prompt: >
  Existing diagram of the lab end state: the AppHost with the scheduler emulator, the bookings task hub and booking-worker with both orchestrators, plus BookingFunctions, Azurite and booking-worker replicas on Container Apps, each marked with the number of the step that adds it, no generation. Referenced at its repo path.
order: 18
source: labs/08-durable-execution/readme.md
gamma_prompt: >
  Slide titled "Orchestrate the booking saga", subtitle "What the lab builds in nine steps". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate or
  add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Orchestrate the booking saga

## Content

- Steps 1 to 4: the saga on the emulator, its compensations, safe retries and a worker restart
- Steps 5 and 6: a delivery deadline timer and the customer's reroute approval
- Step 7: the same saga as Durable Functions on the `azureManaged` provider
- Step 8: the Durable Task analyzer in both projects
- Step 9: a KEDA scale rule for `booking-worker` on Container Apps

## Speaker Notes

Today the booking runs as plain service calls, so a crash between two calls can charge a customer and never print a label. The lab turns it into a durable saga in about 70 minutes, and the diagram marks each part with the step that adds it.

Saga and restart: steps 1 to 4 host booking-worker on the scheduler emulator, write BookingSagaOrchestrator with six activities and their compensations, add a retry policy and idempotency checks, then restart the worker in the middle of a saga. The saga resumes from its stored history.

Deadline and approval: steps 5 and 6 add DeliveryDeadlineOrchestrator, which raises DeliveryDelayed when the window closes and then waits up to 24 hours for RerouteApproved.

Durable Functions: step 7 adds BookingFunctions, a standalone copy of the saga that reaches the same emulator through the azureManaged provider, with Azurite beside it.

Analyzer: step 8 turns on the Durable Task analyzer in both projects, so the build reports replay-unsafe calls and ends with zero DURABLE warnings.

KEDA on Container Apps: step 9 deploys booking-worker with a managed identity and the azure-durabletask-scheduler rule, so replicas follow the activity backlog. Run provision.azcli before step 1, because scheduler creation can take up to 15 minutes.
