---
slide: "90-01"
topic: 03-hosting
title: "Shipping Grey Logistics to Container Apps"
subtitle: "What runs in rg-azn-labs at the end of the lab"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/03-hosting/_images/lab-end-state.svg"
media-file: labs/03-hosting/_images/lab-end-state.svg
visual-prompt: >
  Existing diagram of what runs at the end of the lab, the AppHost with its emulators on your machine and the pieces in rg-azn-labs, each marked with the step that adds it, no generation. Referenced at its repo path.
order: 14
source: labs/03-hosting/readme.md
gamma_prompt: >
  Slide titled "Shipping Grey Logistics to Container Apps", subtitle "What runs in rg-azn-labs at the
  end of the lab". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Shipping Grey Logistics to Container Apps

## Content

- Step 1: `aspire deploy` creates `aca-env` with public `shipping-api` and `tracking-service`
- Steps 2 and 3: `carrier-api-key` from **Key Vault**, `Tracking:PageSize` from **App Configuration**
- Step 4: both services export telemetry to **Application Insights**
- Step 5: tracking-service v2 behind a **90/10 traffic split**
- Step 6: `booking-trace.kql` follows one booking across both services

## Speaker Notes

In the lab, Grey Logistics moves from your machine, where the AppHost starts the Service Bus and Cosmos DB emulators, into the resource group `rg-azn-labs`. It takes about 55 minutes, and each step adds one piece of the diagram.

Deploy: step 1 declares `aca-env`, gives `shipping-api` and `tracking-service` external endpoints and maps `/health` in every environment. Then `aspire deploy --non-interactive` creates the apps.

Secrets and settings: step 2 adds a Key Vault and a `carrier-api-key` health check, and step 3 adds an App Configuration store with `Tracking:PageSize` set to 50. Each step proves the read with a health check that reports Healthy.

Telemetry: step 4 adds an Application Insights resource and the Azure Monitor distro in the shared service defaults, so one booking can be followed across both services.

Release: step 5 adds a `schemaVersion` field to the tracking response, switches tracking-service to multiple revision mode and sends 10 percent of requests to v2. The previous revision stays active for rollback.

Query: step 6 writes `docs/booking-trace.kql` and sends one booking to `shipping-api`. Keep the environment afterwards, because the next lab deploys into the same resource group.
