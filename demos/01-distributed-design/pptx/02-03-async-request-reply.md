---
slide: "02-03"
topic: 02-sync-async
title: "When HTTP is the right choice"
subtitle: "The Asynchronous Request-Reply pattern for long-running work"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/02-03-async-request-reply.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, participants blue #DBEAFE with #1D4ED8 borders, text
  #0F172A, arrows #334155 3px, status replies purple #6D28D9, minimum 26px type. No heading inside the
  image. No arrow crosses a box or a label. The sequence from the topic readme between Client and
  Service with dashed lifelines: "Start the long-running work"; dashed purple reply "202 Accepted with
  Location and Retry-After headers"; a grey loop frame tabbed "loop: until the work is finished" around
  "GET the Location URL after Retry-After" and the dashed reply "Work still pending"; then "GET the
  Location URL", dashed purple reply "303 See Other pointing to the finished result", "GET the finished
  result" and the dashed reply "Finished result". Labels sit centred above their arrows.
order: 7
source: demos/01-distributed-design/02-sync-async/readme.md
gamma_prompt: >
  Slide titled "When HTTP is the right choice", subtitle "The Asynchronous Request-Reply pattern for
  long-running work". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# When HTTP is the right choice

## Content

- The caller needs the answer for its next step, such as a **shipment ID**
- A **query** asks for current data
- Long-running work returns **202 Accepted** with Location and Retry-After
- The client polls until **303 See Other** points to the finished result
- If the caller only needs the work to happen, publish a command or an event

## Speaker Notes

A broker removes temporal coupling, yet HTTP stays the right choice in three cases. The diagram shows the third, long-running work that the caller polls.

Answer for the next step: a validation error or a shipment ID decides what the app shows next, so the caller waits for it.

Query: a request for current data changes nothing and is safe to repeat.

202 Accepted: the service accepts the work at once and returns a Location header and a Retry-After header. While the work is pending, the client calls the Location URL again after each Retry-After interval.

303 See Other: once the work is finished, the Location URL redirects the client to the finished result, which it then fetches.

The deciding question: if the caller needs the answer to continue, use HTTP; if it only needs the work to happen, publish a command or an event. Messaging brings eventual consistency, duplicates and out-of-order processing, which the reliability topic handles.
