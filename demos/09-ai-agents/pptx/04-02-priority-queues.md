---
slide: "04-02"
topic: 04-rate-limits
title: "Priority queues for urgent and bulk work"
subtitle: "Three layouts and their starvation risk"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/09-ai-agents/04-rate-limits/_images/priority-queues.svg"
media-file: demos/09-ai-agents/04-rate-limits/_images/priority-queues.svg
visual-prompt: >
  Existing diagram of three priority queue layouts fed by booking-worker, one queue with a priority value, an urgent and a bulk queue with one consumer pool marked as the lab's layout, and one pool per queue, with the starvation risk of each, no generation. Referenced at its repo path.
order: 11
source: demos/09-ai-agents/04-rate-limits/readme.md
gamma_prompt: >
  Slide titled "Priority queues for urgent and bulk work", subtitle "Three layouts and their starvation
  risk". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card;
  do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Priority queues for urgent and bulk work

## Content

- **Urgent** exceptions need a draft within minutes; **bulk** sweeps can wait an hour
- One queue with a priority value: the risk depends on the broker and the consumer logic
- Several queues with one consumer pool: low-priority messages can wait indefinitely
- One pool per queue: each drains at its own pace, with low starvation risk
- The lab uses the middle layout, so a bulk backlog can still take every replica

## Speaker Notes

Two kinds of work reach the agent, and the Priority Queue pattern makes sure urgent work reaches the model first. The layout decides whether low-priority work can starve.

Two priorities: an urgent exception, such as a parcel with a medical or time-critical label, needs a draft within minutes. A bulk exception, such as a weekly sweep of delayed parcels for analytics, can wait an hour.

One queue with a priority value: consumers take the highest priority message first, and the starvation risk depends on the broker and the consumer logic.

Several queues: with one consumer pool, consumers drain the high queue before the low queue, and low-priority messages can wait indefinitely. With one pool per queue, each pool drains its own queue at its own pace and has its own capacity.

The lab: its replicas form one pool, the middle layout, so a bulk backlog can still scale every replica, and a started bulk orchestration competes with urgent calls for the same activity slots. The guidance is to give each priority its own queue and its own scaling rule.
