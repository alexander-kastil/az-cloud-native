---
slide: "03-01"
topic: 03-state-stores
title: "ETags and first-write-wins"
subtitle: "A write succeeds only while the ETag still matches"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/07-dapr/_images/etag-first-write.svg"
media-file: labs/07-dapr/_images/etag-first-write.svg
visual-prompt: >
  Existing diagram of two reserve-capacity requests for one parcel writing to statestore with FirstWrite, where the first write wins and the second rereads and returns a conflict, no generation. Referenced at its repo path.
order: 7
source: demos/07-dapr/03-state-stores/readme.md
gamma_prompt: >
  Slide titled "ETags and first-write-wins", subtitle "A write succeeds only while the ETag still
  matches". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# ETags and first-write-wins

## Content

- `GetStateAndETagAsync` returns the value together with its **ETag**
- `TrySaveStateAsync` with `ConcurrencyMode.FirstWrite` returns `false` when another writer won
- The Redis state store throws a `DaprException` for the same conflict
- On a conflict, read again and recompute in a short loop
- After a fixed number of attempts, return a conflict to the caller

## Speaker Notes

A customer cancels a shipment while a scanner marks the same parcel as picked up. Without a rule for conflicts, the last write silently replaces the first.

Read with the ETag: the ETag is an opaque version token the store issues on every write. Keep it with the value you read, because the store compares it when you write again.

First write wins: a write that carries an ETag succeeds only while that ETag still matches the stored one. `TrySaveStateAsync` returns `false` when another writer got there first.

Redis behaves differently: the Redis state store throws a `DaprException` for the same conflict, so the caller catches it and treats it as a lost race.

The retry loop: the handler reads, computes the change and tries again. The diagram shows the lab's version, where a second `reserve-capacity` request for the same parcel rereads, finds the record and returns `Results.Conflict()` after at most three attempts.

Bounded attempts: after a fixed number of attempts the handler returns a conflict, so a customer request never loops forever.
