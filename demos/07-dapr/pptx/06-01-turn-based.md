---
slide: "06-01"
topic: 06-actors
title: "Virtual actors and turn-based access"
subtitle: "Each van is an actor that handles one call at a time"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/07-dapr/06-actors/_images/turn-based-actor.svg"
media-file: demos/07-dapr/06-actors/_images/turn-based-actor.svg
visual-prompt: >
  Existing diagram of two AssignParcelAsync calls for van-42 running one turn at a time, where PKG-778 takes the free slot and PKG-779 finds the van full, no generation. Referenced at its repo path.
order: 14
source: demos/07-dapr/06-actors/readme.md
gamma_prompt: >
  Slide titled "Virtual actors and turn-based access", subtitle "Each van is an actor that handles one
  call at a time". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Virtual actors and turn-based access

## Content

- The runtime activates an actor on the first request for its ID
- Idle actors are garbage-collected and come back with their saved state
- Calls run one at a time, so two assignments for `van-42` never share a free slot
- `IVanActor` derives from `IActor`, `VanActor` from `Actor`; callers use `ActorProxy.Create`
- All actor types share one **actor state store** that supports transactions

## Speaker Notes

Each van has one piece of state that changes all day: the parcels on it and its last position. Two routing requests can arrive for the same van in the same second, and both must see the true load.

Activation: an actor is a unit of state and behaviour with an identity, and the runtime activates it the first time a request arrives for its ID. You never create or destroy one explicitly.

Deactivation: when an actor stays idle, the runtime garbage-collects the in-memory object but keeps the actor known. The next request reactivates it with its saved state.

Turn-based access: each method call or timer callback runs to completion before the next one starts, so actor code needs no locks for its own data. In the diagram `PKG-778` takes the last slot on `van-42`, and `PKG-779` waits, sees a full van and gets `false`.

The .NET shape: the interface derives from `IActor`, the class derives from `Actor` and receives an `ActorHost`. `AddActors` and `MapActorsHandlers` host it, and `ActorProxy.Create<IVanActor>` routes each call to the instance that holds the state.

State store: the runtime persists actor state in the state store, so the load survives restarts. The Cosmos DB component marks itself as the actor store with `actorStateStore` set to `"true"`.
