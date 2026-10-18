---
slide: "90-01"
topic: 02-aspire
title: "What the lab builds"
subtitle: "Seven steps from design documents to one traced booking"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/02-aspire/_images/finished-apphost.svg"
media-file: labs/02-aspire/_images/finished-apphost.svg
visual-prompt: >
  Existing diagram of the finished Contoso Logistics AppHost, each piece marked with the step that adds it, from the two emulators and both services to ops-console, exception-agent and the one trace across shipping-api and tracking-service, no generation. Referenced at its repo path.
order: 17
source: labs/02-aspire/readme.md
gamma_prompt: >
  Slide titled "What the lab builds", subtitle "Seven steps from design documents to one traced
  booking". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# What the lab builds

## Content

- Start from the lab 1 design documents, with no service running
- One `aspire start` runs both emulators, shipping-api, tracking-service, ops-console and exception-agent
- A booking travels through **Service Bus** from shipping-api to tracking-service
- `verify-booking-trace.py` asserts **one trace** across both services

## Speaker Notes

The lab practises every topic of the module on Contoso Logistics in about 55 minutes, with GitHub Copilot agent mode or Claude Code writing the code.

Starting point: the `contoso-logistics` starter folder holds only the design documents from lab 1, and `AGENTS.md` gives the agent the class stack. Setup needs the .NET 10 SDK, the Aspire CLI, Node.js, Python 3 and a container runtime, and the lab provisions nothing in Azure.

The finished AppHost: the diagram marks each piece with the step that adds it, from the five projects in step 1 and ServiceDefaults in step 2 to the emulators in step 3 and the ops console and exception agent in step 6.

The booking flow: shipping-api saves the shipment and publishes `ShipmentBooked` in step 4, and tracking-service projects it through its own subscription in step 5.

The proof: step 7 adds a Python script that books shipment `CL-2026-000142`, checks the tracking page and asserts that the trace titled `POST /shipments` holds spans from both services. It runs `aspire stop` in a `finally` block, so no AppHost keeps running.
