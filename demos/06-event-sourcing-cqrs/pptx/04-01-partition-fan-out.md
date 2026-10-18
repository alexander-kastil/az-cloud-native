---
slide: "04-01"
topic: 04-read-models
title: "Point reads and queries"
subtitle: "Indexing and request charge on the tracking container"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/06-event-sourcing-cqrs/04-read-models/_images/partition-fan-out.svg"
media-file: demos/06-event-sourcing-cqrs/04-read-models/_images/partition-fan-out.svg
visual-prompt: >
  Existing diagram of three reads on the tracking container with four physical partitions: a point read reaching one item, a single-partition query reaching one parcel's scans, and the ops console search fanning out to every partition, no generation. Referenced at its repo path.
order: 10
source: demos/06-event-sourcing-cqrs/04-read-models/readme.md
gamma_prompt: >
  Slide titled "Point reads and queries", subtitle "Indexing and request charge on the tracking
  container". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column,
  never cropped, never used as a background or side accent. Use exactly the one image given for this
  card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Point reads and queries

## Content

- Opt-out indexing: include `/*`, exclude `/_etag/?`
- A **composite index** on `scanType` ascending and `occurredAt` descending matches the ops console sort
- Log `RequestCharge` and `ActivityId` on every read you tune
- A **point read** by `id` and partition key skips the query engine and costs the least
- The partition key in `QueryRequestOptions` keeps the page query on one partition

## Speaker Notes

Cosmos DB charges request units for every read, and the bill depends on the indexing policy, the shape of the query and the consistency level. This slide tunes the tracking container for the tracking page and the ops console.

Indexing policy: Cosmos DB indexes every property by default, and Learn recommends an opt-out policy that excludes only the paths you never filter on. The tracking policy includes /* and excludes /_etag/?, so new properties get indexed automatically.

Composite index: a query that sorts on more than one property needs a composite index in the same sort order. The ops console query sorts by scanType ascending and occurredAt descending, and composite paths never use wildcards.

Request charge: ItemResponse.RequestCharge returns the RUs of each response, and ActivityId identifies the request in diagnostics. Measure before you change the indexing policy, because index changes can take time to apply.

Point read: a read by id and partition key skips the query engine and costs the least. The tracking page can't use one for every scan of a parcel, because it doesn't know the scan IDs in advance.

Query scope: with the partition key in QueryRequestOptions, the page query stays on one partition. Without it, the query fans out across every physical partition and the charge grows with the partition count, a cost only the ops console search accepts.
