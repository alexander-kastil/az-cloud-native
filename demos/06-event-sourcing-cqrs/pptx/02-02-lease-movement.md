---
slide: "02-02"
topic: 02-change-feed-processor
title: "Leases and instances"
subtitle: "Stable instance names and three intervals decide how work moves"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/06-event-sourcing-cqrs/02-change-feed-processor/_images/lease-movement.svg"
media-file: demos/06-event-sourcing-cqrs/02-change-feed-processor/_images/lease-movement.svg
visual-prompt: >
  Existing diagram of four leases of tracking-projection moving between replica-0 and replica-1 with the 13 s acquire, 17 s renew and 60 s expiration intervals, no generation. Referenced at its repo path.
order: 6
source: demos/06-event-sourcing-cqrs/02-change-feed-processor/readme.md
gamma_prompt: >
  Slide titled "Leases and instances", subtitle "Stable instance names and three intervals decide how work
  moves". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card; do
  not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Leases and instances

## Content

- Each running copy of the service is one **instance** of the processor
- A lease belongs to one instance at a time, and leases rebalance as instances join or stop
- Give every replica a stable `WithInstanceName` instead of a new GUID per start
- Acquire 13 s, renew 17 s, expiration 60 s: the Azure Functions trigger defaults

## Speaker Notes

With the projection running, the next question is how it scales. The diagram follows the four leases of tracking-projection between replica-0 and replica-1.

Instances: instances share the lease container, and each lease belongs to one instance at a time. When you add an instance, the processor rebalances the leases across the group.

Instance names: every instance needs a unique name, set with WithInstanceName. A GUID meets that rule, but a restarted instance then gets a new name and leaves its old leases behind until they expire.

Three intervals: the acquire interval sets how often an instance checks that leases are spread evenly, and the renew interval how often a live instance renews its leases. The expiration interval sets how long a lease stays with an instance that stops renewing it.

In the diagram: each 13 s acquire check of replica-1 takes one lease, and after replica-1 stops, its two leases expire 60 s after their last renew and return to replica-0. These values match the Azure Functions trigger defaults.
