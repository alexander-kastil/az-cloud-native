---
slide: "03-02"
topic: 03-integrations
title: "Local emulators and Azure deployment"
subtitle: "The same messaging resource and client code on both lanes"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/02-aspire/03-integrations/_images/laptop-vs-azure.svg"
media-file: demos/02-aspire/03-integrations/_images/laptop-vs-azure.svg
visual-prompt: >
  Existing diagram of the messaging resource on two lanes, the Service Bus emulator with its SQL Server companion on the laptop and a Service Bus namespace in Azure, with shipping-api reading the same connection on both, no generation. Referenced at its repo path.
order: 8
source: demos/02-aspire/03-integrations/readme.md
gamma_prompt: >
  Slide titled "Local emulators and Azure deployment", subtitle "The same messaging resource and client
  code on both lanes". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Local emulators and Azure deployment

## Content

- `RunAsEmulator` swaps the Azure resource for a local container during `aspire run`
- Service Bus runs with a SQL Server companion, Cosmos DB as vNext Linux emulator, Storage as **Azurite**
- Every emulator needs a **container runtime**; the first run pulls the images
- Without `RunAsEmulator`, Aspire provisions the real resource and the client code stays the same

## Speaker Notes

The same AppHost code runs on a laptop and against Azure, and the diagram puts the messaging resource on both lanes.

RunAsEmulator: it replaces the Azure resource with a named container that the AppHost starts. The emulator's logs appear in the dashboard next to the services that use it.

Three emulators: the Service Bus emulator runs with a SQL Server companion container and has no Entra ID sign-in, Cosmos DB uses the vNext Linux emulator, and Azurite emulates blob, queue and table storage. `WithDataVolume()` keeps the Cosmos DB data across runs.

Container runtime: every emulator needs one on the developer machine. The Service Bus emulator pulls its images on first use, so the first `aspire run` takes longer than the ones after it.

Deploy time: without `RunAsEmulator`, Aspire provisions a real Service Bus namespace and the AppHost needs an Azure subscription and location, while shipping-api keeps the same `connectionName`. A flow that passes against the emulator still needs a run on the Azure path before a release, and Module 3 covers `aspire deploy`.
