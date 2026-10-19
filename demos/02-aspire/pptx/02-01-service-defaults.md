---
slide: "02-01"
topic: 02-service-defaults
title: "The ServiceDefaults project"
subtitle: "One call gives a service telemetry, resilience, discovery and health checks"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/02-01-service-defaults.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Left:
  two blue #DBEAFE service boxes, shipping-api and tracking-service, each with two white code chips
  "builder.AddServiceDefaults();" and "app.MapDefaultEndpoints();", and under them the note
  "IHostApplicationBuilder extension, so web apps and worker services use the same call". An arrow
  "references" from each service into a frame on the right titled "ServiceDefaults", secondary "project
  from the Aspire template", holding four stacked rows: blue "OpenTelemetry" with "logs, metrics and
  traces, plus the Azure.* sources" and "OTEL_EXPORTER_OTLP_ENDPOINT turns on the OTLP exporter"; purple
  "Standard resilience handler" with "AddStandardResilienceHandler on every HttpClient from the factory"
  and "retries, timeouts and a circuit breaker"; amber "Service discovery" with "https+http://shipping-api
  resolves the AppHost resource name" and "HTTPS when available, HTTP otherwise"; green "Health
  endpoints" with "/health every check, /alive checks tagged live" and "mapped by MapDefaultEndpoints in
  Development only".
order: 5
source: demos/02-aspire/02-service-defaults/readme.md
gamma_prompt: >
  Slide titled "The ServiceDefaults project", subtitle "One call gives a service telemetry, resilience,
  discovery and health checks". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The ServiceDefaults project

## Content

- Each service references ServiceDefaults and calls `AddServiceDefaults` once
- **OpenTelemetry** wires logs, metrics and traces; `OTEL_EXPORTER_OTLP_ENDPOINT` sets the target
- Every factory `HttpClient` gets the resilience handler and **service discovery**
- `MapDefaultEndpoints` maps `/health` and `/alive`, in Development only

## Speaker Notes

Every Grey service needs the same telemetry, retry and discovery behaviour, and services that each configure it on their own drift apart. The ServiceDefaults project that the Aspire template generates holds that behaviour once.

One call: `AddServiceDefaults` extends `IHostApplicationBuilder`, so web apps and worker services use the same call. It also sets the `Azure.Experimental.EnableActivitySource` switch.

OpenTelemetry: logs, metrics and traces go to OpenTelemetry, and `UseOtlpExporter` runs only when `OTEL_EXPORTER_OTLP_ENDPOINT` is set, so that variable redirects every signal without a code change. A domain area such as shipment booking adds its own source with `AddSource("Grey.Shipping")`.

Resilience and discovery: `ConfigureHttpClientDefaults` adds the standard resilience handler and service discovery to every client the factory creates. A base address such as `https+http://shipping-api` prefers HTTPS, falls back to HTTP and resolves the resource name from the AppHost.

Health endpoints: `/health` reports every registered check and `/alive` only the checks tagged live, so a liveness probe does not depend on downstream services. Both are mapped only in Development, because exposing them has security implications.
