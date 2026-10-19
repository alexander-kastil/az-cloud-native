---
slide: "07-02"
topic: 07-event-versioning
title: "CloudEvents type and dataschema"
subtitle: "Consumer rules for each kind of schema change"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/07-02-cloudevents-envelope.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif, Consolas for attribute names and values, module palette, minimum 26px type. No heading
  inside the image. No arrow crosses a box or a label. Left two thirds: a card "CloudEvents 1.0 envelope"
  listing the readme example, required context attributes id, source /grey/logistics/shipping-api,
  specversion 1.0, type com.grey.logistics.shipment.booked; optional attributes datacontenttype
  application/json, dataschema https://schemas.grey.example/shipment-booked/2.0.json, subject
  shipments/SHP-10482, time 2026-10-18T08:00:00Z; then data with shipmentId, customerId, promisedBy,
  serviceLevel. The id and source rows are highlighted green, type blue, dataschema purple. Right third:
  matching callouts joined by short arrows, green "source and id: identify the event, the deduplication
  key of the idempotent consumer", blue "type: stays the same across versions", purple "dataschema:
  changes with each version, so the reader picks the right deserializer", and below it a dashed grey box
  "unknown dataschema: dead-letter the message".
order: 16
source: demos/06-event-sourcing-cqrs/07-event-versioning/readme.md
gamma_prompt: >
  Slide titled "CloudEvents type and dataschema", subtitle "Consumer rules for each kind of schema change".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# CloudEvents type and dataschema

## Content

- `type` stays the same across versions, `dataschema` changes with each one
- `source` plus `id` is the idempotent consumer's deduplication key
- Adding an optional field is safe for consumers that already run
- A rename writes both names for one release; a new meaning gets a new `type` or `dataschema`
- A consumer dead-letters a message whose `dataschema` it doesn't know

## Speaker Notes

Consumers and producers deploy at different times, so each event has to tell its reader which shape it carries. CloudEvents 1.0 gives every event a standard envelope, with id, source, specversion and type as the required context attributes.

Type and dataschema: type describes the kind of event and stays the same across versions, so a subscriber that handles every booking still matches. dataschema identifies the schema of the data and changes with each version, so a reader can pick the right deserializer.

Deduplication key: the source and id pair identifies the event. The idempotent consumer from the first topic uses that pair as its deduplication key.

Safe changes: adding an optional field is safe, because consumers ignore unknown fields. A new required field starts as optional and becomes required only in a later version.

Renames, removals and new meanings: a rename writes both names for one release before the old name goes, and a removed field is first no longer written but still read until consumers move on. A field that changes meaning gets a new event type or a new dataschema.

Unknown schema: a consumer that receives a dataschema it doesn't know should dead-letter the message. Module 4 covers the dead-letter queue that holds such messages for review.
