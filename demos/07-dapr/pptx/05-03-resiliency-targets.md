---
slide: "05-03"
topic: 05-resiliency
title: "Resiliency targets"
subtitle: "Policies keyed by app ID, actor type or component name"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/05-03-resiliency-targets.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette, minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. Left: blue "Your app" and amber "pubsub broker" boxes with arrows into a tall blue "Dapr sidecar"
  box. From the sidecar a vertical trunk fans out to four target rows on the right, each branch labelled
  above its arrow: apps.routing-service to a routing-service sidecar box and on to routing-service;
  actors.VanActor to a purple VanActor box; components.statestore.outbound to a green statestore box;
  components.pubsub.inbound to a blue "Your app, subscription handler" box. Component names in pink.
order: 13
source: demos/07-dapr/05-resiliency/readme.md
gamma_prompt: >
  Slide titled "Resiliency targets", subtitle "Policies keyed by app ID, actor type or component name".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Resiliency targets

## Content

- `apps` keys policies by app ID, `actors` by actor type, `components` by component name
- `outbound` protects the call to the store, `inbound` the delivery to your app
- Container Apps offers **Dapr component resiliency** as a preview feature
- `az containerapp env dapr-component resiliency create` attaches a policy to a component
- Service discovery resiliency does not apply to Dapr service invocation

## Speaker Notes

A target decides where a policy applies. The same named policies can protect a peer service, an actor type, a store and a broker.

Three sections: the `apps` section keys entries by app ID, the `actors` section by actor type such as `VanActor`, and the `components` section by component name.

Direction: component targets split into `outbound` for calls from the sidecar to the component, such as the state write, and `inbound` for delivery of a subscription message from the component to your app.

Container Apps: the preview feature offers the same policy model for components on a managed Dapr environment, with timeouts, HTTP retries and circuit breakers. You attach a policy to a component already registered on the environment.

The CLI: the command attaches, for example, an outbound timeout to the component `pubsub`, then you restart the Dapr applications. The portal supports only timeout and retry policies, so check circuit breaker behaviour against your CLI version.

Service discovery: that separate Container Apps feature covers requests addressed by container app name, not calls through the Dapr Service Invocation API. Dapr calls use the Dapr spec or the component policy.
