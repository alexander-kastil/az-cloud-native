---
minutes: 10
---

# Polyglot AppHost: Angular & Python

Contoso's operations console is an Angular app, and the exception agent is a Python service, so the AppHost can start them only when it knows how to host non-.NET apps.
This topic hosts both next to the .NET services, so one `aspire run` starts the whole system and the dashboard lists their endpoints beside the rest.

## Hosting JavaScript Apps with AddViteApp and AddJavaScriptApp

The JavaScript hosting package is `Aspire.Hosting.JavaScript`. Its calls create a resource for a Node-based app, and the resource runs the app's package script when the AppHost starts.

| Call | Use it for |
| --- | --- |
| `AddJavaScriptApp(name, appDirectory)` | A Node app that runs its `dev` script by default |
| `AddViteApp(name, appDirectory)` | A Vite app, including Angular 17 and later, which uses Vite internally |
| `AddNodeApp(name, appDirectory, scriptPath)` | A Node server started from a script file |

`AddViteApp` registers an `http` endpoint and sets `PORT`, so do not call `WithHttpEndpoint` on a Vite resource. Older samples use `AddNpmApp`, but the current JavaScript page documents the three calls above instead.

Use `AddJavaScriptApp` for a Node app whose `dev` script starts it, and use `AddViteApp` for a Vite app that needs its port from Aspire. Both calls take the app directory as their second argument, and the examples in the docs give that path relative to the AppHost project.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var shippingApi = builder.AddProject<Projects.ShippingApi>("shipping-api");

var opsConsole = builder.AddViteApp("ops-console", "../ops-console")
    .WithReference(shippingApi)
    .WaitFor(shippingApi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
```

The `WaitFor` call holds the dev server until the shipping API runs, so the console does not start against a missing backend. `WithExternalHttpEndpoints` gives the console an address that a browser on the developer machine can open.

## The Angular Operations Console

The ops console keeps its own `package.json`, and its `dev` script serves the Angular app under the AppHost. The Angular proxy reads the API address from `API_HTTPS` or `API_HTTP`, and the reference to the shipping API is the call that connects the two resources in the model.

The aspire.dev page shows the proxy reading those variables but does not show which call sets them. Open the resource view in the dashboard and confirm the variables before you rely on them.

The browser talks only to the Angular dev server, and the proxy forwards API calls to the shipping API. The console code calls relative paths such as `/api/weather`, so the browser needs no CORS setup during development.

## Python Apps with AddPythonApp and AddUvicornApp

The Python hosting package is `Aspire.Hosting.Python`. `AddPythonApp` runs a script, and `AddUvicornApp` runs an ASGI application in `module:variable` form, which suits a FastAPI agent.

| Call | Arguments | Runs |
| --- | --- | --- |
| `AddPythonApp(name, appDirectory, scriptPath)` | Directory and script file | A Python script |
| `AddUvicornApp(name, appDirectory, app)` | Directory and `module:variable` | An ASGI app |
| `AddPythonModule(name, appDirectory, moduleName)` | Directory and module name | A module, like `python -m` |
| `AddPythonExecutable(name, appDirectory, executableName)` | Directory and tool name | A CLI from the virtual environment |

Aspire uses the `.venv` folder in the app directory and detects it from `requirements.txt` or `pyproject.toml`. Call `WithVirtualEnvironment` to point at another location. No API turns virtual environment management off.

Use `AddPythonApp` for a worker script and `AddUvicornApp` for a service that answers HTTP requests.

A Python developer starts the exception agent the same way as the .NET services, with one `aspire run` and no second terminal. The dashboard lists the Python resource beside them.

The Python project keeps its dependencies in `requirements.txt` or `pyproject.toml`, and the `.venv` folder beside them holds the packages that Aspire starts. Those packages stay in that virtual environment, so the agent's dependencies never mix with other Python tools on the developer machine.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddAzureServiceBus("messaging")
    .RunAsEmulator();
var shipmentEvents = messaging.AddServiceBusTopic("shipment-events");
shipmentEvents.AddServiceBusSubscription("exception-agent");

builder.AddUvicornApp("exception-agent", "../exception-agent", "main:app")
    .WithReference(messaging)
    .WaitFor(messaging);

builder.Build().Run();
```

The exception agent takes its own subscription on the `shipment-events` topic. It receives `DeliveryDelayed` events without any change to the publisher, and the subscription name keeps its traffic separate from the tracking service.

## Telemetry from Non-.NET Services

A Node or Python process joins the system only when the team needs its runtime, and the rest of the Contoso system stays .NET. The exception agent runs under the same AppHost lifecycle as the .NET services, so it starts and stops with them.

The standalone dashboard accepts OpenTelemetry from Python and Node.js apps, as the [dashboard topic](../04-dashboard/readme.md) shows. A Python or Node service sends its logs and traces to that OTLP endpoint, and the dashboard shows them without any AppHost. The service sets its endpoint in its own environment, so it needs no change to the AppHost to appear there.

## Key Topics

- [Aspire JavaScript hosting](https://aspire.dev/integrations/frameworks/javascript/)
- [Aspire Python hosting](https://aspire.dev/integrations/frameworks/python/)
- [Standalone Aspire dashboard](https://aspire.dev/dashboard/standalone/)
- [Aspire integrations overview](https://aspire.dev/integrations/overview/)
