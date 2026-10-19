---
slide: "01-02"
topic: 01-apphost
title: "Parameters for secrets and settings"
subtitle: "AddParameter keeps the routing API key out of source"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/02-aspire/pptx/images/01-02-parameters.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, 'Segoe UI', Inter, Arial,
  sans-serif as in the module's diagrams, text #0F172A, secondary #64748B, arrows #334155, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. An
  AppHost frame on the left two thirds holds two white code cards: top "var routingApiKey =
  builder.AddParameter("routing-api-key", secret: true);" with secret: true in pink #BE185D and the note
  "secret marks the value as sensitive"; bottom "builder.AddProject<Projects.RoutingService>(
  "routing-service") .WithEnvironment("ROUTING_API_KEY", routingApiKey);". Between them, under the caption
  "Aspire looks for the value", three numbered steps top to bottom: 1 blue "Environment variables" with
  "Parameters__routing_api_key" and "dashes become underscores", 2 blue "Configuration files and user
  secrets", 3 amber "Asks the developer" with "when no source holds the value, on the first run". An
  arrow from the AddParameter card into step 1, short arrows down between steps, a bracket spanning the
  three steps with an arrow "value" to a blue routing-service box on the right holding "Projects.
  RoutingService", a chip "ROUTING_API_KEY", "environment variable" and "the key stays out of source".
  An elbow arrow "starts" from the bottom card under the steps into the routing-service box.
order: 4
source: demos/02-aspire/01-apphost/readme.md
gamma_prompt: >
  Slide titled "Parameters for secrets and settings", subtitle "AddParameter keeps the routing API key out
  of source". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Parameters for secrets and settings

## Content

- `AddParameter` declares a value the AppHost needs but does not hard-code
- `secret: true` marks it as sensitive, such as the routing service's API key
- Aspire reads **environment variables** first, then configuration files and user secrets
- With no value anywhere, Aspire asks the developer on the first run

## Speaker Notes

After the connections, the AppHost still needs values it must not contain, such as the API key of the routing service.

AddParameter: it declares a value that the AppHost needs but does not hard-code. The same model then runs on every developer machine with that developer's own key.

Secret flag: `secret: true` marks the value as sensitive, and `WithEnvironment` hands it to the routing service as `ROUTING_API_KEY` without the key appearing in source.

Lookup order: Aspire looks in environment variables first, then in configuration files and user secrets. The environment variable form is `Parameters__routing_api_key`, with dashes replaced by underscores.

First run: when no source holds the value, Aspire asks the developer. A new team member therefore sees exactly which settings the system needs.
