---
slide: "01-02"
topic: 01-extensions-ai
title: "Middleware on every model call"
subtitle: "Logging and OpenTelemetry around the Foundry deployment"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/09-ai-agents/01-extensions-ai/_images/middleware-rings.svg"
media-file: demos/09-ai-agents/01-extensions-ai/_images/middleware-rings.svg
visual-prompt: >
  Existing diagram of the ChatClientBuilder rings UseLogging, UseOpenTelemetry and the optional UseDistributedCache around the AsIChatClient adapter and the Foundry deployment, no generation. Referenced at its repo path.
order: 4
source: demos/09-ai-agents/01-extensions-ai/readme.md
gamma_prompt: >
  Slide titled "Middleware on every model call", subtitle "Logging and OpenTelemetry around the Foundry
  deployment". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Middleware on every model call

## Content

- `ChatClientBuilder` applies the Use methods in reverse order, so the first Use is outermost
- **UseLogging** writes each request and response through ILogger
- **UseOpenTelemetry** emits generative AI spans beside the HTTP and messaging spans
- The conventions are experimental: pin the package and check span names after each upgrade
- **UseDistributedCache** answers a repeated prompt without a model call; custom middleware derives from `DelegatingChatClient`

## Speaker Notes

With the client connected, this slide shows what wraps every call between the exception-agent and the Foundry deployment. The request goes in through each ring, and the response comes back out the same way.

Order and logging: ChatClientBuilder applies the Use methods in reverse order, so UseLogging sees each request first, then UseOpenTelemetry, and the AsIChatClient adapter sits innermost. UseLogging writes an entry for each request and response through ILogger.

OpenTelemetry: UseOpenTelemetry emits spans and metrics under the OpenTelemetry semantic conventions for generative AI, so the model spans sit beside the HTTP and messaging spans of the same operation. The conventions are experimental, so pin the package version in production, check the span names after each upgrade, and pass a source name you add to the tracer configuration.

Cache and custom middleware: UseDistributedCache stores complete responses keyed by the message history, so a repeated prompt turns back before the model, as the cache hit in the diagram shows. Custom middleware derives from DelegatingChatClient and keeps rate limiting and redaction out of the business code.
