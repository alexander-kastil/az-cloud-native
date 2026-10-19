---
slide: "07-02"
topic: 07-managed-dapr
title: "Choosing Container Apps or AKS"
subtitle: "Four questions decide where a service runs"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/07-02-host-decision.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette, minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. A grey start box "A Grey Logistics service" on the left leads to four white question boxes
  in a row joined by "no" arrows: "Hosts actors or uses the Workflow SDK?", "Needs a pinned Dapr
  version?", "Needs a component outside Tier 1 and Tier 2?", "Runs as a job?". From each question a
  "yes" arrow rises to one wide purple bar across the top "AKS with the Dapr extension, routing-service
  with VanActor". The last question's "no" arrow drops to a green box "Container Apps with Dapr,
  shipping-api, tracking-service".
order: 17
source: demos/07-dapr/07-managed-dapr/readme.md
gamma_prompt: >
  Slide titled "Choosing Container Apps or AKS", subtitle "Four questions decide where a service runs".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Choosing Container Apps or AKS

## Content

- AKS installs Dapr as the `Microsoft.Dapr` cluster extension
- The extension supports the current and the previous Dapr version
- Pin a version with `--version` and `--auto-upgrade-minor-version false`
- High availability and mTLS are on by default: three control-plane replicas
- Container Apps for stateless Tier 1 services, AKS for actors and pinned versions

## Speaker Notes

Both platforms offer Dapr, but they differ in who picks the version, which components are supported and which features exist at all. The decision flow asks four questions per service.

The extension: the Dapr extension for AKS is a cluster extension of type `Microsoft.Dapr`, so it follows the AKS support policy.

Version window: the extension supports the current and the previous Dapr versions, and keeping inside that window is the cluster owner's job. An old cluster may need intermediate upgrades.

Pinning: `stable` is the default release train, and `dev` carries experimental features unsuitable for production. Set `--version` together with `--auto-upgrade-minor-version false` to pin a version.

Defaults: a new cluster starts with three control-plane replicas and workload certificates that last 24 hours. The Container Apps actor restriction does not apply, so AKS is the path for `routing-service` with `VanActor`.

The rule: choose Container Apps for stateless services on Tier 1 components and accept the platform's upgrade timing. Choose AKS for services that host actors, need a pinned version or depend on a component outside the Container Apps list.
