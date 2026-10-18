---
slide: "99-01"
topic: 02-aspire
title: "Takeaways"
subtitle: "What to carry into the hosting module"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/02-aspire/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. A team of four software engineers, two women and two men in their late twenties and early
  thirties in modern work-casual clothes, at a tall light-oak counter in a bright office kitchen at the
  end of a workshop day, laptops closed and stacked, coffee cups in hand, laughing together while one of
  them tells a story. Group on the right half, bright window light on the left half as space for text.
  Modern, light, bright and friendly: high-key natural daylight, white and light-oak contemporary
  interior, soft pastel accents in light blue, mint and lavender, green plants, 2020s design. Screens
  show only soft blurred shapes and colours, never readable UI. 16:9 landscape 1920x1080, contemporary
  editorial photography. Avoid: readable text, logos, watermarks, dark or moody lighting, vintage or
  retro decor, grime, clutter, extra people.
order: 20
source: demos/02-aspire/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into the hosting module". Side-accent layout:
  title, subtitle and five bullets on the left half on plain white, the provided photograph of a team
  laughing at a kitchen counter after a workshop day filling the right half, no text over it. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Describe the system once in the **AppHost**; one command starts it in order
- Opt every service into **ServiceDefaults** with one call
- Keep `connectionName` equal to the resource name; emulators run the same code
- Follow one booking as one **trace** across HTTP and Service Bus
- Write **gates** before the agent writes code, and wait for health in every test

## Speaker Notes

Close with the habits this module built, each one a decision about how the Contoso system is described, run and checked.

AppHost and defaults: the AppHost is reviewable C# that starts every resource in order, and ServiceDefaults gives each service the same telemetry, resilience, discovery and health endpoints with one call.

Connections: the connection name is the contract between the AppHost and the client package, so the same code runs against the emulator on the laptop and against Azure.

Tracing: the trace context rides `traceparent` over HTTP and `Diagnostic-Id` over Service Bus. The dashboard, or an agent through MCP, shows where a booking spent its time.

Testing: tests start the real AppHost, wait for health and poll with a deadline, and gates written first give the agent a target it cannot move. Module 3 puts the same services on Azure Container Apps and AKS.
