---
slide: "00-01"
topic: 04-service-bus
title: "Reliable messaging with Service Bus"
subtitle: "Parcel events from the shipping API and scanners to their consumers"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/04-service-bus/pptx/images/00-01-service-bus.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Two software engineers in their late twenties, a woman and a man in modern work-casual
  clothes, at a light-oak workbench in the bright product team studio of a last-mile parcel delivery
  company: she aims a handheld parcel scanner at a plain brown cardboard parcel, its red aiming light on
  the carton, while he leans in to the open laptop beside her, whose screen shows a column of
  soft-coloured rows with a new row sliding in at the top as the scan arrives; both are smiling and
  engaged. Two more plain parcels stand stacked beside the laptop. The pair in the centre of the frame.
  Every carton is plain brown cardboard on all sides, the scanner's small display glows a plain soft
  green, and the laptop is a plain laptop with no logo on its lid. Modern, light, bright and friendly:
  high-key natural daylight, white and light-oak contemporary interior, soft pastel accents in light
  blue, mint and lavender, green plants, 2020s design. Screens show only soft blurred shapes and
  colours, never readable UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid:
  readable text, logos, watermarks, extra people, dark or moody lighting, vintage or retro decor,
  grime, clutter.
order: 1
source: demos/04-service-bus/readme.md
gamma_prompt: >
  Title slide "Reliable messaging with Service Bus" with subtitle "Parcel events from the shipping API
  and scanners to their consumers". Side-accent layout: title and subtitle on the left half on plain
  white, the provided photograph of two engineers scanning a parcel at a bright workbench filling the
  right half, no text over the photo. Use exactly the one image given for this card; do not generate or
  add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Reliable messaging with Service Bus

## Content

- **Routing and consuming**: filtered subscriptions, a Functions trigger and consumers scaled by KEDA
- **Order and poison messages**: one session per parcel, unreadable label scans in the dead-letter queue
- **Duplicates**: duplicate detection at the broker and an idempotent consumer

## Speaker Notes

This module connects the Grey Logistics services with Azure Service Bus, which carries parcel events from the shipping API and the scanners to the consumers that need them. Every SDK call uses `Azure.Messaging.ServiceBus`, because the legacy `Microsoft.Azure.ServiceBus` and `WindowsAzure.ServiceBus` packages retired on September 30, 2026.

Routing and consuming: one topic feeds filtered subscriptions, and a Functions trigger or Container Apps replicas consume them. KEDA scales the consumer with its backlog.

Order and poison messages: sessions keep the scans of one parcel in order, and unreadable label scans move to the dead-letter queue, where an operator can inspect them.

Duplicates: duplicate detection drops a repeated send at the broker, and an idempotent consumer stores each scan once even when it arrives twice.
