---
slide: "01-01"
topic: 01-durable-execution
title: "How replay rebuilds state"
subtitle: "The event history lets a second worker finish the booking"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/08-durable-execution/01-durable-execution/_images/replay.svg"
media-file: demos/08-durable-execution/01-durable-execution/_images/replay.svg
visual-prompt: >
  Existing diagram of Worker 1 crashing after CapturePayment and Worker 2 replaying the event history before it schedules PrintLabel, no generation. Referenced at its repo path.
order: 3
source: demos/08-durable-execution/01-durable-execution/readme.md
gamma_prompt: >
  Slide titled "How replay rebuilds state", subtitle "The event history lets a second worker finish the
  booking". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# How replay rebuilds state

## Content

- Every scheduled activity, completed activity, timer and external event lands in the instance's **event history**
- A worker runs the orchestrator from the first line, and each `await` with a recorded result returns it at once
- After a crash, the next worker replays the history and schedules only `PrintLabel`, so the customer pays once
- Local variables come back from the history; the code never writes them to a database
- Between checkpoints the orchestrator leaves memory, so a long wait holds no worker

## Speaker Notes

A parcel booking touches capacity, payment and label services, and a plain function call loses its progress when the process crashes after the payment. This slide shows how durable execution stores each completed step, so the code resumes after the last step it finished.

Event history: the runtime appends an event for every scheduled activity, completed activity, timer and external event. The Durable Task Scheduler dashboard shows them as TaskScheduled, TaskCompleted, TimerCreated and EventRaised.

Replay from the first line: a worker that picks up an instance runs the orchestrator code from the top, and each await on a durable task receives its recorded result. The first await without a recorded result is the point where the instance waits.

The crash case: if the worker crashes after CapturePayment completes, the next worker receives the stored results for both earlier calls and schedules PrintLabel. The customer pays once, and the label prints on the second attempt.

Event sourcing: the Durable Functions docs describe this approach as event sourcing. Local variables survive a restart because the runtime rebuilds them from the history.

No memory while waiting: between checkpoints the runtime removes the orchestrator from memory, so a long wait for a customer decision holds no worker memory. For a stuck instance, the last completed event shows which step is blocked.
