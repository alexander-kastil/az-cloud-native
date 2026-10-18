---
slide: "05-01"
topic: 05-app-insights
title: "Sending telemetry to Application Insights"
subtitle: "The Azure Monitor distro in the shared service defaults"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/03-hosting/_images/telemetry-export.svg"
media-file: labs/03-hosting/_images/telemetry-export.svg
visual-prompt: >
  Existing diagram of the AppHost setting APPLICATIONINSIGHTS_CONNECTION_STRING on shipping-api and tracking-service, and UseAzureMonitor exporting traces, metrics and logs to app-insights, no generation. Referenced at its repo path.
order: 12
source: demos/03-hosting/05-app-insights/readme.md
gamma_prompt: >
  Slide titled "Sending telemetry to Application Insights", subtitle "The Azure Monitor distro in the
  shared service defaults". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Sending telemetry to Application Insights

## Content

- `UseAzureMonitor` adds traces, metrics and logs to an ASP.NET Core service
- The distro reads `APPLICATIONINSIGHTS_CONNECTION_STRING`
- `AddAzureApplicationInsights` and `WithReference` set it in publish mode
- The **managed OpenTelemetry agent** receives OTLP over gRPC (preview)
- Learn recommends the Microsoft OpenTelemetry Distro for new .NET apps

## Speaker Notes

When a parcel scan fails, the cause can sit in the HTTP request, the Service Bus message, the database write or a consumer exception. One trace across those hops starts with every service sending telemetry.

The distro: install `Azure.Monitor.OpenTelemetry.AspNetCore` and call `UseAzureMonitor` on the OpenTelemetry builder to add traces, metrics and logs to an ASP.NET Core service.

The connection string: the distro reads `APPLICATIONINSIGHTS_CONNECTION_STRING`, and the lab's shared service defaults call `UseAzureMonitor` only when that variable is set.

Aspire wiring: in the AppHost, `AddAzureApplicationInsights` and `WithReference` set the variable for each service, and only in publish mode.

The managed agent: a preview alternative for apps that send OTLP over gRPC, running inside the Container Apps environment as one replica with a fixed 0.5 vCPU and 1.5 GB of memory. Its configuration lives at the environment level, and it sends logs and traces to Application Insights, which takes no metrics from it.

Distro choice: Learn now recommends the Microsoft OpenTelemetry Distro for new .NET apps, and existing Azure Monitor distro users need no action. This module teaches the Azure Monitor distro and its `UseAzureMonitor` call.
