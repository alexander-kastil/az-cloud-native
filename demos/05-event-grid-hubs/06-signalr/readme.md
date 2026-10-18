---
minutes: 10
---

# SignalR Service: Push to Browsers

The Contoso Logistics ops console shows the network live. A dispatcher watches vans move across the map and sees parcel status change without a page reload. The browser can't hold a stable connection to every backend service, so the telemetry and delivery events have to pass through a managed push service on their way to the screen.

This topic covers the service modes of Azure SignalR Service, how the ops console receives van positions and parcel status, the negotiation function and the Functions output binding, an ASP.NET Core hub with `AddAzureSignalR`, the `@microsoft/signalr` client, and Web PubSub as the alternative for raw WebSockets.

## Service Modes: Default, Serverless and Classic

Azure SignalR Service has three service modes, and the mode decides who owns the client connections. In Default mode, your hub servers accept the connections, and the service acts as a proxy between clients and those servers. Your server code connects to the service with `AddAzureSignalR`.

In Serverless mode, no hub server exists, and the service maintains the connections itself, so your functions only negotiate and send messages.

| Mode | Who holds client connections | Server connection | Fit for Contoso Logistics |
| --- | --- | --- | --- |
| Default | Your hub servers, through the service | Yes, with `AddAzureSignalR` | An ASP.NET Core ops API that already runs hubs |
| Serverless | The service | No | An Azure Functions app that negotiates and broadcasts |
| Classic | Hub servers, with a limited fallback | Yes | Legacy applications only |

Learn recommends Default or Serverless for new applications and advises against Classic mode except as a last resort. Set the mode when you create the resource, or change it later in the resource settings. Serverless mode also supports upstream endpoints for messages that clients send to the server, and that feature is in public preview.

## Pushing Van Positions and Parcel Status to the Ops Console

The flow has three hops. The routing-service consumes van positions from Event Hubs, as covered in the previous topic, and it publishes each position to SignalR. Parcel status changes arrive as CloudEvents, and a subscription hands them to the same push path.

The ops console, an Angular application, holds one SignalR connection and handles both message types.

SignalR groups let the server send a message to a subset of clients. The ops console can join a group for the map and a group for a dispatcher's parcel list, so the server sends a van position only to the map group. Each message carries a target name, which the browser uses to route the message to a handler.

## Serverless Functions: Negotiate and Output Binding

A client needs a valid access token and the service endpoint before it can connect. A `negotiate` function returns that information, and the SignalR client calls it automatically when you pass the function base URL. The function must have an HTTP route that ends in `negotiate`, and the connection string must be set in the app settings, under the default name `AzureSignalRConnectionString` unless you override the name.

```csharp
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using Microsoft.Azure.Functions.Worker.Http;

public static class Negotiate
{
    [Function("negotiate")]
    public static string Run(
        [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequestData req,
        [SignalRConnectionInfoInput(HubName = "ops")] string connectionInfo)
    {
        return connectionInfo;
    }
}
```

The function package is `Microsoft.Azure.Functions.Worker.Extensions.SignalRService`. The `SignalRConnectionInfoInput` attribute produces the connection information, and the `SignalROutput` attribute sends messages or manages group membership from other functions.

The isolated worker reference shows `SignalRGroupAction` for adding and removing members. The SignalR resource must run in Serverless mode when Azure Functions uses it.

Because the browser calls the function app from another origin, enable CORS on the function app when the ops console runs in a browser. Protect the negotiate endpoint with an authentication mechanism in production, because the example function returns connection information to anyone who calls it.

## ASP.NET Core Hub with AddAzureSignalR

In Default mode, the ASP.NET Core application hosts the hub and sends messages from server code. Add the `Microsoft.Azure.SignalR` package and call `AddAzureSignalR` after `AddSignalR`. The service connection string goes in configuration, not in source code.

```csharp
builder.Services.AddSignalR().AddAzureSignalR();
```

```json
{
  "Azure": {
    "SignalR": {
      "Enabled": true,
      "ConnectionString": "<your-connection-string>"
    }
  }
}
```

Learn warns that raw connection strings are for demonstration only, and it recommends Azure Key Vault together with Microsoft Entra ID authorization for production. Azure SignalR Service doesn't support Serverless mode for the ASP.NET (non-Core) framework, so the ASP.NET Core route is the one that fits Default mode.

## The @microsoft/signalr Client

The ops console uses the `@microsoft/signalr` package from npm. The `HubConnectionBuilder` takes the negotiation base URL, which is the function app URL without the `negotiate` segment. `withAutomaticReconnect` retries a lost connection with the default delays of 0, 2, 10 and 30 seconds, and it stops after four failed attempts.

```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://contoso-ops.azurewebsites.net/api")
  .withAutomaticReconnect()
  .build();

connection.on("vanPositionReported", (position) => {
  console.log(position.vanId, position.latitude, position.longitude);
});

await connection.start();
```

Register handlers before you start the connection, so that no message arrives before a handler exists. During a reconnect, the client raises the reconnecting state, which the ops console can show as a banner so that dispatchers know the map may be stale.

## Web PubSub as the Alternative for Raw WebSockets

Azure Web PubSub is a managed service for real-time messaging over WebSockets, and it is generally available. Choose SignalR when your team uses the SignalR library, when you need its protocol and transports, or when a SignalR client exists for your platform. Choose Web PubSub when you need a WebSocket protocol of your own, a publish and subscribe model over WebSockets, or a lightweight server that sends messages without a backend.

Web PubSub provides the base service for custom real-time applications, Socket.IO on Azure for Socket.IO applications, MQTT support for MQTT clients, and Web PubSub chat for chat rooms and history. The base service includes the `json.webpubsub.azure.v1` subprotocol, which lets a client publish directly to a group. Web PubSub doesn't replace SignalR, so the ops console keeps SignalR for the browser and uses Web PubSub only where a protocol of its own is needed.

## Key Topics

- [What is Azure SignalR Service?](https://learn.microsoft.com/azure/azure-signalr/signalr-overview)
- [Service mode in Azure SignalR Service](https://learn.microsoft.com/azure/azure-signalr/concept-service-mode)
- [Azure Functions development and configuration with Azure SignalR Service](https://learn.microsoft.com/azure/azure-signalr/signalr-concept-serverless-development-config)
- [SignalR Service bindings for Azure Functions (C#)](https://learn.microsoft.com/azure/azure-functions/functions-bindings-signalr-service)
- [Choose an Azure Web PubSub capability](https://learn.microsoft.com/azure/azure-web-pubsub/choose-web-pubsub-capability)
