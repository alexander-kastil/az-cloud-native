---
slide: "04-03"
topic: 04-dashboard
title: "MCP access and the standalone dashboard"
subtitle: "Coding agents read the telemetry, any OTLP app can send it"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/04-03-agents-standalone.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Four
  boxes in a two by two grid. Top left blue "Python or Node.js app", "runs outside an AppHost",
  OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317 and OTEL_EXPORTER_OTLP_PROTOCOL=grpc, with an arrow
  "OTLP" down into the bottom left purple "Standalone dashboard": "aspire dashboard run", "or the
  mcr.microsoft.com/aspire/dashboard container image", three port chips "18888 dashboard UI", "4317 OTLP
  over gRPC", "4318 OTLP over HTTP", then "OTLP viewer for any app that exports OpenTelemetry" and "no
  AppHost, so no resource start and stop controls". Top right green "AI coding agent": "aspire agent
  init", "sets up agent configuration for the detected agent environments", "reads the same resources,
  logs and traces as the developer", with an arrow "calls tools" down into the bottom right amber "MCP
  server": "aspire agent mcp --dashboard-url "http://localhost:18888"", tools list_structured_logs,
  list_resources, list_console_logs, list_traces, and "standalone connection: telemetry tools only". A
  horizontal arrow "connects" from the MCP server to the dashboard.
order: 11
source: demos/02-aspire/04-dashboard/readme.md
gamma_prompt: >
  Slide titled "MCP access and the standalone dashboard", subtitle "Coding agents read the telemetry, any
  OTLP app can send it". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# MCP access and the standalone dashboard

## Content

- `aspire agent init` sets up agent configuration for the detected agent environments
- `aspire agent mcp` exposes telemetry as tools such as `list_traces` and `list_structured_logs`
- `aspire dashboard run` starts a **standalone dashboard** for apps outside an AppHost
- Apps send OTLP to port 4317 (gRPC) or 4318 (HTTP); the UI runs on 18888

## Speaker Notes

The dashboard's telemetry is useful beyond the browser: an AI coding agent can read it, and apps without an AppHost can send to it.

aspire agent init: it sets up agent configuration for the agent environments that the CLI detects.

MCP server: `aspire agent mcp` starts the Model Context Protocol server with tools such as `list_resources`, `list_structured_logs`, `list_traces` and `list_console_logs`, so an agent reads the same telemetry as the developer. Connected to a standalone dashboard with `--dashboard-url` and `--api-key`, it exposes telemetry tools only and cannot manage AppHost resources.

Standalone dashboard: it is an OTLP viewer for any app that exports OpenTelemetry, including Python and Node.js apps, started with `aspire dashboard run` or the container image. It has no AppHost, so it has no resource start and stop controls.

Ports: `OTEL_EXPORTER_OTLP_ENDPOINT` points the app at the dashboard, with gRPC on port 4317 and HTTP on 4318. The UI opens at `http://localhost:18888`.
