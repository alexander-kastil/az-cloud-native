---
slide: "05-02"
topic: 05-resiliency
title: "How the circuit breaker opens and closes"
subtitle: "cbPolicy trips after five consecutive failures"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/07-dapr/05-resiliency/_images/circuit-breaker.svg"
media-file: demos/07-dapr/05-resiliency/_images/circuit-breaker.svg
visual-prompt: >
  Existing diagram of calls from the shipping-api sidecar to routing-service over time, where five consecutive failures open cbPolicy for 60 s and one half-open probe closes it again, no generation. Referenced at its repo path.
order: 12
source: demos/07-dapr/05-resiliency/readme.md
gamma_prompt: >
  Slide titled "How the circuit breaker opens and closes", subtitle "cbPolicy trips after five
  consecutive failures". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# How the circuit breaker opens and closes

## Content

- `trip: consecutiveFailures >= 5` opens the breaker
- While open, the breaker rejects calls at once for its `timeout` of 60 s
- Half-open lets `maxRequests` (1) probe through, and a success closes it
- `interval` stays at 0 s, so nothing clears the count while the breaker is closed
- Dapr creates breakers per component and per app instance

## Speaker Notes

A timeout ends one attempt and a retry repeats it, while a circuit breaker stops attempts entirely once a target has failed too often. That gives the target room to recover.

Trip condition: the `trip` field holds a condition such as `consecutiveFailures >= 5`. In the diagram, the four failed attempts of call 1 and the first attempt of call 2 make five failures in a row.

Open: the breaker's `timeout` controls how long it stays open before it allows a probe, and the lab's `cbPolicy` keeps the default of 60 seconds. During that time the retry of call 2 and every new call are rejected at once.

Half-open: `maxRequests` limits how many requests pass while the breaker probes, with a default of 1. One successful probe closes the breaker again.

Interval: the lab's policy sets only `trip`, so `interval` keeps its default of 0 seconds, which never clears the failure count while the breaker is closed.

Scope: Dapr creates circuit breakers per component and per app instance, so one unhealthy replica does not block the whole service.
