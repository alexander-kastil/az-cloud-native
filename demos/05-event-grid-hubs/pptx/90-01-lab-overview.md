---
slide: "90-01"
topic: 05-event-grid-hubs
title: "Streaming van positions to a live map"
subtitle: "The lab in five steps, from the van simulator to the partner queue"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/05-event-grid-hubs/_images/van-positions-and-proof-of-delivery.svg"
media-file: labs/05-event-grid-hubs/_images/van-positions-and-proof-of-delivery.svg
visual-prompt: >
  Existing diagram of what runs at the end of lab 5, each piece marked with the step that adds it: VanSimulator streaming over Kafka into van-positions, telemetry-ingest checkpointing to ops-console and pushing through Azure SignalR Service to the ops console map, and a photo in proof-of-delivery reaching the partner-delivered queue through pod-events, LabelService and egt-contoso-labs05, no generation. Referenced at its repo path.
order: 14
source: labs/05-event-grid-hubs/readme.md
gamma_prompt: >
  Slide titled "Streaming van positions to a live map", subtitle "The lab in five steps, from the van
  simulator to the partner queue". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Streaming van positions to a live map

## Content

- The lab starts from the lab 4 solution, with an empty map and no partner events
- Step 1: a van simulator sends keyed positions over **Kafka**
- Steps 2 and 3: a checkpointed processor pushes them through **Azure SignalR Service**
- Steps 4 and 5: a proof-of-delivery photo publishes `ParcelDelivered` to the partner queue
- About 45 minutes, with Docker Desktop and `provision.azcli`

## Speaker Notes

The lab ties the module together: at the start the ops console shows no vans, and no partner learns when a parcel is delivered. The diagram marks each piece with the step that adds it.

Starting point: the lab begins from the lab 4 solution in `labs/05-event-grid-hubs/contoso-logistics/`, worked with an AI coding agent that reads `AGENTS.md` for the class stack.

Step 1: `VanSimulator` sends one `VanPositionReported` per van every second to `van-positions`, over the Kafka endpoint of the Event Hubs emulator and keyed by `VanId`.

Steps 2 and 3: `telemetry-ingest` reads through the `ops-console` consumer group, checkpoints to Azurite and pushes to a map panel that draws one dot per van. A configuration value then switches the hub to Azure SignalR Service in Default mode, and the hub route stays the same.

Steps 4 and 5: `ParcelDeliveredPublisher` sends `ParcelDelivered` as a CloudEvent to the custom topic, and the `partner-delivered` subscription puts it in the partner's queue. A background worker then turns a photo in `proof-of-delivery` into that event without a manual publish.

Setup: Docker Desktop must run for steps 1 and 2, and `provision.azcli` creates every Azure resource for steps 3 to 5 in `rg-azn-labs`. Keep that resource group afterwards, because the next lab deploys into it.
