---
slide: "99-01"
topic: 06-event-sourcing-cqrs
title: "Takeaways"
subtitle: "What to carry into the Dapr module"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/06-event-sourcing-cqrs/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. At a light-oak packing table on the bright floor of a modern parcel hub, a female software
  engineer in her early thirties scans a plain brown cardboard parcel with a handheld scanner, while a
  male colleague in his late twenties holds a tablet up beside her with its display facing the camera, a
  softly blurred vertical list of rows where the newest row glows mint green. Both smile at the tablet. A
  few more plain parcels on the table, tall windows and green plants behind them. The two engineers are
  in the middle of the frame, faces fully visible. Modern, light, bright and friendly: high-key natural
  daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint and
  lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never readable
  UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos,
  watermarks, labels, barcodes or printed tape on the parcels, signs or floor markings with writing, the
  back of the tablet, extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 18
source: demos/06-event-sourcing-cqrs/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into the Dapr module". Side-accent layout:
  title, subtitle and five bullets on the left half on plain white, the provided photograph of two
  engineers scanning a parcel and watching the tracking list update filling the right half, no text over
  it. Use exactly the one image given for this card; do not generate or add any other images, icons or
  logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Commit the event and its outbox event in one batch, relay with duplicate detection
- Make every projection idempotent: upsert under the event ID, await every write
- Keep reads on one partition, and send session tokens back for read-your-writes
- Delete the cached page after the upsert, and let the TTL bound the rest
- Put the version in the event ID, and evolve schemas additively or by upcasting

## Speaker Notes

Close with the habits this module built, each one a decision about where an event goes and who reads it.

Outbox: one transactional batch on one partition key removes the dual write. MessageId plus duplicate detection drops the copies that a retried relay sends.

Idempotent projections: the change feed processor delivers at least once, so the projection upserts under the event ID. Awaiting every write keeps the checkpoint from running ahead of unfinished work.

Reads: point reads and single-partition queries keep the RU charge low, and the composite index matches the ops console sort. A session token sent back with the next request lets a user read their own write on another instance.

Cache: the projection deletes the Redis key only after the upsert, so a failed write never clears a page. A TTL with jitter bounds how long any stale page can stay.

Event store and versioning: a version in the event ID turns a conflicting append into 409 Conflict, and snapshots keep replay short. Schemas change additively or through upcasters, and Module 7 continues with the Dapr portability work.
