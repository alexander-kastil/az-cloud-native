---
slide: "04-02"
topic: 04-partitions-checkpoints
title: "Checkpoints and partition ownership"
subtitle: "A crashed instance's partitions resume from the last checkpoint"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/05-event-grid-hubs/04-partitions-checkpoints/_images/ownership-takeover.svg"
media-file: demos/05-event-grid-hubs/04-partitions-checkpoints/_images/ownership-takeover.svg
visual-prompt: >
  Existing diagram of two telemetry-ingest instances sharing the ops-console checkpoint store, instance B crashing before its checkpoint and instance A taking over partitions 2 and 3 from the last checkpoint, so event 8 of partition 2 is processed twice, no generation. Referenced at its repo path.
order: 11
source: demos/05-event-grid-hubs/04-partitions-checkpoints/readme.md
gamma_prompt: >
  Slide titled "Checkpoints and partition ownership", subtitle "A crashed instance's partitions resume
  from the last checkpoint". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Checkpoints and partition ownership

## Content

- `EventProcessorClient` stores ownership and checkpoints in a `BlobContainerClient`
- Instances claim partitions until the load is balanced
- After a crash, another instance takes over from the last checkpoint
- Delivery is **at least once**, so the handler must be idempotent
- **Capture** archives the stream to storage in Avro format

## Speaker Notes

The ops console reads van positions with the `EventProcessorClient`, and its checkpoint store decides where reading resumes. The diagram shows instance B crashing and instance A taking over its partitions.

Processor and checkpoint store: the processor writes ownership and checkpoint records to a blob container that must exist before it starts, so the lab calls `CreateIfNotExistsAsync` first. Register the error handler, and call `StopProcessingAsync` before the process exits so other instances can take the partitions over promptly.

Load balancing: each instance writes an ownership record and claims partitions until the load is balanced, and at most one processor reads a partition at any moment. The partition count caps the parallel readers in a group, so an instance beyond it stays idle.

Takeover: a crashed instance's records stop changing, and the others see the old last-modified time and take over from the last checkpoint. In the diagram, event 8 of partition 2 is processed a second time.

At least once: make the handler idempotent, for example by storing each event ID and skipping IDs it has already processed. A checkpoint after every event costs a blob write per event, while fewer checkpoints cost less and replay a few more events after a crash.

Capture and retention: Capture writes the stream to Blob Storage or Data Lake Storage in Avro format, and only for events that arrive after you enable it. Standard keeps events for up to 7 days, Premium and Dedicated for up to 90 days.
