# Module 2: Building Distributed Apps with Aspire

Module 1 decided where the service boundaries sit, and this module turns those boundaries into a running system. Contoso Logistics has a shipping API, a tracking service, a routing service, a message bus and a database, and one developer must start, connect and observe all of them together.
After this module, you can start the whole Contoso system with one command, follow a booking across services in the dashboard, and test the event flow end to end.

## Topics

| Topic | Description |
| --- | --- |
| [Aspire AppHost: Model the System](./01-apphost/readme.md) | `DistributedApplication.CreateBuilder`, `AddProject`, `WithReference`, `WaitFor`, `AddParameter` and `aspire run` |
| [ServiceDefaults: Telemetry, Resilience, Discovery](./02-service-defaults/readme.md) | `AddServiceDefaults`, `MapDefaultEndpoints`, `AddStandardResilienceHandler`, `AddServiceDiscovery` and the `https+http` scheme |
| [Aspire Integrations: Hosting & Client](./03-integrations/readme.md) | `AddAzureServiceBus`, `AddAzureCosmosDB`, `AddAzureStorage`, `RunAsEmulator`, `AddAzureServiceBusClient` and `AddAzureCosmosClient` |
| [Aspire Dashboard: Traces Across Services](./04-dashboard/readme.md) | Resources, console and structured logs, distributed traces, metrics, `aspire agent mcp` and the standalone dashboard |
| [Polyglot AppHost: Angular & Python](./05-polyglot/readme.md) | `AddViteApp`, `AddJavaScriptApp`, `AddPythonApp`, `AddUvicornApp` and `WithVirtualEnvironment` |
| [Aspire Testing: Distributed Integration Tests](./06-testing/readme.md) | `Aspire.Hosting.Testing`, `DistributedApplicationTestingBuilder.CreateAsync`, `CreateHttpClient` and `WaitForResourceHealthyAsync` |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/doctor` | Checks the installation, MCP servers and hooks, which catches a broken Aspire CLI or dashboard MCP entry before `aspire run` starts the Contoso AppHost. |
| `/review` | Reviews the AppHost wiring on the current branch: references, waits and parameters across the Contoso resources. |
| `/security-review` | Audits secret parameters and external endpoints, such as a `secret: true` parameter or an external endpoint on the ops console. |
| `/cost` | Shows token use for a session that wires several resources and their services at once, such as the AppHost topic in this module. |

## Key Topics covered in this module

- [Aspire documentation](https://aspire.dev/)
- [Aspire service defaults](https://aspire.dev/get-started/csharp-service-defaults/)
- [Aspire integrations overview](https://aspire.dev/integrations/overview/)
- [Aspire dashboard overview](https://aspire.dev/dashboard/overview/)
- [Aspire testing overview](https://aspire.dev/testing/overview/)
