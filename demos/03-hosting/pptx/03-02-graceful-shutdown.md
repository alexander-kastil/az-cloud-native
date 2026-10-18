---
slide: "03-02"
topic: 03-revisions
title: "Graceful shutdown for message consumers"
subtitle: "In-flight messages settle before SIGKILL"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/03-hosting/03-revisions/_images/shutdown-timeline.svg"
media-file: demos/03-hosting/03-revisions/_images/shutdown-timeline.svg
visual-prompt: >
  Existing diagram of the shutdown timeline from SIGTERM, with the processor stopping intake, two in-flight handlers settling and the host exiting inside the 25 s ShutdownTimeout and the 60 s grace period, no generation. Referenced at its repo path.
order: 9
source: demos/03-hosting/03-revisions/readme.md
gamma_prompt: >
  Slide titled "Graceful shutdown for message consumers", subtitle "In-flight messages settle before
  SIGKILL". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Graceful shutdown for message consumers

## Content

- A change to the **template** creates a revision; app-wide settings do not
- SIGTERM, then `terminationGracePeriodSeconds` (default 30 s), then SIGKILL
- Set the grace period longer than `HostOptions.ShutdownTimeout`
- `StopProcessingAsync` stops intake and waits for in-flight handlers
- Settle each message before the handler returns

## Speaker Notes

Every new revision deactivates an old one, and a message consumer must not lose the messages it is handling at that moment.

What creates a revision: a change to the template, which holds the container image, the environment variables, the resources and the scale rules. Secrets, the revision mode, ingress, traffic weights and labels apply to every revision and don't create one.

The shutdown signal: when a replica scales in, the app is deleted or a revision deactivates, the platform sends SIGTERM. After the termination grace period, 30 seconds by default, it sends SIGKILL.

Two timeouts: `HostOptions.ShutdownTimeout` sets how long the generic host waits in `StopAsync`, also 30 seconds by default. The diagram sets 25 s inside a 60 s grace period, so the host finishes before the platform kills the replica.

Draining: the consumer's `StopAsync` calls `StopProcessingAsync`, which stops intake and waits for the in-flight message handlers to complete.

Settlement: an unsettled message returns to the queue when its lock expires, and its delivery count goes up. Call `AbandonMessageAsync` for a message the handler can't finish, so the next receiver gets it at once.
