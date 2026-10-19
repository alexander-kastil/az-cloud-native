---
slide: "04-02"
topic: 04-dashboard
title: "Traces across HTTP and Service Bus"
subtitle: "One trace ID follows the booking from shipping-api to tracking-service"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/02-aspire/04-dashboard/_images/trace-propagation.svg"
media-file: demos/02-aspire/04-dashboard/_images/trace-propagation.svg
visual-prompt: >
  Existing diagram of one trace ID carried from POST /shipments in shipping-api through the Service Bus topic shipment-events to tracking-service, first in the traceparent header and then in the Diagnostic-Id property, with a span per step in the trace waterfall, no generation. Referenced at its repo path.
order: 10
source: demos/02-aspire/04-dashboard/readme.md
gamma_prompt: >
  Slide titled "Traces across HTTP and Service Bus", subtitle "One trace ID follows the booking from
  shipping-api to tracking-service". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Traces across HTTP and Service Bus

## Content

- HTTP calls carry the W3C trace context in the `traceparent` header
- Service Bus carries it in the `Diagnostic-Id` property of each message
- The waterfall adds a span per step; the **longest bar** names the slowest step
- The defaults switch on the Azure SDK spans and register the `Azure.*` source

## Speaker Notes

Traces connect the requests of one booking across services, and the diagram follows a single trace ID from the booking POST to the tracking projection.

traceparent: the defaults instrument both ends of an HTTP call, so the caller and the callee each create a span and both spans share one trace identifier.

Diagnostic-Id: the Azure SDK writes the trace context to this application property and creates spans such as `ServiceBusSender.Send` and `ServiceBusReceiver.Receive`. One trace therefore covers the booking from `POST /shipments` in shipping-api to the projection in tracking-service.

Waterfall: each span shows its start time, its duration and its parent. The developer reads the cause from the longest bar before opening any log.

Defaults: the Service Bus spans appear when the `Azure.Experimental.EnableActivitySource` switch is set and the `Azure.*` source is registered, and the service defaults do both. The CLI reads the same spans with `aspire otel traces --format Json --limit 100`, which the lab's verification script uses.
