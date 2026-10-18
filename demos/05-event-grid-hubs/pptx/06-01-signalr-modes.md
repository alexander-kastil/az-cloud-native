---
slide: "06-01"
topic: 06-signalr
title: "SignalR Service modes"
subtitle: "Who holds the browser connections"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/05-event-grid-hubs/06-signalr/_images/signalr-modes.svg"
media-file: demos/05-event-grid-hubs/06-signalr/_images/signalr-modes.svg
visual-prompt: >
  Existing diagram of Azure SignalR Service in Default mode, proxying the ops console to VanPositionHub in telemetry-ingest over a server connection set up with AddAzureSignalR, and in Serverless mode, where an Azure Functions app negotiates once and sends each message with SignalROutput, no generation. Referenced at its repo path.
order: 13
source: demos/05-event-grid-hubs/06-signalr/readme.md
gamma_prompt: >
  Slide titled "SignalR Service modes", subtitle "Who holds the browser connections". Layout: Two
  columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# SignalR Service modes

## Content

- **Default**: your server hosts the hub, and the service proxies each client to it
- `AddAzureSignalR` on the `AddSignalR` registration, only when the connection string is set
- **Serverless**: a `negotiate` function and `SignalROutput`, with no hub server
- `withAutomaticReconnect` retries after 0, 2, 10 and 30 seconds
- **Web PubSub** fits when you need a WebSocket protocol of your own

## Speaker Notes

The browser can't hold a stable connection to every backend service, so van positions pass through a managed push service on their way to the map. The service mode decides who owns the client connections, and Learn advises against Classic mode except as a last resort.

Default mode: the telemetry-ingest ASP.NET Core service hosts `VanPositionHub`, and the service proxies the clients to it. `VanPositionProcessor` sends each position with `Clients.All` through `IHubContext<VanPositionHub>`.

AddAzureSignalR: add the `Microsoft.Azure.SignalR` package and call it after `AddSignalR`, with the connection string in configuration rather than source code. Learn recommends Azure Key Vault with Entra ID authorization for production.

Serverless mode: no hub server exists, a `negotiate` function returns the endpoint and an access token, and `SignalROutput` sends messages from other functions. Enable CORS when the ops console calls the function app from a browser, and protect the negotiate endpoint in production.

The client: the ops console uses `@microsoft/signalr` and registers its handlers before it starts the connection. `withAutomaticReconnect` stops after four failed attempts, and the reconnecting state can show a banner that the map may be stale.

Web PubSub: choose it for your own WebSocket protocol, publish and subscribe over WebSockets, or a lightweight server that sends without a backend. It doesn't replace SignalR, so the ops console keeps SignalR for the browser.
