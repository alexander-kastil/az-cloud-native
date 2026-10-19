---
slide: "02-03"
topic: 02-agent-subscribers
title: "Idempotency for agent consumers"
subtitle: "Check the processed marker before the model call"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/processed-check-batch.svg"
media-file: labs/09-ai-agents/_images/processed-check-batch.svg
visual-prompt: >
  Existing diagram of DelayedParcelOrchestration checking the processed marker in agent-state, writing the reroute proposal and the marker in one transactional batch, then publishing RerouteRequested, no generation. Referenced at its repo path.
order: 7
source: demos/09-ai-agents/02-agent-subscribers/readme.md
gamma_prompt: >
  Slide titled "Idempotency for agent consumers", subtitle "Check the processed marker before the model
  call". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card;
  do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Idempotency for agent consumers

## Content

- Brokers deliver **at least once**: duplicate detection does not stop a redelivery after a lock expires
- `ReadProcessedDraft` checks agent-state for the event id **before** the model call
- A retry reuses the stored draft, because a second model call words it differently
- `SaveRerouteProposal` writes the `reroute-` proposal and the processed marker in one transactional batch
- `RerouteRequested` goes to parcel-events only after the batch succeeds

## Speaker Notes

The broker can deliver DeliveryDelayed twice, and a model-driven consumer has to handle that without a second draft or a second proposal. The diagram is the lab's step 4, the orchestration with its processed check and its batch.

At least once: Service Bus duplicate detection suppresses repeated sends inside its history window, but not a redelivery after a lock expires, and Event Hubs checkpoints can replay a range after a restart. The idempotent consumer pattern handles both by recording what it has already processed.

Check first: the order of the steps decides the outcome, so the agent looks up the processed marker keyed by the event id and only then calls the model. Model output is not deterministic, so a retry must reuse the stored draft.

One batch: the reroute proposal, keyed reroute- plus the event id in the parcel's partition, and the processed marker go into one transactional batch, so a crash cannot leave a marker without its proposal. A duplicate delivery upserts the same proposal and creates no second one.

Publish after the batch: RerouteRequested goes to parcel-events once the batch succeeds. Module 6 shows the same pattern with the transactional outbox.
