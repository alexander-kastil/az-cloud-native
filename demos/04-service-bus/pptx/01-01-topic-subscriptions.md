---
slide: "01-01"
topic: 01-subscriptions-filters
title: "Topics, subscriptions and filters"
subtitle: "Each subscription keeps its own copy of the events it selects"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/01-subscriptions-filters/_images/topic-copies.svg"
media-file: demos/04-service-bus/01-subscriptions-filters/_images/topic-copies.svg
visual-prompt: >
  Existing diagram of the parcel-events topic copying each event into the tracking, customer-messages and exceptions subscriptions through correlation filters on Subject, no generation. Referenced at its repo path.
order: 3
source: demos/04-service-bus/01-subscriptions-filters/readme.md
gamma_prompt: >
  Slide titled "Topics, subscriptions and filters", subtitle "Each subscription keeps its own copy of
  the events it selects". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Topics, subscriptions and filters

## Content

- Each **subscription** receives a copy of every message its rules match
- `tracking` gets `ParcelScanned`, `customer-messages` gets `ParcelDelivered` and `DeliveryDelayed`, `exceptions` gets `DeliveryDelayed`
- A slow consumer builds a backlog on its own subscription only
- On Azure, every new subscription starts with a **default rule** that matches every message
- Rules on one subscription combine with OR, so delete the default rule

## Speaker Notes

Contoso Logistics publishes every parcel event to one topic, `parcel-events`. Three services need different events, so each one gets its own subscription with rules that select them.

Copies per subscription: a topic accepts messages from publishers, and each subscription receives a copy of every message that matches its rules. One scan can reach several consumers, and each subscription also has its own dead-letter queue.

Three subscriptions: `tracking` feeds the tracking service, `customer-messages` the notification service, and `exceptions` the exception agent. Each one uses a correlation filter on `Subject`.

Separate backlogs: each subscription holds its own copy of the messages it selects, so a slow consumer builds a backlog on its own subscription and doesn't hold up the others.

Default rule: each new subscription on Azure starts with a default rule whose true filter matches every message. The Service Bus emulator doesn't add one.

OR between rules: rules on one subscription combine with a logical OR, so the default rule keeps selecting everything until you delete it.
