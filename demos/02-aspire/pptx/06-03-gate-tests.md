---
slide: "06-03"
topic: 06-testing
title: "Gate tests"
subtitle: "A fixed test proves each step that an agent builds"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/06-03-gate-tests.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Top
  row, three numbered boxes left to right with arrows: 1 purple "Write the gate" with
  [Trait("Gate", "BookingReachesTracking")], "tests/ContosoLogistics.Tests" and "AGENTS.md: the agent must
  not edit the test project"; 2 blue "Agent writes the code" with "an AI coding agent builds the step
  that the gate proves"; 3 grey "Run the gate" with dotnet test tests/ContosoLogistics.Tests --filter
  "Gate=BookingReachesTracking" and "without a filter: every step so far". From box 3 one arrow down to a
  green "Gate passes" box with a checkmark, "the step is done, as a result anyone can rerun", and an
  elbow arrow to an amber "Gate fails" box, "the agent fixes the code of the step that owns the gate",
  which loops back up into box 2. Left of the outcomes the caption "each gate name reads as one promise"
  over two purple pills AppHostListsResources and BookingReachesTracking. Bottom row under the caption "a
  gate takes the smallest form that proves the outcome": "REST call, curl: status code and body", "Unit
  test, xUnit, starts no container", "Integration test, starts the whole AppHost".
order: 16
source: demos/02-aspire/06-testing/readme.md
gamma_prompt: >
  Slide titled "Gate tests", subtitle "A fixed test proves each step that an agent builds". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Gate tests

## Content

- A gate proves one step is done, checked from outside the code the step changes
- Write the gate first; `AGENTS.md` tells the agent to leave the test project unchanged
- `[Trait("Gate", ...)]` lets `dotnet test --filter` run exactly one step's gate
- A gate takes the smallest form: a `curl` call, a unit test or a full AppHost test

## Speaker Notes

When an AI coding agent writes the code, a gate turns done from a claim into a result anyone can rerun.

What a gate is: it names the outcome a step promises, such as a booking reaching the tracking service, and fails until the code delivers it. Gates also catch a later step that breaks an earlier one, and each gate name reads as one promise the design makes.

A fixed target: write the gates before the agent writes the code and state in `AGENTS.md` that it must not edit the test project, because an agent that may change a failing test can always make it pass. When a gate fails, the agent fixes the code of the step that owns it.

The Gate trait: one command runs exactly the gate of one step, and a run without a filter checks every step built so far.

Smallest form: a gate can be a `curl` request whose status code and body show a booking was accepted, an xUnit test that starts no container, or an integration test that starts the whole AppHost. The labs write one gate, `BookingReachesTracking` in lab 2, and note what the others would check.
