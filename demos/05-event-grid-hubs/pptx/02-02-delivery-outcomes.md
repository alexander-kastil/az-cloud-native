---
slide: "02-02"
topic: 02-filters-retries
title: "Dead-lettering and errors without retries"
subtitle: "Where an event ends up when delivery fails"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/05-event-grid-hubs/pptx/images/02-02-delivery-outcomes.svg
visual-prompt: >
  Opus 5.5, hand-written SVG in the module's diagram style, viewBox 0 0 1920 1080, slate #F8FAFC canvas
  with one white panel bordered #94A3B8 2px, 'Segoe UI', Inter, Arial, sans-serif, headings #0F172A,
  secondary text #64748B, minimum 26px type, arrows #334155. No heading inside the image; the slide
  title carries it. No arrow crosses a box or a label. A flowchart in four columns. Amber #FEF3C7 "Event
  Grid delivers the event" leads to the grey #F1F5F9 question "HTTP 200 to 204?"; yes goes right to green
  #DCFCE7 "Delivered"; no goes down to "Webhook 400, 401, 403, 413 or Azure resource 400, 403, 413?".
  Yes, no retry, goes right to "Dead-letter container configured?"; no goes down to amber "Wait at least"
  with "Azure resource 401 or 404: 5 minutes", "408: 2 minutes", "503: 30 seconds", "any other code: 10
  seconds", then right to "Attempts or time to live used up?". Yes goes up to the dead-letter question;
  no loops along the bottom back to the first box. The dead-letter question: yes to purple #EDE9FE
  "Dead-lettered, written 5 minutes after the last attempt", no to grey "Dropped". A note at the bottom
  right: "On probation, an event can be dead-lettered or dropped without a delivery attempt".
order: 7
source: demos/05-event-grid-hubs/02-filters-retries/readme.md
gamma_prompt: >
  Slide titled "Dead-lettering and errors without retries", subtitle "Where an event ends up when
  delivery fails". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dead-lettering and errors without retries

## Content

- Dead-lettering is off by default: give the subscription a **blob container**
- Webhook 400, 401, 403 and 413 and Azure resource 400, 403 and 413 are never retried
- The dead-letter write comes five minutes after the last attempt
- Each event in the blob carries `deadLetterReason`, `deliveryAttempts` and `lastDeliveryOutcome`
- On **probation**, an event can be dead-lettered without a delivery attempt

## Speaker Notes

Retries end somewhere, and this slide shows where an event lands: delivered, dead-lettered or dropped. The flow follows one event from the delivery attempt to its outcome.

A container to switch it on: dead-lettering is off by default, so create a blob container and give the subscription its full resource path. With a managed identity, grant that identity a write role on the storage account.

Errors without retry: Event Grid treats only HTTP 200 to 204 as success, and these status codes mean that delivery will never succeed. Event Grid dead-letters such an event at once, or drops it when no dead-letter location exists.

Timing: the write comes five minutes after the last attempt, and the event is dropped if the container stays unavailable for four hours. Other codes retry after their own minimum waits, such as two minutes for 408 and 30 seconds for 503.

What the blob holds: an array of events, each with `deadLetterReason`, usually `MaxDeliveryAttemptsExceeded`, plus `deliveryAttempts` and `lastDeliveryOutcome`. Blob names contain the subscription name in upper case.

Probation: when deliveries keep failing, Event Grid puts the subscription on probation, and events can be dead-lettered or dropped without an attempt. Check the dead-letter container first when a partner reports missing events.
