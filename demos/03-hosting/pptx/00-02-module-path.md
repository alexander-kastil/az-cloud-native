---
slide: "00-02"
topic: 03-hosting
title: "Overview"
subtitle: "The path through the five topics"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/03-hosting/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, ground #F8FAFC as in the module's existing
  diagrams, 'Segoe UI', Inter, Arial, sans-serif, text #0F172A and secondary #64748B, 3px card borders,
  minimum 26px type. No heading inside the image; the slide title carries it. No arrow crosses a box or
  a label. Top row of three blue #DBEAFE cards with #1D4ED8 borders, caption above "Deploy, configure
  and release on Container Apps": 01 Aspire Deploy "ACR and Container Apps", 02 Key Vault & App
  Configuration "secrets and settings", 03 Revisions "traffic splitting and draining". Bottom row of two
  green #DCFCE7 cards with #15803D borders, caption above "Choose the host, then follow a parcel
  scan": 04 AKS "routing service with Dapr actors", 05 Application Insights "traces and KQL". Each
  number sits in a filled circle badge. Short arrows between neighbours, one elbow connector from card
  03 down between the rows and round the left of the bottom caption into card 04.
order: 2
source: demos/03-hosting/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path through the five topics". Layout: Two columns. Left
  column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Deploy the AppHost to Container Apps with **aspire deploy**
- Keep secrets in **Key Vault** and settings in **App Configuration**
- Release a new version behind a **traffic split** and drain consumers
- Host the routing service on **AKS** with the Dapr extension
- Follow a parcel scan in **Application Insights** with KQL

## Speaker Notes

This slide walks the five topics once, in the order the module teaches them.

Aspire Deploy: the AppHost declares the Container Apps environment, and `aspire deploy` provisions the registry, the environment and one container app per service.

Key Vault and App Configuration: secrets and settings reach the services through a managed identity, and feature flags refresh on their own every 30 seconds by default.

Revisions: a new release starts with a small traffic weight while the previous revision stays active for rollback. Consumers finish their in-flight messages before the platform stops the replica.

AKS: the routing service keeps one Dapr actor per van, which Container Apps managed Dapr doesn't support, so Aspire generates its Kubernetes deployment from the same AppHost.

Application Insights: the Azure Monitor distro sends the telemetry, and KQL joins the rows of one transaction by `OperationId`.
