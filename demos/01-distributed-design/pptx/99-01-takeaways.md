---
slide: "99-01"
topic: 01-distributed-design
title: "Takeaways"
subtitle: "What to carry into the Aspire module"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/01-distributed-design/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Four software engineers, two women and two men in their late twenties to mid thirties in
  modern work-casual clothes, in a bright modern lounge on a light sofa and armchairs around a low
  light-oak table. One engineer turns her open laptop toward the others so they can see a softly blurred
  diagram of coloured boxes and arrows on its screen; two colleagues lean in smiling and nodding, and
  one holds a coffee cup. The laptop screen faces the colleagues and the camera at an angle, never its
  back. Group on the right half, bright calm window light on the left half as space for text. Modern,
  light, bright and friendly: high-key natural daylight, white and light-oak contemporary interior,
  soft pastel accents in light blue, mint and lavender, green plants, 2020s design. Screens show only
  soft blurred shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary editorial
  photography. Avoid: readable text, logos, watermarks, extra people, dark or moody lighting, vintage or
  retro decor, grime, clutter.
order: 20
source: demos/01-distributed-design/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into the Aspire module". Side-accent layout:
  title, subtitle and five bullets on the left half on plain white, the provided photograph of a team in
  a bright lounge looking at a laptop one engineer turns toward them filling the right half, no text
  over it. Use exactly the one image given for this card; do not generate or add any other images,
  icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Start with a modular monolith and split when team, deployment, scaling or data demand it
- Use HTTP when the caller needs the answer, messages when the work only has to happen
- Draw boundaries where the business language changes
- Publish integration events after the commit, named after business facts
- Protect every flow with a pattern, and pick the broker by ordering, then retention

## Speaker Notes

Close with the decisions this module prepared, each one made before anyone writes a service.

Monolith first: a modular monolith keeps the seams in the code and in the database. A module moves out when it needs its own scaling, release train or team.

HTTP or messages: start with the question the caller asks. HTTP serves an answer the caller needs to continue, a command or an event serves work that only has to happen.

Boundaries: event storming with the domain experts finds the bounded contexts, and a microservice spans at most one of them.

Events: integration events leave only after the entity is persisted, carry a business name such as ParcelDelivered, and travel in a CloudEvents envelope on Event Grid and Dapr.

Patterns and brokers: outbox, idempotent consumer, saga, retries and dead-letters protect each flow, and ordering and retention decide the broker. Module 2 turns these boundaries into a running system with Aspire.
