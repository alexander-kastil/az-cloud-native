---
slide: "99-01"
topic: 09-ai-agents
title: "Takeaways"
subtitle: "The agent is a service like any other"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/09-ai-agents/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. In the bright operations area of a last-mile parcel delivery company, a software architect in
  her mid thirties and two Azure developers, a man and a woman in their late twenties to early thirties,
  stand in front of a large wall display that shows one calm column of equal soft green rows; the
  architect, turned half toward the camera, points back at one row in the middle of the column that
  looks exactly like the rows above and below it, and the two developers nod, relaxed, coffee cups in
  hand. The wall display shows only colour fields and faint grey lines, with no letters, words or
  numbers anywhere, in a plain bezel with no logo on it. A plain brown parcel box with no letters, words
  or numbers anywhere stands on a light-oak side table. The three people grouped in the centre of the
  frame, faces fully visible. Modern, light, bright and friendly: high-key natural daylight, white and
  light-oak contemporary interior, soft pastel accents in light blue, mint and lavender, green plants,
  2020s design. Screens show only soft blurred shapes and colours, never readable UI. 16:9 landscape
  1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks, extra people,
  letters or numbers on the screen, cups or parcel box, dark or moody lighting, vintage or retro decor,
  grime, clutter.
order: 18
source: demos/09-ai-agents/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "The agent is a service like any other". Side-accent
  layout: title, subtitle and five bullets on the left half on plain white, the provided photograph of
  three engineers at an operations wall display filling the right half, no text over it. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- The exception-agent subscribes with the same contracts, retries and idempotency rules as any service
- Call the model through IChatClient with Entra ID, and only from a durable activity
- Check the processed marker before the model call, and reuse the stored draft
- Size replicas from the token quota and queue urgent work ahead of bulk
- Start agent coordination as choreography; use A2A when one answer is needed now

## Speaker Notes

Close the module, and the class storyline, with the habits that let a model run inside an event-driven system. The model stays one slow, non-deterministic, non-idempotent step in a larger flow.

Same rules: the exception-agent is a subscriber like any other, with the same contracts, retries and idempotency rules as the first modules taught.

Model calls and idempotency: IChatClient with DefaultAzureCredential keeps keys out of the service, and the durable activity records the draft so a replay never calls the model again. The processed marker comes before the model call, and a retry reuses the stored draft.

Quota: keep replicas times per-replica concurrency within the calls the token quota allows, and give urgent exceptions their own queue.

Coordination: start with choreography, where consumers join without a change to the publisher, and use A2A only where a caller needs one answer now. Agent fundamentals, MCP servers and durable agents in depth continue in the pro-code-agents class.
