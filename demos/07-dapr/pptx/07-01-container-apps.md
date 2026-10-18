---
slide: "07-01"
topic: 07-managed-dapr
title: "Dapr on Container Apps"
subtitle: "Components live on the environment and scopes pick the app IDs"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/07-dapr/_images/components-on-container-apps.svg"
media-file: labs/07-dapr/_images/components-on-container-apps.svg
visual-prompt: >
  Existing diagram of pubsub.yaml and statestore.yaml registered on the Container Apps environment cae-azn-labs-07, scoped to app IDs and pointing to Service Bus and Cosmos DB, no generation. Referenced at its repo path.
order: 16
source: demos/07-dapr/07-managed-dapr/readme.md
gamma_prompt: >
  Slide titled "Dapr on Container Apps", subtitle "Components live on the environment and scopes pick
  the app IDs". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dapr on Container Apps

## Content

- `--enable-dapr` and `--dapr-app-id` turn Dapr on per app
- Components are defined once on the environment and scoped to app IDs
- Tier 1 and Tier 2 only: Cosmos DB state and Service Bus topics are **Tier 1**
- The platform picks and upgrades the Dapr version, with a suffix such as `-msft.1`
- No actor or workflow SDK packages, and no Dapr for jobs or Container Apps Express

## Speaker Notes

Booking and tracking are stateless request handlers that suit Container Apps. This slide shows how managed Dapr works there and which limits come with it.

Per app: Container Apps enables Dapr through each app's Dapr settings, and the sidecar exposes the building block APIs on port 3500. Only the listed sidecar options are available, and the Dapr configuration spec is not supported.

Environment components: components are defined once on the managed environment, and their scopes list the app IDs that load each one. The diagram shows the lab's environment `cae-azn-labs-07`, where `pubsub` goes to `tracking-service` and `notification-service`, and `statestore` to `routing-service`.

Tiers: Tier 1 components get immediate investigation for security or serious regressions, Tier 2 components a lower priority. The Cosmos DB state store and the Service Bus topics component are both Tier 1.

Versions: you cannot choose a Dapr version, and the environment is upgraded automatically on a best-effort basis. Planned maintenance controls when upgrades happen, not which version arrives.

Limits: the Dapr server extensions, actor and workflow SDK packages are not compatible with Container Apps, and Dapr is not supported for jobs. Container Apps Express supports neither Dapr nor jobs.
