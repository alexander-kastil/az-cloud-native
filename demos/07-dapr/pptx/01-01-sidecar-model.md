---
slide: "01-01"
topic: 01-sidecar
title: "The sidecar model"
subtitle: "The service calls localhost and the component file picks the store"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/07-dapr/01-sidecar/_images/sidecar-component-swap.svg"
media-file: demos/07-dapr/01-sidecar/_images/sidecar-component-swap.svg
visual-prompt: >
  Existing diagram of routing-service calling TrySaveStateAsync on its Dapr sidecar, with the component file sending the same call to Cosmos DB or Redis, no generation. Referenced at its repo path.
order: 3
source: demos/07-dapr/01-sidecar/readme.md
gamma_prompt: >
  Slide titled "The sidecar model", subtitle "The service calls localhost and the component file picks
  the store". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The sidecar model

## Content

- Dapr runs as a separate process next to each service
- The service calls its sidecar on localhost: HTTP on port 3500, gRPC on 50001
- Building blocks: service invocation, state, pub/sub, bindings, actors, secrets, configuration
- Each block delegates to a **component**: Cosmos DB, Service Bus or Redis
- `TrySaveStateAsync("statestore", ...)` stays the same in both runs

## Speaker Notes

Contoso Logistics runs booking, tracking, routing and billing as separate processes, and each needs to find the others, save state and publish events without importing one cloud product's SDK. Dapr gives each service that portability through a sidecar and a set of building blocks.

Separate process: Dapr runs beside the application, and the sidecar handles the connection to the infrastructure.

Localhost calls: the application calls the sidecar over HTTP or gRPC on localhost. In Azure Container Apps the sidecar serves HTTP on port 3500 and gRPC on port 50001.

Building blocks: these are the APIs the sidecar exposes. Service invocation, state management, pub/sub, bindings, actors, secrets and configuration are generally available in Container Apps.

Components: each block delegates to a component, so the same call reaches Cosmos DB, Service Bus or Redis depending on the file the sidecar loads. The diagram shows `routing-service` saving state to Cosmos DB in one run and to Redis in the other with the same code.
