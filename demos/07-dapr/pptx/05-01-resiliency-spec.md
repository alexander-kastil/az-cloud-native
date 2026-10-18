---
slide: "05-01"
topic: 05-resiliency
title: "The resiliency spec"
subtitle: "Named policies in one file, applied to calls to routing-service"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/07-dapr/_images/resiliency-routing-calls.svg"
media-file: labs/07-dapr/_images/resiliency-routing-calls.svg
visual-prompt: >
  Existing diagram of the shipping-api sidecar loading resiliency.yaml and applying the general timeout, retryPolicy and cbPolicy to every call to routing-service, no generation. Referenced at its repo path.
order: 11
source: demos/07-dapr/05-resiliency/readme.md
gamma_prompt: >
  Slide titled "The resiliency spec", subtitle "Named policies in one file, applied to calls to
  routing-service". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The resiliency spec

## Content

- `policies` names timeouts, retries and circuit breakers; `targets` attaches them
- `general`: each attempt waits at most 5 s
- `retryPolicy`: constant 2 s delay, 3 retries, only status codes 500 to 599
- A full call can take the timeout times the attempts, plus the delays
- The sidecar loads the file only when the AppHost sets `ResourcesPaths`

## Speaker Notes

On a busy evening `routing-service` slows down, and every waiting request from `shipping-api` holds a connection and a thread. The fix is a small set of rules: give up after a fixed time, retry a few times with spacing, and stop calling a service that keeps failing.

Two parts: the `policies` section defines named timeouts, retry policies and circuit breakers, and the `targets` section attaches those names to what Dapr calls. Change a policy in one place and every target that uses it follows.

Timeout: `shipping-resiliency` limits each attempt to five seconds, and a timeout that fires ends only that attempt.

Retry: the policy retries a failed attempt up to three times with a constant two-second delay, only for HTTP 500 to 599. The diagram shows the lab's version, where a 409 conflict returns at once without a retry.

Total wait: the full call can take the timeout multiplied by the number of attempts, plus the delays between them. Set the timeout for one attempt, then budget the total.

Loading the file: the sidecar reads the spec only when the AppHost sets `ResourcesPaths` in the sidecar options to the folder that holds it.
