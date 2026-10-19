---
slide: "02-01"
topic: 02-change-feed-processor
title: "Building the tracking projection"
subtitle: "Processor name, delegate, lease container and the estimator"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/02-01-processor-builder.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif, Consolas for API names, module palette (green #DCFCE7 with #15803D for containers, blue
  #DBEAFE with #1D4ED8 for the processor, amber #FEF3C7 with #B45309 for the delivery note), minimum 26px
  type. No heading inside the image. No arrow crosses a box or a label. Top row left to right: cylinder
  "shipments" (Cosmos DB, partition /shipmentId, ParcelScanned events), arrow "change feed" into a large
  box "tracking-projection" (GetChangeFeedProcessorBuilder; delegate HandleChangesAsync, instance name
  WithInstanceName, lease container WithLeaseContainer; amber row "at-least-once delivery: a throw
  re-reads the same batch"), arrow "UpsertItemAsync" into cylinder "tracking" (Cosmos DB, read model,
  ScanRecord) with the note "id = event id, so a repeat writes the same record". Below the processor an
  arrow "checkpoint" into cylinder "leases" (partition key /id, position and owner per partition range),
  an arrow from leases to box "change feed estimator" (GetChangeFeedEstimatorBuilder, same name and lease
  container, pending changes every minute). Bottom left a dashed box "Azure Functions option"
  (CosmosDBTrigger, same processor machinery, the platform owns scaling).
order: 5
source: demos/06-event-sourcing-cqrs/02-change-feed-processor/readme.md
gamma_prompt: >
  Slide titled "Building the tracking projection", subtitle "Processor name, delegate, lease container and
  the estimator". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column,
  never cropped, never used as a background or side accent. Use exactly the one image given for this
  card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Building the tracking projection

## Content

- `GetChangeFeedProcessorBuilder` takes a processor name and a delegate
- The **lease container** (partition key `/id`) stores each range's position and owner
- Delivery is **at least once**: a delegate that throws reads the same batch again
- The event ID is the ScanRecord ID, so a repeated upsert writes the same record
- The **change feed estimator** reports pending changes; `CosmosDBTrigger` is the Functions option

## Speaker Notes

The tracking page needs ScanRecord documents built from ParcelScanned events in the shipments container. The change feed processor is the .NET SDK component that does the reading, scaling and checkpointing for that projection.

Builder: you call GetChangeFeedProcessorBuilder on the monitored container with a processor name and a delegate, then Build and StartAsync. A first start reads only new changes, unless WithStartTime(DateTime.MinValue.ToUniversalTime()) asks for the beginning of the container.

Lease container: it stores the position of each partition range and the instance that owns it, and a partitioned lease container needs /id as its partition key. Create the CosmosClient with the global endpoint, because lease documents are scoped to the endpoint that created them.

At least once: when the delegate throws, the processor reads the same batch again from the last saved position. A batch that always throws blocks its partition range, so Learn suggests an errored-message store inside the handler.

Idempotent upsert: the tracking document ID is the event ID, so a second upsert of the same event overwrites the record with identical content. The delegate awaits each upsert.

Lag and the Functions option: the estimator uses the same processor name and lease container and reports the pending count, best run as its own deployment. The Azure Functions CosmosDBTrigger uses the same machinery and lets the platform handle scaling and checkpoints.
