---
slide: "00-01"
topic: 05-event-grid-hubs
title: "Events and streams with Event Grid and Event Hubs"
subtitle: "Connecting Contoso Logistics to partners and the live map"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/05-event-grid-hubs/pptx/images/00-01-event-streams.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers in their late twenties, a woman and a man in modern work-casual
  clothes, stand in front of a large wall-mounted display in the bright operations area of a logistics
  software team. The display shows a softly blurred city map with small coloured dots and thin route
  lines; the woman points at one dot while the man holds an open laptop showing soft blurred coloured
  lines, both engaged and smiling. People centred in the frame, the display beside them. Modern, light,
  bright and friendly: high-key natural daylight, white and light-oak contemporary interior, soft pastel
  accents in light blue, mint and lavender, green plants, 2020s design. Screens show only soft blurred
  shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary editorial photography.
  Avoid: readable text, map labels or street names, logos, watermarks, extra people, dark or moody
  lighting, vintage or retro decor, grime, clutter.
order: 1
source: demos/05-event-grid-hubs/readme.md
gamma_prompt: >
  Title slide "Events and streams with Event Grid and Event Hubs" with subtitle "Connecting Contoso
  Logistics to partners and the live map". Side-accent layout: title and subtitle on the left half on
  plain white, the provided photograph of two engineers at a live map display filling the right half, no
  text over the photo. Use exactly the one image given for this card; do not generate or add any other
  images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Events and streams with Event Grid and Event Hubs

## Content

- **Discrete events**: publish `ParcelDelivered` with Event Grid, then filter, retry and pull it
- **Telemetry**: stream van positions through Event Hubs partitions, checkpoints and Kafka
- **Push to browsers**: SignalR Service brings each position to the ops console

## Speaker Notes

This opening slide sets out the three themes of the module. Module 4 taught the queues that carry commands between services, and this module adds the broadcast side.

Discrete events: the label-service publishes `ParcelDelivered` as a CloudEvent to Event Grid, and partners and functions react to it. Filters, retries, dead-lettering and namespace pull delivery decide how each partner receives it.

Telemetry: every van reports its GPS position as a steady stream into Event Hubs. Partition keys, consumer groups and checkpoints keep the results correct, and the Kafka endpoint lets telematics vendors keep their Kafka clients.

Push to browsers: Azure SignalR Service pushes each position to the ops console, so a dispatcher watches vans move across the map without a page reload.
