---
slide: "04-02"
topic: 04-sessions
title: "Session state and session locks"
subtitle: "A checkpoint that survives restarts and a lock renewed while the handler works"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/04-service-bus/pptx/images/04-02-session-state.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, in the module's diagram palette: background
  #F8FAFC, 'Segoe UI', Inter, Arial, sans-serif, Consolas for API names, text #0F172A, secondary
  #64748B, arrows #334155 3px, minimum 26px type. No heading inside the image; the slide title carries
  it. No arrow crosses a box or a label. Top row, left to right: a white box with #B45309 border
  "PKG-100482, session on tracking" holding three amber #FEF3C7 scans "1 PickedUp", "2 AtHub", "3
  OutForDelivery"; two blue #DBEAFE boxes stacked, "processor 1: applies PickedUp and AtHub,
  SetSessionStateAsync" and "processor 2: after a restart, GetSessionStateAsync, resumes with
  OutForDelivery"; a purple #EDE9FE box "session state" with the value "AtHub, the last scan applied"
  and the lines "survives processor restarts", "up to 256 KB on Standard, 100 MB on Premium", "stays
  until you clear it with an empty value", "counts toward the entity's storage quota". Arrow "writes"
  from processor 1 to the state, arrow "reads" from the state to processor 2. Bottom row, captioned
  "session lock, one receiver holds it at a time", three boxes joined by arrows: blue "renewed while the
  handler works, MaxAutoLockRenewalDuration, 5 minutes by default"; green #DCFCE7 "settled while the
  lock is held, keep the renewal running while the handler works"; grey "SessionIdleTimeout releases a
  quiet session, so other receivers take other sessions sooner".
order: 10
source: demos/04-service-bus/04-sessions/readme.md
gamma_prompt: >
  Slide titled "Session state and session locks", subtitle "A checkpoint that survives restarts and a
  lock renewed while the handler works". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect
  ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Session state and session locks

## Content

- `ServiceBusSessionProcessor` works on up to `MaxConcurrentSessions` sessions, 8 by default
- **Session state** is a checkpoint that survives processor restarts
- The processor renews the lock up to `MaxAutoLockRenewalDuration`, 5 minutes by default
- `SessionIdleTimeout` closes a quiet session, so the processor moves on to another
- Sessions need the Standard or Premium tier

## Speaker Notes

The previous slide showed how sessions keep order. This one covers the processor that receives them, the state a session carries and the lock that guards it.

Session processor: `ServiceBusSessionProcessor` delivers each session's messages in the order they were sent, and different sessions run in parallel. Microsoft Learn names a very high `MaxConcurrentSessions` relative to the core count as a frequent cause of thread starvation.

Session state: `SetSessionStateAsync` stores a small value and `GetSessionStateAsync` reads it, so a new processor can resume from the last step a parcel reached. The state holds up to 256 KB on Standard and 100 MB on Premium and stays until you clear it with an empty value.

Lock renewal: the processor renews the session lock while it works, up to `MaxAutoLockRenewalDuration`. If the lock expires, the session fails with `SessionLockLost` and another receiver can take it, so keep the renewal running until the handler settles.

Idle timeout: `SessionIdleTimeout` closes a session when the processor waits that long for its next message. Set it low for sparse sessions and high for parcels whose scans arrive over hours.

Tier: the Basic tier supports queues only, and Standard adds topics, subscriptions, sessions, transactions and duplicate detection. Premium adds dedicated messaging units, larger messages and private networking, so plan the tier before you create the namespace.
