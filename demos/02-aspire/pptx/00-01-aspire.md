---
slide: "00-01"
topic: 02-aspire
title: "Building distributed apps with Aspire"
subtitle: "Start, trace and test the Grey system from one AppHost"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/02-aspire/pptx/images/00-01-aspire.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers in their late twenties, a woman and a man in modern work-casual
  clothes, in a bright open team area beside a large whiteboard wall: he holds a marker next to a
  hand-drawn sketch of plain boxes joined by arrows with no words on it, she holds an open laptop up
  beside the sketch with its screen facing the camera, showing a softly blurred light map dotted with
  small coloured markers for delivery vans, and both smile as they compare the drawing with the running
  system. Modern, light, bright and friendly: high-key natural daylight, white and light-oak
  contemporary interior, soft pastel accents in light blue, mint and lavender, green plants, 2020s
  design. Screens show only soft blurred shapes and colours, never readable UI. 16:9 landscape
  1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks, dark or moody
  lighting, vintage or retro decor, grime, clutter, extra people, the back of the laptop.
order: 1
source: demos/02-aspire/readme.md
gamma_prompt: >
  Title slide "Building distributed apps with Aspire" with subtitle "Start, trace and test the Grey
  system from one AppHost". Side-accent layout: title and subtitle on the left half on plain white, the
  provided photograph of two engineers comparing a whiteboard sketch with a laptop filling the right
  half, no text over the photo. Use exactly the one image given for this card; do not generate or add
  any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Building distributed apps with Aspire

## Content

- **Model the system**: one AppHost starts every Grey service and resource in order
- **Connect and observe**: shared defaults, emulated Azure resources and one dashboard
- **Extend and test**: Angular and Python apps in the same AppHost, tests that start it

## Speaker Notes

This opening slide sets out the three themes of the module: Module 1 decided where the service boundaries sit, and this module turns them into a running Grey system.

Model the system: the AppHost describes the shipping API, the tracking service, the message bus and the database in C#, and one command starts them in the right order.

Connect and observe: ServiceDefaults gives every service the same telemetry, retries and discovery, and integrations connect Service Bus and Cosmos DB through local emulators. The dashboard then follows one booking across services.

Extend and test: the Angular ops console and the Python exception agent run under the same AppHost. Distributed integration tests start the real AppHost and check the event flow end to end.
