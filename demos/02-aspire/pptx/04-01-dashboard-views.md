---
slide: "04-01"
topic: 04-dashboard
title: "Dashboard views"
subtitle: "Resources, logs, traces and metrics in one browser tab"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/02-aspire/pptx/images/04-01-dashboard-views.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers in a bright glass-walled meeting room: a woman in her early thirties
  stands at a large wall-mounted screen and points at the longest of several stacked horizontal bars of
  a softly blurred timeline chart, while a man in his late twenties sits at a light-oak table with an
  open laptop and looks up at the bar she is pointing at, both focused and at ease, modern work-casual
  clothes. The wall screen faces the camera and is never blank. People on the right half, bright glass
  wall and daylight on the left half as space for text. Modern, light, bright and friendly: high-key
  natural daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint
  and lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never
  readable UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text,
  logos, watermarks, dark or moody lighting, vintage or retro decor, grime, clutter, extra people.
order: 9
source: demos/02-aspire/04-dashboard/readme.md
gamma_prompt: >
  Slide titled "Dashboard views", subtitle "Resources, logs, traces and metrics in one browser tab".
  Side-accent layout: title, subtitle and four bullets on the left half on plain white, the provided
  photograph of two engineers at a wall screen in a bright meeting room filling the right half, no text
  over it. Use exactly the one image given for this card; do not generate or add any other images, icons
  or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dashboard views

## Content

- **Resources** shows state and health, and restarts one resource without the rest
- **Console** and configuration show why a service failed to start
- **Structured logs** keep `TrackingNumber` as a field the dashboard filters on
- **Metrics** find the time window of a slowdown, a trace finds the request

## Speaker Notes

A parcel scan that takes four seconds to reach the tracking page could lose that time in any of several services, and the console of each terminal rarely shows where. The Aspire dashboard puts the resources, logs, traces and metrics of the running system in one browser tab.

Resources: the page lists every resource from the AppHost with its state and health. A developer can stop, start or restart a single resource without stopping the rest of the Contoso system.

Console and configuration: console output shows standard output and standard error of each process, and the configuration view shows the settings a resource received. When a service fails to start, its state shows whether the process crashed or a health check failed.

Structured logs: each field stays data, and the message template defines the field names. Every service that logs a `TrackingNumber` field gives the dashboard the same key to filter by, without parsing text.

Metrics: the defaults register ASP.NET Core, `HttpClient` and runtime metrics, charted per resource. The chart finds the time window of a slowdown, and the trace finds the request and the step that caused it.
