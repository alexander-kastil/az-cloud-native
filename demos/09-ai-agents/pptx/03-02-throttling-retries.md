---
slide: "03-02"
topic: 03-durable-llm-calls
title: "Retries on throttling"
subtitle: "Which responses retry, and how a retry reuses the stored result"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/09-ai-agents/pptx/images/03-02-throttling-retries.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif, module palette, minimum 27px type. No heading inside the image. No arrow crosses a box or a
  label. Left column, four white response boxes with a bold line and a grey reason: "429 with
  retry-after-ms" "capacity returns after the indicated delay"; "408 from a provisioned deployment" "the
  request timed out in the deployment queue"; "5xx" "transient service failure"; "400 with code
  content_filter" "the same content fails again". The first three arrows lead to one tall green #DCFCE7
  box "Retry in the activity" "RetryPolicy supplies the backoff" "a 429 retry starts only after the
  retry-after-ms value has passed" with a white chip "maxNumberOfAttempts: 5" "counts the first call, so
  it allows four retries"; the fourth leads to amber #FEF3C7 "Record the failure and stop" "no retry for
  the same content". Bottom panel "Replay safety: the result is stored under the event id" with three
  chips in a row: "Attempt 1" "drafts and writes the draft under the event id", amber "Crash" "before
  it reports success", green "Attempt 2" "finds the event id and returns the stored draft".
order: 9
source: demos/09-ai-agents/03-durable-llm-calls/readme.md
gamma_prompt: >
  Slide titled "Retries on throttling", subtitle "Which responses retry, and how a retry reuses the
  stored result". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Retries on throttling

## Content

- **429** carries retry-after-ms, and the next attempt waits at least that long
- **408** from a provisioned deployment and **5xx** retry as transient failures
- **400** with code content_filter fails again, so the activity records the failure and stops
- An attempt can crash after the model answered, and the runtime runs the activity again
- The activity stores its result under the event id and returns it on the next attempt

## Speaker Notes

A throttled model call is the common failure, and this slide sorts the responses the activity sees into retry and stop. The sorting follows the throttling guidance in the Azure Well-Architected Framework.

429: the tokens-per-minute or requests-per-minute budget is spent, and the response carries retry-after-ms and retry-after headers. The retry policy supplies the backoff, and the next attempt should not start before the header value has passed.

408 and 5xx: a 408 from a provisioned deployment means the request timed out in the deployment queue, and a 5xx is a transient service failure, so both retry. A 400 with code content_filter fails again with the same content, so the activity records the failure and stops.

Replay safety: an activity can receive the model's answer and crash before it reports success, and the runtime then runs it again. Because the activity writes its result under the event id and returns the stored result when that key exists, the second attempt makes no new model call.
