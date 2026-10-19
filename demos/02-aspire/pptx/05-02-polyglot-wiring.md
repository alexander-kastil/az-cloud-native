---
slide: "05-02"
topic: 05-polyglot
title: "Ops console and exception agent in the AppHost"
subtitle: "One aspire run starts Angular and Python beside the .NET services"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/02-aspire/_images/polyglot-wiring.svg"
media-file: labs/02-aspire/_images/polyglot-wiring.svg
visual-prompt: >
  Existing diagram of the AppHost starting ops-console with AddViteApp and the shipping-api endpoints as API_HTTPS and API_HTTP for proxy.conf.js, and exception-agent with AddUvicornApp and a reference to messaging, no generation. Referenced at its repo path.
order: 13
source: demos/02-aspire/05-polyglot/readme.md
gamma_prompt: >
  Slide titled "Ops console and exception agent in the AppHost", subtitle "One aspire run starts Angular
  and Python beside the .NET services". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect
  ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Ops console and exception agent in the AppHost

## Content

- `AddViteApp` starts `ops-console`; two `WithEnvironment` calls pass `API_HTTPS` and `API_HTTP`
- The Angular **proxy** forwards `/api` to shipping-api, so the browser needs no CORS setup
- `AddUvicornApp` runs the exception agent's `main:app` with a reference to `messaging`
- The dashboard lists each app beside an **installer** resource that reads Finished

## Speaker Notes

This slide puts the two hosting calls to work for Grey, in the same wiring that the lab builds.

Ops console: `AddViteApp` starts the console from `web/ops-console`, and `WithEnvironment` sets `API_HTTPS` and `API_HTTP` to the shipping API endpoints. `WaitFor` holds the dev server until shipping-api runs, and `WithExternalHttpEndpoints` gives it an address a browser can open.

Proxy: the browser talks only to the Angular dev server, whose proxy reads the API address from those variables and forwards the relative `/api` paths. Confirm the variables in the dashboard's resource view before you rely on them.

Exception agent: `AddUvicornApp` runs the FastAPI app with `WithReference` and `WaitFor` on messaging. The agent takes its own subscription on `shipment-events`, so it receives `DeliveryDelayed` events without any change to the publisher.

Installer resources: one `aspire run` starts everything with no second terminal. The dashboard shows `ops-console-installer` and `exception-agent-installer` beside the apps, each reading Finished once its install step completes.
