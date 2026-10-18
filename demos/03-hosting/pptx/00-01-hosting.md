---
slide: "00-01"
topic: 03-hosting
title: "Hosting on Container Apps and AKS"
subtitle: "Putting the Contoso Logistics services on Azure"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/03-hosting/pptx/images/00-01-hosting.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers in their late twenties, a woman and a man in modern work-casual
  clothes, stand in a bright operations corner of a modern office in front of a large wall-mounted
  display that shows a softly blurred grid of green and light-blue status tiles, their services coming
  online after a cloud deployment. She holds an open laptop on one forearm and points at a tile, he
  smiles and nods. They stand side by side in the centre third of the frame, turned half toward each
  other so both faces are clearly visible, calm white wall to the left and right. The mugs on the
  nearby counter are plain and blank. Modern, light, bright and friendly: high-key natural daylight,
  white and light-oak contemporary interior, soft pastel accents in light blue, mint and lavender, green
  plants, 2020s design. Screens show only soft blurred shapes and colours, never readable UI. 16:9
  landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks,
  extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 1
source: demos/03-hosting/readme.md
gamma_prompt: >
  Title slide "Hosting on Container Apps and AKS" with subtitle "Putting the Contoso Logistics services
  on Azure". Side-accent layout: title and subtitle on the left half on plain white, the provided
  photograph of two engineers at a bright operations wall display filling the right half, no text over
  the photo. Use exactly the one image given for this card; do not generate or add any other images,
  icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Hosting on Container Apps and AKS

## Content

- **Deploy**: `aspire deploy` puts the AppHost on Container Apps, the routing service runs on AKS
- **Configure**: secrets from Key Vault and settings from App Configuration over a managed identity
- **Release and trace**: revisions shift traffic, Application Insights follows a parcel scan

## Speaker Notes

This module takes the Contoso Logistics services that modules 1 and 2 designed and modeled in an Aspire AppHost, and puts them on Azure.

Deploy: most services run on Azure Container Apps, the managed host for this class, and `aspire deploy` provisions the registry, the environment and the apps. The routing service runs on AKS because it depends on Dapr actors.

Configure: secrets live in Key Vault and settings in App Configuration, and each service reads them through a managed identity, so no password is stored.

Release and trace: revisions shift traffic to a new version and drain message consumers, and Application Insights follows a parcel scan from the HTTP request to the Service Bus handler.
