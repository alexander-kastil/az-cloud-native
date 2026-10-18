---
slide: "01-01"
topic: 01-apphost
title: "Declaring and wiring resources"
subtitle: "WithReference passes the connections, WaitFor orders the startup"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/02-aspire/01-apphost/_images/apphost-startup.svg"
media-file: demos/02-aspire/01-apphost/_images/apphost-startup.svg
visual-prompt: >
  Existing diagram of the AppHost frame where shipping-api and tracking-service wait for the cosmos and messaging emulators and WithReference hands each service its connection strings, no generation. Referenced at its repo path.
order: 3
source: demos/02-aspire/01-apphost/readme.md
gamma_prompt: >
  Slide titled "Declaring and wiring resources", subtitle "WithReference passes the connections, WaitFor
  orders the startup". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Declaring and wiring resources

## Content

- Each builder call adds a **resource**; the generated `Projects` type catches a wrong reference at compile time
- `WithReference` hands a service the connection details as environment variables
- `WaitFor` holds a service until its dependency is Running; client retries still apply
- `aspire run` starts every resource and the dashboard; `--isolated` runs a second copy

## Speaker Notes

Contoso runs several services, a message bus and a database, and each one needs the others' addresses before it starts. The AppHost describes that system in C#, so one command starts every resource in the right order.

Resources: every call on the builder adds a resource to the model, and `Build().Run()` starts them. `Projects.ShippingApi` is generated from the AppHost's project references, so a wrong reference fails at compile time and a topology change shows up as a reviewable diff.

WithReference: the arrows in the diagram run from cosmos and messaging up into each service and carry a connection string such as `ConnectionStrings__cosmos`. The service reads it through its client package, so connection strings stay out of source.

WaitFor: it delays a service until the resource it names reaches the Running state, so the tracking service cannot start before the bus is ready. The wait orders the startup and does not replace retries in the client code.

aspire run: it builds the project, starts every resource and the dashboard, and prints the dashboard URL with a login token. `--isolated` randomizes the ports and separates user secrets, so a second copy of the Contoso system can run beside the first.
