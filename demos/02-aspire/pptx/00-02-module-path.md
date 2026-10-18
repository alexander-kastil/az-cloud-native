---
slide: "00-02"
topic: 02-aspire
title: "Overview"
subtitle: "From the AppHost model to distributed tests"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Two
  rows of three numbered rounded cards, read left to right, each card a two-digit number, the topic
  title from the module readme and two secondary lines. Top row blue #DBEAFE with #1D4ED8 border,
  caption on the left "Model and connect the system": 01 Aspire AppHost: Model the System "AddProject,
  WithReference, WaitFor and aspire run", 02 ServiceDefaults: Telemetry, Resilience, Discovery "One call
  for OpenTelemetry, retries and health endpoints", 03 Aspire Integrations: Hosting & Client "Hosting
  and client packages, RunAsEmulator". Bottom row green #DCFCE7 with #15803D border, caption "Observe,
  extend and test": 04 Aspire Dashboard: Traces Across Services "Logs, traces, metrics and aspire agent
  mcp", 05 Polyglot AppHost: Angular & Python "AddViteApp and AddUvicornApp beside the .NET services",
  06 Aspire Testing: Distributed Integration Tests "Tests that start the AppHost, gates for each step".
  Short arrows between neighbours, one elbow connector from card 03 down between the rows to card 04.
order: 2
source: demos/02-aspire/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "From the AppHost model to distributed tests". Layout: Two columns.
  Left column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Model the system in an **AppHost** and start it with `aspire run`
- Give every service the same **ServiceDefaults** for telemetry, retries and discovery
- Connect Service Bus, Cosmos DB and Storage through **integrations** and emulators
- Follow one booking across services in the **dashboard**
- Host Angular and Python beside .NET, then **test** the event flow end to end

## Speaker Notes

This slide walks the path through the six topics once, following on from the three themes of the title slide.

Model and run: the AppHost declares every resource in C#, WithReference wires the connections and WaitFor orders the startup. `aspire run` starts the whole model together with the dashboard.

Shared behaviour: ServiceDefaults adds OpenTelemetry, the standard resilience handler, service discovery and health endpoints with one call. Integrations split the hosting package in the AppHost from the client package in the service.

Observe: the dashboard shows resources, logs, traces and metrics in one browser tab, and an AI coding agent reads the same telemetry through MCP.

Extend and test: the Angular ops console and the Python exception agent run under the same AppHost. DistributedApplicationTestingBuilder starts that AppHost inside a test and checks the booking flow end to end.
