---
slide: "03-01"
topic: 03-durable-llm-calls
title: "Why the model call belongs in an activity"
subtitle: "Replay reads the recorded draft"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/draft-activity-retry.svg"
media-file: labs/09-ai-agents/_images/draft-activity-retry.svg
visual-prompt: >
  Existing diagram of DelayedParcelWorker starting DelayedParcelOrchestration, which calls the DraftCustomerMessage activity with a five-attempt retry policy, a first call that returns HTTP 429 retried after 2 s, and the draft recorded in the history, no generation. Referenced at its repo path.
order: 8
source: demos/09-ai-agents/03-durable-llm-calls/readme.md
gamma_prompt: >
  Slide titled "Why the model call belongs in an activity", subtitle "Replay reads the recorded draft".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Why the model call belongs in an activity

## Content

- An orchestrator **replays its history** each time it resumes, so every call must return the same result
- A model returns different text for the same prompt, which breaks the first replay
- An **activity** runs once per attempt, and the runtime records its output in the history
- `CallActivityAsync` with `TaskOptions.FromRetryPolicy`: 5 attempts, 2 s first interval, backoff 2, 60 s maximum
- A waiting orchestration has no model call in flight, so a parcel can wait a day for a person

## Speaker Notes

Topic 03 gives the model call a durable home: the delayed-parcel workflow runs for hours, while a model call takes seconds, can be throttled and can fail after it was billed. The diagram is the lab's step 3, where the worker starts the orchestration and the activity calls contoso-chat.

Replay: an orchestrator replays its history each time it resumes, and every call must return the same result on replay. A model returns different text for the same prompt, so a model call in the orchestrator breaks determinism on the first replay.

The activity: an activity runs once for each attempt, and the runtime records its output in the history. On replay the orchestrator reads the recorded draft and never calls the model again.

Retry policy: the input carries the parcel id and the event id, and the policy allows 5 attempts with a 2-second first interval, a backoff coefficient of 2 and a 60-second maximum. The 5 counts the first call too, and an unbounded retry would keep the orchestration running with no clear failure.

Waiting for approval: after the draft the orchestrator publishes RerouteRequested, and the booking workflow waits for RerouteApproved or the deadline timer. A waiting orchestration has no model call in flight, and Module 8 teaches the external event pattern behind that wait.
