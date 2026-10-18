---
slide: "05-01"
topic: 05-redis-cache
title: "Cache-aside with change feed invalidation"
subtitle: "The projection deletes the cached tracking page after each scan"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/06-event-sourcing-cqrs/_images/cache-invalidation.svg"
media-file: labs/06-event-sourcing-cqrs/_images/cache-invalidation.svg
visual-prompt: >
  Existing diagram of TrackingCache, the Redis key tracking:<trackingNumber>, the tracking container and tracking-projection across a first read, a repeat read, a new scan with UpsertItemAsync then KeyDeleteAsync, and the next read, no generation. Referenced at its repo path.
order: 12
source: demos/06-event-sourcing-cqrs/05-redis-cache/readme.md
gamma_prompt: >
  Slide titled "Cache-aside with change feed invalidation", subtitle "The projection deletes the cached
  tracking page after each scan". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Cache-aside with change feed invalidation

## Content

- **Cache-aside**: read `tracking:<trackingNumber>`, query on a miss, store with an expiration
- Five minutes plus up to 30 seconds of **jitter** spreads the expirations
- The projection upserts the scan first, then calls `KeyDeleteAsync`
- A retry repeats the idempotent upsert, and the TTL bounds a stale page
- In Azure: Entra ID through `Microsoft.Azure.StackExchangeRedis`, wired by Aspire

## Speaker Notes

A parcel's tracking page can be opened hundreds of times while the parcel is in transit, and each open would otherwise run a query against the tracking container. Azure Managed Redis answers the repeat reads from memory, and the change feed keeps the cache correct.

Cache-aside: TrackingCache reads the key tracking:<trackingNumber> and returns the page on a hit. On a miss it queries the tracking container, stores the serialized page with an expiration and returns it.

Jitter: pages loaded in the same minute would otherwise expire together and send a burst of queries to Cosmos DB at once. The random offset of up to 30 seconds spreads the expirations, as Learn recommends.

Invalidation order: the projection runs after the scan commits, so the cache never removes a page for a write that failed. It upserts the ScanRecord first and then deletes the key, so the next read loads a fresh copy.

Retries: if the delete fails after the upsert, the processor retries the batch, and the idempotent upsert makes that safe. The TTL bounds how long the old page can survive if the retries keep failing.

Wiring: in Azure the connection authenticates with a Microsoft Entra token through Microsoft.Azure.StackExchangeRedis, and the AppHost passes AddAzureManagedRedis to the service with WithReference. The lab runs a local Redis container with AddRedis, and the service registers the same name with AddRedisClient.
