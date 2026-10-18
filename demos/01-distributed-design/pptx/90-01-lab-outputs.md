---
slide: "90-01"
topic: 01-distributed-design
title: "Lab: event-storm the parcel delivery flow"
subtitle: "Two design documents that lab 2 turns into C# records"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/01-distributed-design/_images/lab-outputs.svg"
media-file: labs/01-distributed-design/_images/lab-outputs.svg
visual-prompt: >
  Existing diagram of the two files the lab produces, docs/events.md and docs/architecture.md, with each section marked by the step that adds it and both files going to lab 2, no generation. Referenced at its repo path.
order: 18
source: labs/01-distributed-design/readme.md
gamma_prompt: >
  Slide titled "Lab: event-storm the parcel delivery flow", subtitle "Two design documents that lab 2
  turns into C# records". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Lab: event-storm the parcel delivery flow

## Content

- Starts from an empty workspace with agent instructions and an empty `docs/` folder
- `docs/events.md` holds 13 events, 7 commands and the aggregates behind them
- `docs/architecture.md` holds contexts, the service map, flows and saga compensations
- Five steps in about 45 minutes, with no code and no tests
- **Design checks** confirm both documents before lab 2

## Speaker Notes

The lab turns the module into a design for Contoso Logistics, which exists only as a business brief. Participants open `labs/01-distributed-design/contoso-logistics/` with GitHub Copilot agent mode or Claude Code.

Starting point: the workspace holds only agent instruction files, an MCP server configuration and an empty `docs/` folder, and the agent reads `AGENTS.md` for the class stack.

events.md: steps 1 and 2 add the Events table with kind and style, the Emitted by column, and the Commands and Aggregates sections.

architecture.md: step 3 adds the Bounded Contexts and Service Map sections, step 4 the Flows section, and step 5 the Reliability pattern column and the Saga Compensations section.

Format: the lab practices all six topics of the module and produces documents only, so `src/` and `tests/` get no new files.

Design checks: they confirm, for example, that the scan flow keys on the tracking number, the telemetry flow partitions on the van ID and the compensations run in reverse order. The finished design in `contoso-logistics-solution` is there for comparison once every check passes.
