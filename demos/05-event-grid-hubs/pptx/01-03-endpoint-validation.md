---
slide: "01-03"
topic: 01-custom-topics
title: "Endpoint validation"
subtitle: "Event Grid proves you own the endpoint before the first event"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/05-event-grid-hubs/pptx/images/01-03-endpoint-validation.svg
visual-prompt: >
  Opus 5.5, hand-written SVG in the module's diagram style, viewBox 0 0 1920 1080, slate #F8FAFC canvas
  with two white panels side by side bordered #94A3B8 2px, 'Segoe UI', Inter, Arial, sans-serif,
  headings #0F172A, secondary text #64748B, minimum 26px type, arrows #334155 with a dot at the sender.
  No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Each panel
  is a sequence diagram with two participants, amber #FEF3C7 "Event Grid" on the left and blue #DBEAFE
  "partner endpoint" with "HTTPS webhook" on the right, joined by dashed lifelines. Left panel "CloudEvents
  schema", secondary "every partner endpoint in this module": OPTIONS with "header
  WebHook-Request-Origin" to the endpoint, "WebHook-Allowed-Origin" with "the same origin or an asterisk"
  back, "POST the first event" to the endpoint; below it a green #DCFCE7 note "Either handshake runs
  automatically in Azure Functions with the Event Grid trigger and Azure Logic Apps". Right panel "Event
  Grid schema": "SubscriptionValidationEvent" with "aeg-event-type: SubscriptionValidation" to the
  endpoint, "HTTP 200 with the validation code" with "within 30 s, HTTP 202 does not count" back, a
  purple "or, without a synchronous answer", "GET validationUrl" with "within 10 min" back, "POST the
  first event" to the endpoint.
order: 5
source: demos/05-event-grid-hubs/01-custom-topics/readme.md
gamma_prompt: >
  Slide titled "Endpoint validation", subtitle "Event Grid proves you own the endpoint before the first
  event". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card; do
  not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Endpoint validation

## Content

- **CloudEvents schema**: `OPTIONS` with `WebHook-Request-Origin`, answered by `WebHook-Allowed-Origin`
- **Event Grid schema**: a `SubscriptionValidationEvent`, answered with HTTP 200 and the code within 30 seconds
- Without a synchronous answer, open the `validationUrl` with a GET within 10 minutes
- Azure Functions with the Event Grid trigger and Logic Apps answer automatically
- HTTP 202 doesn't count, and self-signed certificates aren't supported

## Speaker Notes

Before Event Grid sends the first event to a webhook, it checks that you own the endpoint, which stops a third party from flooding it. The handshake depends on the schema of the subscription.

CloudEvents schema: Event Grid sends an HTTP OPTIONS request with a `WebHook-Request-Origin` header, and the endpoint agrees by returning `WebHook-Allowed-Origin` with the same origin or an asterisk. The label-service publishes CloudEvents, so every partner endpoint in this module must answer OPTIONS.

Event Grid schema: Event Grid sends a `SubscriptionValidationEvent` with the `aeg-event-type` header set to `SubscriptionValidation`. The endpoint returns HTTP 200 with the validation code in the body within 30 seconds.

The asynchronous path: if the endpoint can't answer synchronously, the event data carries a `validationUrl` that you open with a GET request within 10 minutes.

Automatic handshakes: Azure Functions with the Event Grid trigger and Azure Logic Apps run the handshake for you, which is one reason internal consumers usually use the trigger.

Common failures: HTTP 202 doesn't count as a valid answer, and self-signed certificates aren't supported for validation.
