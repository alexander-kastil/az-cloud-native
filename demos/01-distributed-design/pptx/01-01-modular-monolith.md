---
slide: "01-01"
topic: 01-monolith-vs-microservices
title: "A modular monolith as the starting point"
subtitle: "Service boundaries inside one deployable unit"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/01-monolith-vs-microservices/_images/modular-monolith.svg"
media-file: demos/01-distributed-design/01-monolith-vs-microservices/_images/modular-monolith.svg
visual-prompt: >
  Existing diagram of a modular monolith with the Booking, Routing, Billing and Labels modules over their own schemas, then Routing moved out as routing-service behind the same interface, no generation. Referenced at its repo path.
order: 3
source: demos/01-distributed-design/01-monolith-vs-microservices/readme.md
gamma_prompt: >
  Slide titled "A modular monolith as the starting point", subtitle "Service boundaries inside one
  deployable unit". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# A modular monolith as the starting point

## Content

- Split for **team size**, independent deployment, scaling or data ownership
- A **modular monolith** deploys as one unit, organized into modules
- Each module owns its tables and exposes a small public interface
- Moving a module out turns its interface into the **service contract**

## Speaker Notes

Contoso could make booking, routing, billing and labels separate services from day one, because each looks like it needs its own team. This slide gives the criteria for that call and the first step the class recommends.

Team size, deployment, scaling and data: a service should be small enough for one feature team and ship without its neighbours. Routing grows with the number of vans while the tracking page grows with viewers, and two services that share a data store still behave as one.

One deployable unit: a modular monolith ships as one unit, but its code follows the boundaries a service would have. For Contoso, the first version hosts booking, routing, billing and labels in one process, after a check of team and infrastructure maturity.

Own tables, small interface: other modules call the interface and never read another module's tables. Each module keeps its own schema, so the seams show up in the code and in the database.

Moving a module out: when one module needs its own scaling, release train or team, it becomes a service. The interface already written becomes its contract, so the change stays small, as Routing shows on the right.
