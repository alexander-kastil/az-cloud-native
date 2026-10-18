---
slide: "01-02"
topic: 01-sidecar
title: "Dapr in the Aspire AppHost"
subtitle: "Install the CLI and attach a sidecar to each service"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/01-02-component-name.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette: boxes #DBEAFE with #1D4ED8 border, component names #BE185D, arrows #334155 4px,
  minimum 26px type. No heading inside the image. No arrow crosses a box or a label. Left: a blue AppHost
  Program.cs panel with the readme's code: builder.AddDapr(); AddDaprComponent("statestore",
  "state.azure.cosmosdb", LocalPath = "components/statestore.yaml"); AddProject ShippingApi
  "shipping-api" .WithDaprSidecar(sidecar => sidecar.WithReference(stateStore)). Top right: a folded file
  card components/statestore.yaml with metadata name statestore and spec type state.azure.cosmosdb,
  reached by an elbow arrow from the LocalPath line. Bottom right: a Dapr sidecar box "reads the
  component file", fed by an "attaches" arrow from the AppHost and an "at startup" arrow from the file
  card, and a shipping-api box "passes statestore to the Dapr client" pointing at the sidecar. The name
  statestore is pink in all three places. Below, a pink-bordered pill: "the same name in the AppHost,
  the component file and the service code".
order: 4
source: demos/07-dapr/01-sidecar/readme.md
gamma_prompt: >
  Slide titled "Dapr in the Aspire AppHost", subtitle "Install the CLI and attach a sidecar to each
  service". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dapr in the Aspire AppHost

## Content

- `winget install Dapr.CLI`, then `dapr init`, then check `dapr --version`
- `dapr init` starts Redis, Zipkin, placement and scheduler; `--slim` skips them
- `AddDapr`, `AddDaprComponent` and `WithDaprSidecar` come from `CommunityToolkit.Aspire.Hosting.Dapr`
- Use one component name in the AppHost, the YAML `metadata.name` and the service code
- This class targets Dapr 1.18: CLI and runtime both report 1.18.0

## Speaker Notes

With the sidecar model in place, this slide shows how a developer machine and the Aspire AppHost get Dapr running beside a service.

Install and initialize: on Windows install the CLI with winget, run `dapr init` for the runtime, and check both versions with `dapr --version`.

What init starts: a default self-hosted initialization starts Redis and Zipkin containers plus the placement and scheduler services that actors need, and writes default component files under `.dapr` in your profile. The `--slim` flag skips the containers and default files for teams that bring their own infrastructure.

AppHost wiring: the Community Toolkit package adds `AddDapr`, `AddDaprComponent` registers a component, and `WithDaprSidecar` with `WithReference` gives the sidecar access to it. `DaprComponentOptions.LocalPath` points at the component file the sidecar reads at startup.

One name: keep the component name in the AppHost identical to `metadata.name` in the file and use the same name in the service code. Production component files reference a secret store, because the file is usually committed.

Dapr 1.18: the CLI and the runtime must both report 1.18.0, so rerun the version check after every CLI upgrade. The Community Toolkit README still names 1.15, so treat it as dated and follow the Dapr documentation.
