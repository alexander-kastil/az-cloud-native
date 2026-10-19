---
slide: "90-01"
topic: 07-dapr
title: "What the lab builds"
subtitle: "Scan, tracking, notification and routing flows on Dapr building blocks"
layout: mixed
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/07-dapr/_images/lab-overview.svg"
media-file: labs/07-dapr/_images/lab-overview.svg
visual-prompt: >
  Existing diagram of Grey Logistics before step 1 and after step 7, locally and on Container Apps, with the step that adds each Dapr piece, no generation. Referenced at its repo path.
order: 18
source: labs/07-dapr/readme.md
gamma_prompt: >
  Slide titled "What the lab builds", subtitle "Scan, tracking, notification and routing flows on Dapr
  building blocks". Two equal columns. Left column: title, subtitle and bullets at normal size. Right
  column: the provided diagram of the lab before and after as an inline content image, scaled down to
  fit the card height, never cropped. Keep body text at the normal body size; never shrink text to fit.
  Use exactly the one image given for this card; do not generate or add any other images, icons or
  logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# What the lab builds

## Content

- Sidecars with the Redis components `pubsub` and `statestore` (step 1)
- Scan events on `parcel-events`, with the tracking number as `SessionId` (step 2)
- routing-service: an assignment with an ETag check, called by app ID (steps 3 and 4)
- One resiliency spec and one `VanActor` per van (steps 5 and 6)
- The same component names on `cae-azn-labs-07`, backed by Service Bus and Cosmos DB (step 7)

## Speaker Notes

The lab starts with the Grey Logistics services calling Service Bus, Cosmos DB and Redis through their own SDKs, and moves them onto Dapr building blocks in about 65 minutes. What you practise is swapping an infrastructure dependency behind a named building block.

Step 1: a new `notification-service` joins, and the AppHost gives `tracking-service`, `notification-service` and `shipping-api` sidecars that load `pubsub` and `statestore`, both on the Redis container from `dapr init`. Push back if a sidecar starts with no component file behind it.

Step 2: `tracking-service` publishes each `ParcelScanned` through `pubsub` to `parcel-events` with the tracking number as `SessionId`, and `notification-service` receives it through a Dapr subscription. No `Azure.Messaging.ServiceBus` type stays in this flow.

Steps 3 and 4: the new `routing-service` stores each parcel assignment with first-write-wins and returns a conflict for a second van. `shipping-api` reaches it with `InvokeMethodAsync` by app ID and builds no host name.

Steps 5 and 6: one resiliency spec holds the timeout, the retries on 500 to 599 and the breaker, so `shipping-api` keeps no retry loop. `VanActor` keeps each van within its capacity and runs on the local AppHost only.

Step 7: the Azure component files register `pubsub` and `statestore` on `cae-azn-labs-07` with Tier 1 types and scopes. Push back if an Azure file uses a Redis type, because Redis is Tier 2 on Container Apps.
