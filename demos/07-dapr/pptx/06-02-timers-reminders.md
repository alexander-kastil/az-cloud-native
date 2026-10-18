---
slide: "06-02"
topic: 06-actors
title: "Timers and reminders"
subtitle: "A heartbeat while the actor is active, a service check that survives deactivation"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/07-dapr/06-actors/_images/timers-vs-reminders.svg"
media-file: demos/07-dapr/06-actors/_images/timers-vs-reminders.svg
visual-prompt: >
  Existing diagram of van-42 over time, where the heartbeat timer ticks only while the actor is active and the van-service-check reminder stays in the state store and reactivates the actor, no generation. Referenced at its repo path.
order: 15
source: demos/07-dapr/06-actors/readme.md
gamma_prompt: >
  Slide titled "Timers and reminders", subtitle "A heartbeat while the actor is active, a service check
  that survives deactivation". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Timers and reminders

## Content

- `RegisterTimerAsync`: a heartbeat every 5 s, only while the actor is active
- `RegisterReminderAsync`: `van-service-check` every 24 h, persisted in the state store
- The runtime calls `ReceiveReminderAsync` for every reminder that fires
- Reminders cost more than timers, because the runtime writes them to storage
- Container Apps does not run the actor packages: use the AppHost or AKS

## Speaker Notes

Actors can schedule their own work, and the choice between a timer and a reminder decides whether that work survives deactivation.

Timers: a timer calls back into the actor on a schedule and lives only while the actor is active. That suits a heartbeat check on an active van, which can restart on the next activation.

Reminders: a reminder is persisted through the actor state provider and survives deactivation and restarts. `VanActor` registers `van-service-check` with a due time of one minute and a period of 24 hours, a daily service check that must fire after hours of idle time.

Callbacks: the callback named in `RegisterTimerAsync` must match a method on the same class, and the runtime invokes `ReceiveReminderAsync` for every reminder that fires.

Cost: reminders cost more than timers because the runtime writes them to storage, and in Container Apps they need at least one replica to fire reliably.

Hosting rule: Container Apps states that the Dapr actor and workflow SDK packages are not compatible with it. Run `VanActor` locally with the Aspire AppHost or on AKS with the Dapr extension.
