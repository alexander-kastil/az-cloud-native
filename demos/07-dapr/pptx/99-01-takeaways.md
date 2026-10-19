---
slide: "99-01"
topic: 07-dapr
title: "Takeaways"
subtitle: "What to carry into durable execution"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/07-dapr/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. A small software team in the bright team room of a modern parcel delivery company at the end
  of a workshop day, a few plain brown cardboard parcels stacked on a side shelf. A young woman stands at
  a large whiteboard and draws with a marker: a larger rounded box with a smaller box docked at its side,
  and below them a row of simple shapes, a cylinder, a square and a circle. A fresh line runs from the
  small box to the cylinder, and a faint wiped-out trace of an older line still leads to the circle. Two
  colleagues stand close beside her, one holding an open laptop whose plain silver lid carries nothing at all, no logo, sticker or emblem, both smiling as they
  follow the new line. The drawing shows only shapes and lines, with no letters, words or numbers
  anywhere. The three people are in the centre of the frame. Modern,
  light, bright and friendly: high-key natural daylight, white and light-oak contemporary interior,
  soft pastel accents in light blue, mint and lavender, green plants, 2020s design. Screens show only
  soft blurred shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary editorial
  photography. Avoid: readable text, words or letters on the whiteboard, logos, watermarks, extra
  people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 19
source: demos/07-dapr/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into durable execution". Side-accent
  layout: title, subtitle and five bullets on the left half on plain white, the provided photograph of
  a team at a whiteboard redrawing the line from a component box to a different shape filling the right half, no text over it. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- The code names a component, and the component file picks the Azure service
- Call services by app ID and leave the retries to the resiliency spec
- Write once-only transitions with an ETag and first-write-wins
- Give each van a `VanActor`, hosted on the AppHost or AKS
- Stateless Tier 1 services go to Container Apps, actors and pinned versions to AKS

## Speaker Notes

Close with the decisions this module asks for, each one made in a file or a hosting choice rather than in scattered SDK code.

Components: the code names `pubsub` and `statestore`, and the YAML decides whether Cosmos DB, Service Bus or Redis sits behind them. A broker swap replaces a file and leaves the publisher and subscriber alone.

Calls and policies: services reach each other by app ID over mTLS, and timeouts, retries and breakers live in one resiliency spec instead of retry loops in the caller.

State: a transition that must happen once, such as a booking moving from Booked to PickedUp, uses an ETag with first-write-wins and a bounded reread loop.

Actors: `VanActor` runs one call at a time per van, so capacity holds without locks. The actor packages do not run on Container Apps, so it lives on the AppHost or AKS.

Hosting: Container Apps manages the version and limits the components, AKS hands you the version window. The next module, durable execution, builds on these services to make long-running processes survive restarts.
