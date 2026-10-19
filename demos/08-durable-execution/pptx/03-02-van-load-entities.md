---
slide: "03-02"
topic: 03-durable-functions
title: "Durable entities for van load"
subtitle: "One entity per van handles its operations one at a time"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/03-02-van-load-entity.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Left
  column, three caller boxes: grey "LoadVan" with "client.Entities.SignalEntityAsync" and "operation
  Load, one-way"; blue "VanLoadOrchestrator" with "context.Entities.SignalEntityAsync" and "operation
  Load, one-way"; blue "VanLoadOrchestrator" with "context.Entities.CallEntityAsync" and "operation Get,
  waits for the result". Arrows labelled "signal", "signal", "call" into a tall amber column holding
  the chips "Load", "Load", "Get" and the caption "operations queue up". One arrow "one at a time" into
  a green entity box "VanLoad" with "[EntityTrigger] TaskEntityDispatcher", an "entity ID" row
  "VanLoad, van-17", a "state" row "Parcels, the parcels the van carries", chips "Load(count)",
  "Unload(count)", "Get()" and the note "keeps its state under 1 MB". Below it an arrow "snapshot" to a
  grey box "client.Entities.GetEntityAsync, a snapshot that can be stale".
order: 8
source: demos/08-durable-execution/03-durable-functions/readme.md
gamma_prompt: >
  Slide titled "Durable entities for van load", subtitle "One entity per van handles its operations one
  at a time". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Durable entities for van load

## Content

- An **entity** holds a small piece of state and accepts operations from many callers
- Grey models each van as one `VanLoad` entity keyed by the van ID, such as `van-17`
- A signal is one-way; a call from an orchestrator waits for the result
- Concurrent signals queue up, so no two operations update the state at once
- A read returns a snapshot that can be stale; keep each entity under 1 MB

## Speaker Notes

Grey tracks how many parcels each van carries, and many scanners change that count at the same time. A durable entity gives that count a home that the scheduler persists.

Entities: an entity holds a small piece of state and accepts operations from many callers. VanLoad uses the POCO form, where the framework serializes the whole object, and TaskEntity<TState> is the other form, with a State property and constructor injection.

One entity per van: the entity ID combines the name VanLoad with the van ID, and the state counts the parcels with the operations Load, Unload and Get. The entity function dispatches them with TaskEntityDispatcher.

Signals and calls: clients and orchestrators send operations through the Entities property. A signal is one-way, so the caller never sees the result, while CallEntityAsync in an orchestrator waits for the operation's result.

One at a time: an entity processes its operations one after another, so concurrent signals to the same van queue up on that entity. No two operations update its state at once.

Reads and limits: GetEntityAsync returns a snapshot of the state, which can be stale by the time your code uses it. The isolated model serializes state with System.Text.Json, which compares property names case-sensitively, so keep the state type simple and under the 1 MB limit.
