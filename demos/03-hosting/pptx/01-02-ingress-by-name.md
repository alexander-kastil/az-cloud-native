---
slide: "01-02"
topic: 01-aspire-deploy
title: "Ingress and calls by app name"
subtitle: "Public endpoints only where outside clients need them"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/03-hosting/pptx/images/01-02-ingress-by-name.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, ground #F8FAFC as in the module's existing
  diagrams, 'Segoe UI', Inter, Arial, sans-serif, Consolas for code and URLs, text #0F172A and secondary
  #64748B, minimum 26px type. No heading inside the image; the slide title carries it. No arrow crosses
  a box or a label. Left: a white "Internet" box, "clients outside the environment". Two elbow arrows
  labelled "external ingress" lead into a white container with a #1D4ED8 border, "aca-env, Container
  Apps environment", holding two blue #DBEAFE cards: "shipping-api" and "tracking-service", each with
  "WithExternalHttpEndpoints()" and a "/health" pill reading "mapped in every environment". A vertical
  arrow from tracking-service up to shipping-api, labelled "http://shipping-api" and "never leaves the
  environment". Right: a dashed white box "local runs only" with two amber #FEF3C7 cards "ops-console"
  and "exception-agent", then "ExcludeFromManifest()" and "left out of aspire deploy".
order: 4
source: demos/03-hosting/01-aspire-deploy/readme.md
gamma_prompt: >
  Slide titled "Ingress and calls by app name", subtitle "Public endpoints only where outside clients
  need them". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Ingress and calls by app name

## Content

- `WithExternalHttpEndpoints` publishes an endpoint; without it a service stays internal
- `shipping-api` and `tracking-service` are the two public services
- Apps in the environment call each other by name: `http://shipping-api`
- `/health` maps in every environment, so it can be probed from outside
- `ExcludeFromManifest()` keeps `ops-console` and `exception-agent` local

## Speaker Notes

Once the apps are deployed, ingress decides which services receive traffic from outside the environment.

External or internal: `WithExternalHttpEndpoints` publishes an endpoint, and a service without it is reachable only from apps in the same environment. Use the external call only for endpoints that clients outside the environment must reach.

The two public services: the lab's AppHost calls `WithExternalHttpEndpoints` on `shipping-api` and `tracking-service`, the two services that clients reach from outside.

Calls by app name: `tracking-service` reaches `shipping-api` at `http://shipping-api`, and that traffic never leaves the environment. App names are 2 to 32 lowercase letters, numbers and hyphens, starting with a letter and ending with a letter or number.

Health from outside: the shared service defaults map `/health` in every environment, not only Development, so an external endpoint can be probed from outside your machine.

Local only: `ops-console` and `exception-agent` call `ExcludeFromManifest()`, so `aspire deploy` leaves them out and they run only in local runs.
