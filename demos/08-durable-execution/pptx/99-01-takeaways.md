---
slide: "99-01"
topic: 08-durable-execution
title: "Takeaways"
subtitle: "What to carry into the AI agents module"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/08-durable-execution/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Three software engineers at the end of a workshop day in the bright café corner of a modern
  parcel delivery company, standing close together at a light-oak counter: a young woman turns her open
  laptop with a plain lid on the counter toward her two colleagues, its screen facing them and the camera
  and showing a softly blurred list of rows, each ending in a small green mark, with one single row
  ending in a small amber mark. She rests a fingertip beside the amber row and smiles, one colleague nods
  and the other leans in to look. The three people are in the centre of the frame. Modern, light, bright and friendly: high-key
  natural daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint
  and lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never
  readable UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text,
  logos, watermarks, extra people, words in square brackets or parentheses, dark or moody lighting,
  vintage or retro decor, grime, clutter.
order: 20
source: demos/08-durable-execution/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into the AI agents module". Side-accent
  layout: title, subtitle and five bullets on the left half on plain white, the provided photograph of
  three engineers at a bright café counter reading a laptop list of saga runs with one amber row filling the right half, no text
  over it. Use exactly the one image given for this card; do not generate or add any other images, icons
  or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Keep orchestrators deterministic and put every side effect in an activity
- Give every activity a stable idempotency key, because it runs at least once
- Bound every wait for a person with a durable timer, and cancel the timer that loses
- Scale SDK workers on the scheduler backlog with the KEDA rule
- Undo completed saga steps in reverse order, and alert on compensated sagas

## Speaker Notes

Close with the habits this module built, each one a rule to apply before a long-running process goes to production.

Deterministic orchestrators: replay runs the code again, so clock reads, GUIDs, delays and network calls move to the context APIs or into activities. The analyzer flags them before a merge.

Idempotent activities: the runtime runs each activity at least once, so a stable key such as the parcel ID plus the step name keeps a retry from charging the customer twice.

Timers with events: a durable timer bounds every wait for a delivery scan or a customer decision, and the worker stays free while the instance waits. Cancel the timer that loses, or the orchestration stays open.

Scale on the backlog: the azure-durabletask-scheduler rule adds worker replicas as pending activities grow and removes them as the load falls.

Compensate in reverse: the saga pops its compensation stack, so the most recent completed step is undone first. Module 9 builds on this when the exception agent subscribes to DeliveryDelayed and proposes a reroute that a person approves.
