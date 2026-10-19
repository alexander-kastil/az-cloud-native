---
slide: "02-03"
topic: 02-change-feed-processor
title: "Latest version or all versions and deletes"
subtitle: "What each change feed mode returns from the shipments container"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/06-event-sourcing-cqrs/02-change-feed-processor/_images/change-feed-modes.svg"
media-file: demos/06-event-sourcing-cqrs/02-change-feed-processor/_images/change-feed-modes.svg
visual-prompt: >
  Existing diagram of two change feed reads of partition SHP-10482 in latest version mode and in all versions and deletes mode, with the TTL delete of the outbox event returned only by the second, no generation. Referenced at its repo path.
order: 7
source: demos/06-event-sourcing-cqrs/02-change-feed-processor/readme.md
gamma_prompt: >
  Slide titled "Latest version or all versions and deletes", subtitle "What each change feed mode returns
  from the shipments container". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Latest version or all versions and deletes

## Content

- **Latest version** is the default and returns the newest state of each changed item
- It has no fixed retention and can start from the beginning, but it misses deletes
- **All versions and deletes** records every create, replace and delete, TTL included
- It needs continuous backup and starts only from now or a checkpoint
- The shipment audit needs deletes, so it uses `GetChangeFeedProcessorBuilderWithAllVersionsAndDeletes`

## Speaker Notes

The processor reads the change feed in one of two modes, and the mode decides which changes it ever sees. The diagram shows two reads of partition SHP-10482, the second one ten days after the booking.

Latest version: the default mode returns the newest state of each changed item and can skip intermediate updates between two reads. The event documents are created once and never replaced, so this mode skips none of them.

Retention and start: latest version has no fixed retention and can start from the beginning, a point in time, now or a checkpoint. It doesn't capture deletes, so the TTL delete of the outbox event never comes back.

All versions and deletes: this mode records each create, replace and delete, including deletes caused by TTL. In the diagram, read 2 returns the delete of the outbox event by TTL.

Requirements: the mode needs continuous backup on the account and the all versions and deletes feature enabled, and its retention equals the continuous backup window. It starts only from now or a saved continuation token and needs .NET SDK 3.60.0 or later.

Shipment audit: the audit needs deletes, so it builds its processor with GetChangeFeedProcessorBuilderWithAllVersionsAndDeletes. Its handler reads change.Previous for a delete and change.Current for every other operation.
