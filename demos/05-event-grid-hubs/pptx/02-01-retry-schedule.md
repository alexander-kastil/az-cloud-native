---
slide: "02-01"
topic: 02-filters-retries
title: "Filters and the retry schedule"
subtitle: "Which events reach a subscription, and how long Event Grid keeps trying"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/05-event-grid-hubs/02-filters-retries/_images/retry-timeline.svg"
media-file: demos/05-event-grid-hubs/02-filters-retries/_images/retry-timeline.svg
visual-prompt: >
  Existing diagram of Event Grid delivery attempts on a log time axis over 24 hours, with a one-hour partner outage and time to live cut lines at 30 minutes, 12 hours and 24 hours, no generation. Referenced at its repo path.
order: 6
source: demos/05-event-grid-hubs/02-filters-retries/readme.md
gamma_prompt: >
  Slide titled "Filters and the retry schedule", subtitle "Which events reach a subscription, and how
  long Event Grid keeps trying". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Filters and the retry schedule

## Content

- Filter on **event type**, the start or end of the subject, and fields under `data`
- The **retry policy** allows 1 to 30 attempts and a time to live of 1 to 1440 minutes
- Event Grid stops at whichever limit it reaches first
- Waits grow from 10 seconds to every 12 hours, plus a small random offset
- Order isn't guaranteed and a retry can duplicate, so handlers accept both

## Speaker Notes

A partner's webhook goes down for an hour on the busiest day of the year, and `ParcelDelivered` must not disappear without a trace. The timeline shows the attempts during that outage and where each time to live cuts them off.

Filters: they run before delivery, so a subscription receives only the events it asks for. Advanced filters compare a field such as `data.region` with values, ignore case, and can't look inside an array of objects.

Retry policy: maximum attempts default to 30 and the event time to live to 1440 minutes, which is 24 hours. Set both on the subscription with the Azure CLI or on the Additional features tab of the portal.

First limit wins: the time to live is checked only at the next scheduled attempt, so with 30 minutes only six attempts fit and a maximum of 10 attempts has no effect.

The schedule: Event Grid waits 30 seconds for a response, then retries after 10 seconds, 30 seconds, 1 minute and onward up to every 12 hours. It can skip retries for an endpoint that stays unhealthy, so the schedule is best effort.

Order and duplicates: Event Grid doesn't guarantee order, and a retry can produce a duplicate, so every partner handler must tolerate out-of-order events and repeats.
