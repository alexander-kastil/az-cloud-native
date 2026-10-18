---
slide: "01-02"
topic: 01-durable-execution
title: "Determinism rules"
subtitle: "Context APIs and activities replace calls that change on replay"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/analyzer-fixes.svg"
media-file: labs/08-durable-execution/_images/analyzer-fixes.svg
visual-prompt: >
  Existing diagram of the Durable Task analyzer flagging DateTime.UtcNow and HttpClient in orchestrator code, with the fixes that end the build at zero DURABLE warnings, no generation. Referenced at its repo path.
order: 4
source: demos/08-durable-execution/01-durable-execution/readme.md
gamma_prompt: >
  Slide titled "Determinism rules", subtitle "Context APIs and activities replace calls that change on
  replay". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Determinism rules

## Content

- Every replay must make the same decisions, so the clock, random values and network calls stay out
- `context.CurrentUtcDateTime`, `context.NewGuid()` and `context.CreateTimer` replace the clock, GUIDs and delays
- **Activities** hold the side effects: `HttpClient`, storage clients and `Random`
- Activities run at least once, so each one must be idempotent
- The Roslyn analyzer reports DURABLE0001 to DURABLE0011 before you merge

## Speaker Notes

Replay runs the orchestrator code again, and this slide turns that mechanism into rules for what the code may call. The diagram comes from the lab, where the analyzer finds two of these calls in the booking projects.

Same decisions on every replay: the test for any call is whether it returns the same value each time. Arithmetic and string formatting pass, while the clock, random values, network calls, static variables and environment variables fail.

Context replacements: context.CurrentUtcDateTime replaces DateTime.UtcNow, context.NewGuid replaces Guid.NewGuid, and context.CreateTimer replaces Task.Delay and Thread.Sleep. ILogger becomes context.CreateReplaySafeLogger.

Activities hold the side effects: HttpClient, storage SDK clients and Random belong in an activity whose result the history keeps. Several activities can run in parallel with Task.WhenAll, and each input or output is limited to 1 MB.

At least once: the runtime guarantees that each called activity runs at least once, so a retry after a crash can call it twice. A repeated call with the same input must produce the same outcome.

The analyzer: the Durable Task Roslyn Analyzer reports most of these cases as DURABLE0001 through DURABLE0011, including an unbounded while (true) loop without ContinueAsNew. Run it on every orchestrator project before you merge a workflow change.
