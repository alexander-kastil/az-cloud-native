---
slide: "90-02"
topic: 02-aspire
title: "Solution projects"
subtitle: "Step 1 shapes GreyLogistics.sln from the design"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/02-aspire/_images/solution-projects.svg"
media-file: labs/02-aspire/_images/solution-projects.svg
visual-prompt: >
  Existing diagram of the five projects in GreyLogistics.sln after step 1, the AppHost referencing ShippingApi and TrackingService, both services referencing Contracts, and ServiceDefaults referenced in step 2, no generation. Referenced at its repo path.
order: 18
source: labs/02-aspire/readme.md
gamma_prompt: >
  Slide titled "Solution projects", subtitle "Step 1 shapes GreyLogistics.sln from the design".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Solution projects

## Content

- The agent reads `docs/architecture.md` and `docs/events.md` and proposes the projects first
- Five projects: AppHost, ServiceDefaults, Contracts, ShippingApi and TrackingService
- AppHost references become `Projects.ShippingApi` and `Projects.TrackingService`
- **Contracts** holds one record per event, such as `ShipmentBooked`

## Speaker Notes

Every later step adds code to a project that already exists, so step 1 creates the solution that holds them.

Research first: the research prompt asks the agent to propose the projects and resources from the design documents without writing code. Push back if it adds billing, routing or label services, because this lab does not build them.

Five projects: the AppHost uses the `Aspire.AppHost.Sdk` project SDK, ShippingApi and TrackingService are ASP.NET Core web projects, and ServiceDefaults comes from the Aspire template. Both services reference ServiceDefaults in step 2.

Typed references: each AppHost project reference adds a typed entry to the generated `Projects` class, the model the AppHost topic explains.

Contracts: `src/Contracts` holds the event records that every service shares, in the namespace `Grey.Contracts`. The step is done when `dotnet sln list` shows the five projects and the build ends with `0 Error(s)`.
