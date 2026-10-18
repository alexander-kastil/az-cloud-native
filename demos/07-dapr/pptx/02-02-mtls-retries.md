---
slide: "02-02"
topic: 02-service-invocation
title: "Mutual TLS and retries"
subtitle: "Sentry issues the certificates, the resiliency policy decides the retries"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/02-02-retry-replica.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette, minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. Left to right: shipping-api "calls reserve-capacity" points at the Dapr sidecar of shipping-api
  "resolves app ID routing-service, retry from the resiliency policy". Two elbow arrows leave the
  sidecar: "attempt 1" to a red routing-service sidecar, replica 1, with a red cross badge "replica
  fails"; "retry" to a blue routing-service sidecar, replica 2, which points at routing-service replica 2
  with a green CapacityAssignment chip below. A green pill with a lock glyph "mTLS between sidecars"
  sits between the two replicas. A purple Sentry box at the top "issues and rotates the certificates"
  with dashed lines to the sidecars. Two grey notes at the bottom: "AKS Dapr extension: mTLS by default,
  24 h certificates, 15 min clock skew" and "Container Apps: invocation between Dapr apps encrypted by
  the platform".
order: 6
source: demos/07-dapr/02-service-invocation/readme.md
gamma_prompt: >
  Slide titled "Mutual TLS and retries", subtitle "Sentry issues the certificates, the resiliency
  policy decides the retries". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Mutual TLS and retries

## Content

- **Sentry** issues and rotates the certificates; application code never handles them
- The AKS extension turns mTLS on by default: 24 h certificates, 15 min clock skew
- Container Apps encrypts invocation between Dapr-enabled apps through the platform
- Retries with backoff come from the resiliency policy for `routing-service`
- When the caller needs no answer, publish an event instead

## Speaker Notes

The call to `routing-service` has to travel over an encrypted connection and survive a brief outage of one replica. Both come from the platform, not from code in the caller.

Sentry: Dapr secures every call between applications with mutual TLS, and the Sentry service issues and rotates the certificates.

AKS defaults: the AKS Dapr extension enables mTLS by default with a workload certificate lifetime of 24 hours and a clock skew allowance of 15 minutes. Operations should know which certificate settings apply to each environment.

Container Apps: invocation between Dapr-enabled apps is encrypted through the platform, and your code configures none of it.

Retries: service invocation retries with backoff, driven by a resiliency spec that targets the app ID `routing-service`. The policies skip streaming requests, and the Container Apps service discovery resiliency feature does not apply to Dapr service invocation.

Invocation or pub/sub: invocation is synchronous, so a slow callee slows the caller. When the caller needs no answer, such as a `ParcelScanned` event for the read model, pub/sub is the better fit.
