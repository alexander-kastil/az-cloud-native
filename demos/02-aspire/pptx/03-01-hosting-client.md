---
slide: "03-01"
topic: 03-integrations
title: "Hosting and client packages"
subtitle: "The AppHost declares the resource, the service consumes the connection"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/03-01-hosting-client.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Three
  columns with headers "AppHost" (hosting package declares the resource), "connection name" (the
  contract) and "Service" (client package registers the SDK client). Three rows. Service Bus, amber:
  "Aspire.Hosting.Azure.ServiceBus", AddAzureServiceBus("messaging").RunAsEmulator(), pink pill
  "messaging", blue service box "Aspire.Azure.Messaging.ServiceBus", AddAzureServiceBusClient(
  connectionName: "messaging"), chip ServiceBusClient "injected into the service". Cosmos DB, green:
  "Aspire.Hosting.Azure.CosmosDB", AddAzureCosmosDB("cosmos").RunAsEmulator(), pill "cosmos",
  "Aspire.Microsoft.Azure.Cosmos", AddAzureCosmosClient(connectionName: "cosmos"), chip CosmosClient.
  Storage, purple: "Aspire.Hosting.Azure.Storage", AddAzureStorage("storage").AddBlobs("blobs"), pill
  "blobs", "Aspire.Azure.Storage.Blobs", AddAzureBlobServiceClient("blobs"), chip BlobServiceClient. The
  connection name is pink #BE185D wherever it appears; straight arrows from each hosting box to its pill
  and from the pill to the service box.
order: 7
source: demos/02-aspire/03-integrations/readme.md
gamma_prompt: >
  Slide titled "Hosting and client packages", subtitle "The AppHost declares the resource, the service
  consumes the connection". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Hosting and client packages

## Content

- A **hosting package** runs in the AppHost and adds calls such as `AddAzureServiceBus`
- `aspire add azure-service-bus` adds the package and records its version
- A **client package** registers the SDK client with its health checks and OpenTelemetry
- `connectionName` must match the AppHost resource name: the name is the contract

## Speaker Notes

Grey needs Service Bus for events, Cosmos DB for shipment history and Storage for proof-of-delivery photos, and each dependency comes as two packages.

Hosting packages: they add `AddAzureServiceBus`, `AddAzureCosmosDB` and `AddAzureStorage` to the builder and carry the provisioning logic, including the `RunAsEmulator` option. A service that references only the client package never creates a Service Bus namespace.

aspire add: the command adds the hosting package and writes its version to `aspire.config.json`.

Client packages: they register `ServiceBusClient`, `CosmosClient` or `BlobServiceClient` with dependency injection, so the service receives the client through its constructor and never builds a connection itself. The readiness endpoint reports a broken Service Bus connection without extra code.

The contract: `AddAzureServiceBusClient(connectionName: "messaging")` reads `ConnectionStrings__messaging`, which `WithReference` supplies for the resource named messaging. A developer who knows that rule reads a value from the environment without opening the AppHost.
