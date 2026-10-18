---
slide: "07-01"
topic: 07-event-versioning
title: "Upcasting on read"
subtitle: "Stored events stay untouched, readers see the newest shape"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/07-01-upcasting.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif, module palette (events orange #FFEDD5 with #C2410C, services blue #DBEAFE with #1D4ED8,
  subscription amber #FEF3C7 with #B45309, store border dashed green #15803D), minimum 26px type. No
  heading inside the image. No arrow crosses a box or a label. Left: dashed group "event store, stored
  events untouched" holding two event cards, ShipmentBooked "schemaVersion 1, no serviceLevel" and
  ShipmentBooked "schemaVersion 2, serviceLevel express". Both arrow into the box "ShipmentBookedUpcaster"
  (on the read path, v1 gets serviceLevel = standard, every read runs the upcaster), which arrows into the
  card "ShipmentBookedV2". From ShipmentBookedV2 one line drops to a bus that feeds three boxes in a row:
  Replay, Projections, exception-agent-subscription.
order: 15
source: demos/06-event-sourcing-cqrs/07-event-versioning/readme.md
gamma_prompt: >
  Slide titled "Upcasting on read", subtitle "Stored events stay untouched, readers see the newest shape".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Upcasting on read

## Content

- Past events are **immutable**: correct them with a compensating event or an upcaster
- An **additive** change adds an optional field, and tolerant readers ignore unknown fields
- A missing `serviceLevel` deserializes as null, and the reader applies `standard`
- `ShipmentBookedUpcaster` turns `schemaVersion` 1 into `ShipmentBookedV2` on read
- Upcasters chain one version per step, and every read runs them

## Speaker Notes

ShipmentBooked events have sat in the event store since the first release, and the business now wants a serviceLevel on every booking. Old events can't change, and consumers deployed last year still read them.

Immutable history: a bug in an old event stays in the store, and a fix in the code doesn't repair it. Learn offers a compensating event or an upcaster, and lists in-place migration as the last resort because rewriting history breaks the audit trail.

Additive change: an optional new field keeps every existing consumer working, because tolerant readers ignore fields they don't know and default the missing ones. System.Text.Json ignores unknown properties by default.

Defaults: a missing serviceLevel deserializes as null, and the reader applies standard. While the new field is optional, the reader needs no second class for the old shape.

Upcasting: when a change can't be additive, the envelope carries a schemaVersion and the reader upcasts each stored event to the newest shape. ShipmentBookedUpcaster gives version 1 events serviceLevel standard, so replay, projections and exception-agent-subscription only see ShipmentBookedV2.

Cost: upcasting leaves the stored event untouched, so the history stays immutable. Upcasters can chain one version change per step, but every read runs them, so a long chain slows replay.
