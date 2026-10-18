---
slide: "06-02"
topic: 06-testing
title: "Testing the booking flow end to end"
subtitle: "Wait for health, book a shipment, poll until the parcel appears"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/02-aspire/06-testing/_images/e2e-polling.svg"
media-file: demos/02-aspire/06-testing/_images/e2e-polling.svg
visual-prompt: >
  Existing diagram of the end-to-end test waiting until shipping-api and tracking-service are healthy, booking a shipment and polling tracking-service until the parcel appears in the read model before the deadline, no generation. Referenced at its repo path.
order: 15
source: demos/02-aspire/06-testing/readme.md
gamma_prompt: >
  Slide titled "Testing the booking flow end to end", subtitle "Wait for health, book a shipment, poll
  until the parcel appears". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Testing the booking flow end to end

## Content

- `WaitForResourceHealthyAsync` waits for each resource the flow uses
- Health waits replace a guessed `Task.Delay` startup time
- The test books through shipping-api, then **polls** tracking-service with a deadline
- One assertion per flow stage, so a failure names the stage that broke

## Speaker Notes

A started application is not yet a ready application, so the end-to-end test waits before it sends its first request.

Health waits: the booking flow needs shipping-api and tracking-service, so the test waits for both. Each waited resource adds startup time, so the test waits only for the resources that the flow uses.

No fixed delay: a `Task.Delay` guesses a startup time, and the guess passes on a fast laptop and fails on a busy build agent.

Polling: the booking happens asynchronously, so the test polls tracking-service until the parcel appears, which proves the HTTP call, the event on the topic, the subscription and the read model. A deadline that is too short fails on a slow build agent, and one that is too long hides a stuck flow.

One assertion per stage: a failure then names the stage that broke. The endpoint paths belong to the service code, and later modules add them.
