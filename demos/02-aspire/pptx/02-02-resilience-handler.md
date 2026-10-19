---
slide: "02-02"
topic: 02-service-defaults
title: "The standard resilience handler"
subtitle: "Five stages between the booking code and shipping-api"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/02-aspire/02-service-defaults/_images/standard-resilience-pipeline.svg"
media-file: demos/02-aspire/02-service-defaults/_images/standard-resilience-pipeline.svg
visual-prompt: >
  Existing diagram of the five stages of the standard resilience handler between the booking code and shipping-api, with the total timeout spanning every retry, no generation. Referenced at its repo path.
order: 6
source: demos/02-aspire/02-service-defaults/readme.md
gamma_prompt: >
  Slide titled "The standard resilience handler", subtitle "Five stages between the booking code and
  shipping-api". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The standard resilience handler

## Content

- `AddStandardResilienceHandler` adds retries, timeouts and a circuit breaker
- The **total timeout** of 30 s spans every retry; each attempt gets 10 s
- Retries replay requests, so the booking call to shipping-api must be **idempotent**
- Request clients through `IHttpClientFactory` or a typed client to get the handler

## Speaker Notes

Calls between Grey services fail for ordinary reasons, such as a restarting container or a slow database, and the standard resilience handler absorbs them on every `HttpClient` that the factory creates.

Five stages: a call passes the rate limiter, the total timeout, the retry, the circuit breaker and the attempt timeout, from the outermost to the innermost stage. The retry stage allows at most 3 retries with exponential backoff and jitter.

Timeouts: the total timeout of 30 s covers all retries and the attempt timeout caps each try at 10 s. A slow dependency therefore never holds a request open indefinitely.

Idempotency: retries replay requests, so a booking call to the shipping API must be idempotent, and the wait between attempts spares a recovering service a burst of requests. The reliability patterns topic in Module 1 covers that design rule.

Factory clients: a bare `new HttpClient()` outside the factory gets no retries. The booking code requests its client through `IHttpClientFactory` or a typed client.
