---
slide: "05-02"
topic: 05-app-insights
title: "Trace context and telemetry tables"
subtitle: "OperationId and ParentId join the rows of one scan"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/03-hosting/05-app-insights/_images/transaction-tree.svg"
media-file: demos/03-hosting/05-app-insights/_images/transaction-tree.svg
visual-prompt: >
  Existing diagram of the rows of one parcel scan in AppRequests, AppDependencies, AppTraces and AppExceptions sharing one OperationId, joined by ParentId into one transaction tree beside its Gantt chart, no generation. Referenced at its repo path.
order: 13
source: demos/03-hosting/05-app-insights/readme.md
gamma_prompt: >
  Slide titled "Trace context and telemetry tables", subtitle "OperationId and ParentId join the rows of
  one scan". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Trace context and telemetry tables

## Content

- The Service Bus SDK writes the trace context as `Diagnostic-Id` in `traceparent` format
- Send, receive, process and complete spans link through a `Message` span
- `AppRequests`, `AppDependencies`, `AppTraces` and `AppExceptions`: one table per type
- `OperationId` and `ParentId` join the rows of one transaction
- The end-to-end transaction view draws them as a **Gantt chart**

## Speaker Notes

Correlation depends on one identifier that travels with every hop, across the HTTP call and across the Service Bus message.

Trace context in the message: the Azure Service Bus SDK writes the W3C trace context into the `Diagnostic-Id` application property, in `traceparent` format. You don't write the propagation code, but both services need a tracing setup that exports the spans.

SDK spans: `ServiceBusSender.Send`, `ServiceBusReceiver.Receive`, `ServiceBusProcessor.ProcessMessage` and `ServiceBusReceiver.Complete` link through a `Message` span. A scan that starts at tracking-service and ends in a consumer appears as one transaction.

One table per type: `AppRequests` holds inbound requests, `AppDependencies` outbound calls, `AppTraces` log messages and `AppExceptions` exceptions.

The join: every row carries `OperationId` and `ParentId`, and a `union` over the four tables filtered by one `OperationId` returns the whole transaction in time order.

The portal view: transaction search opens the end-to-end transaction details, a Gantt chart of each event with its duration and response code. With sampling on, a missing row can mean the sampler dropped it.
