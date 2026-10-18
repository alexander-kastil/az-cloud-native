---
slide: "06-02"
topic: 06-event-store
title: "Replay and snapshots"
subtitle: "Load from the newest snapshot and apply only the events after it"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/06-event-sourcing-cqrs/06-event-store/_images/replay-snapshot.svg"
media-file: demos/06-event-sourcing-cqrs/06-event-store/_images/replay-snapshot.svg
visual-prompt: >
  Existing diagram of replay filling an empty Shipment aggregate from v1 ShipmentBooked onwards, and of loading 103 events from the snapshot at version 100 with only 3 Apply calls, no generation. Referenced at its repo path.
order: 14
source: demos/06-event-sourcing-cqrs/06-event-store/readme.md
gamma_prompt: >
  Slide titled "Replay and snapshots", subtitle "Load from the newest snapshot and apply only the events
  after it". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column,
  never cropped, never used as a background or side accent. Use exactly the one image given for this
  card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Replay and snapshots

## Content

- `LoadAsync` queries one partition in version order and calls `Apply` per event
- The aggregate reads nothing but its own events
- A **snapshot** stores the state at one version, in the same partition
- 103 events: snapshot 100, then 3 `Apply` calls instead of 103
- The snapshot ID encodes its version, so a duplicate create gets a 409 to ignore

## Speaker Notes

Current state comes from replaying the events in order. This slide shows the replay and how snapshots keep it short once a stream grows long.

Replay: ShipmentRepository queries the events of one shipment in version order, scoped to the partition key, and applies each one to an empty aggregate. Apply switches on the event type and returns a new aggregate for each event.

Own events only: the aggregate never reads the store for anything except its own events, so its behavior depends only on what it replays. The replay cost grows with the number of events.

Snapshot: a snapshot document stores the aggregate state at one version, with the same partition key. Learn calls it an optimization, because the event stream stays the source of truth and any snapshot can be rebuilt from it.

Loading from a snapshot: loading starts from the newest snapshot and replays only the events after it, with a version filter in the query. In the diagram the Shipment loads from the snapshot at version 100 and applies only v101 to v103.

Choosing N: the writer creates a snapshot whenever the version reaches a multiple of N, such as 100, and a second writer that creates the same snapshot gets a 409 Conflict it can ignore. A small N stores more snapshots and keeps replay short, while a large N stores fewer and makes replay longer.
