---
slide: "02-01"
topic: 02-service-invocation
title: "Calling a service by app ID"
subtitle: "The sidecar resolves routing-service to a running instance"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/07-dapr/_images/invoke-by-app-id.svg"
media-file: labs/07-dapr/_images/invoke-by-app-id.svg
visual-prompt: >
  Existing diagram of shipping-api calling InvokeMethodAsync with the app ID routing-service and its sidecar resolving the app ID to a running instance, no generation. Referenced at its repo path.
order: 5
source: demos/07-dapr/02-service-invocation/readme.md
gamma_prompt: >
  Slide titled "Calling a service by app ID", subtitle "The sidecar resolves routing-service to a
  running instance". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Calling a service by app ID

## Content

- Every Dapr app has an **app ID**, shared by all of its replicas
- Callers use `/v1.0/invoke/routing-service/method/reserve-capacity` on the sidecar
- The sidecar resolves the ID through Kubernetes DNS, or mDNS when self-hosted
- `InvokeMethodAsync` handles serialization, `CreateInvokeHttpClient` returns an `HttpClient`
- The `HttpRequestMessage` overloads are obsolete in the current SDK

## Speaker Notes

When `shipping-api` books a parcel, it asks `routing-service` for van capacity and waits for the answer. Service invocation lets the sidecar find the target, so no service carries its own discovery logic.

App IDs: callers use the app ID in place of a host name, and every replica of `routing-service` answers to the same ID. Naming app IDs after the Contoso services keeps the URLs in traces readable.

The invoke URL: the segment after `invoke` is the app ID and the segment after `method` is the route on the callee.

Name resolution: on Kubernetes the sidecar uses Kubernetes DNS, on a self-hosted machine it uses mDNS. In Azure Container Apps you set the ID with `--dapr-app-id`, and the diagram shows that the call stays the same there.

Two .NET options: the typed `InvokeMethodAsync` overload serializes the request and deserializes the response. `CreateInvokeHttpClient` suits code that already uses typed HTTP clients.

Obsolete overloads: the overloads that take an `HttpRequestMessage` are marked obsolete, so the helper is the supported path for raw HTTP calls.
