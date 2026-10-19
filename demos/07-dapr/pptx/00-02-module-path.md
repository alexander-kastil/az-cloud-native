---
slide: "00-02"
topic: 07-dapr
title: "Overview"
subtitle: "The path through the seven topics"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette: cards #DBEAFE with #1D4ED8 border, text #0F172A and #334155, arrows #334155 4px,
  minimum 26px type. No heading inside the image; the slide title carries it. No arrow crosses a box or
  a label. Seven numbered cards read left to right, four in the top row and three centred below, each
  with a blue number badge, the topic title, its tagline and one white chip: 01 Dapr Sidecar "Run with
  Aspire" WithDaprSidecar, 02 Service Invocation "mTLS & Retries" InvokeMethodAsync, 03 State Stores
  "ETags & Concurrency" TrySaveStateAsync, 04 Pub/Sub "Swap Brokers via Components" PublishEventAsync,
  05 Resiliency Policies "Retries & Breakers" Resiliency spec, 06 Dapr Actors "Virtual Actor Model"
  ActorProxy.Create, 07 Managed Dapr "Container Apps vs. AKS" Microsoft.Dapr. Short arrows between
  neighbours, one elbow connector from card 04 down between the rows to card 05.
order: 2
source: demos/07-dapr/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path through the seven topics". Layout: Two columns. Left
  column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Start the sidecar from the Aspire AppHost and load the component files
- Call `routing-service` by app ID, then write state with **ETag** checks
- Publish scan events through a broker you can swap in YAML
- Put timeouts, retries and breakers in one **resiliency spec**, model each van as an actor
- Decide per service: Container Apps or AKS

## Speaker Notes

This slide walks once through the seven topics, which take one building block at a time. Each topic keeps the Grey domain, so `shipping-api`, `tracking-service` and `routing-service` stay the same while the calls change.

Sidecar first: the Aspire AppHost starts a Dapr sidecar beside each service, and component YAML describes the stores and brokers behind each block.

Calls and state: `shipping-api` reaches `routing-service` by app ID over mTLS, and state writes use ETags so two writers cannot both win a transition that must happen once.

Events: `ParcelScanned` events go through the pub/sub API, and swapping Service Bus for Redis or Kafka means replacing the component file.

Policies, actors and hosting: one resiliency spec holds timeouts, retries and circuit breakers, `VanActor` gives each van one owner, and the last topic compares Container Apps with the AKS Dapr extension.
