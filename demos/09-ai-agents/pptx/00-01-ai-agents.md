---
slide: "00-01"
topic: 09-ai-agents
title: "AI agents in event-driven systems"
subtitle: "The exception-agent joins the Contoso Logistics event flow"
layout: title-only
visual-weight: bg
visual-type: photo
media-source: nano-banana
media-file: demos/09-ai-agents/pptx/images/00-01-ai-agents.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. In the software team area of a modern parcel logistics company, a woman software engineer in
  her early thirties stands at a white standing desk and turns her large monitor toward a male colleague
  of about thirty, who leans in with a coffee mug; the monitor shows a softly blurred board of coloured
  lanes and cards, like an event flow that is running. Behind them a light-oak shelf holds a few plain
  brown parcel boxes with blank sides. Both people grouped in the centre of the frame, faces fully
  visible, calm open space around them. Modern, light, bright and friendly: high-key natural daylight,
  white and light-oak contemporary interior, soft pastel accents in light blue, mint and lavender, green
  plants, 2020s design. Screens show only soft blurred shapes and colours, never readable UI. 16:9
  landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks, extra
  people, labels, barcodes or printing on the parcel boxes, dark or moody lighting, vintage or retro
  decor, grime, clutter.
order: 1
source: demos/09-ai-agents/readme.md
gamma_prompt: >
  Title slide "AI agents in event-driven systems" with subtitle "The exception-agent joins the Contoso
  Logistics event flow". Side-accent layout: title and subtitle on the left half on plain white, the
  provided photograph of two engineers at a bright standing desk filling the right half, no text over
  the photo. Use exactly the one image given for this card; do not generate or add any other images,
  icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# AI agents in event-driven systems

## Content

- **Calling the model**: IChatClient on a Foundry deployment, run from a subscriber and a durable activity
- **Staying in quota**: priority queues and a KEDA replica cap sized from tokens per minute
- **Search and coordination**: change feed embeddings, vector search, and events or A2A between agents

## Speaker Notes

This module closes the class storyline: the exception-agent joins the event flow and reacts when a parcel misses its window. It drafts the customer message and proposes a reroute that a person approves.

Calling the model: the agent reaches a Microsoft Foundry deployment through Microsoft.Extensions.AI, subscribes to DeliveryDelayed, and runs each model call as a durable activity.

Staying in quota: priority queues keep urgent exceptions ahead of bulk work, and KEDA caps the replicas so the agent stays inside its token quota.

Search and coordination: a change feed and a Cosmos DB vector index make delivery notes searchable by meaning. The last topic decides when agents coordinate through events and when they call each other with A2A.
