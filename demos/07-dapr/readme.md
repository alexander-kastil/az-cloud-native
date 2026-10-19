# Module 7: Dapr Building Blocks for Portable Services

Modules 4 to 6 connect Grey Logistics to Service Bus, Event Hubs and Cosmos DB through their Azure SDKs. This module runs the same flow through Dapr building blocks instead. Services call service invocation, state, pub/sub and actors on their sidecar, and the component files decide which Azure service sits behind each call.

The seven topics take one building block at a time: the sidecar, service invocation, state with ETags, pub/sub, resiliency, actors, and the hosting limits on Container Apps and AKS. Each topic keeps the Grey domain, so `shipping-api`, `tracking-service` and `routing-service` stay the same while the calls change.

After this module you can run a service with a Dapr sidecar in the Aspire AppHost, write state with ETag checks, publish and subscribe through a broker you can swap, define retry and circuit breaker rules, and decide whether a service belongs on Container Apps or AKS.

## Topics

| Topic | Description |
| --- | --- |
| [Dapr Sidecar: Run with Aspire](./01-sidecar/readme.md) | `dapr init`, `AddDapr`, `WithDaprSidecar`, component YAML and the Dapr 1.18 target |
| [Service Invocation: mTLS & Retries](./02-service-invocation/readme.md) | `DaprClient.InvokeMethodAsync`, `CreateInvokeHttpClient`, app IDs and Sentry mTLS |
| [State Stores: ETags & Concurrency](./03-state-stores/readme.md) | `SaveStateAsync`, `GetStateAndETagAsync`, `TrySaveStateAsync`, `ExecuteStateTransactionAsync` and the Cosmos DB state component |
| [Pub/Sub: Swap Brokers via Components](./04-pub-sub/readme.md) | `PublishEventAsync`, the `Topic` attribute, dead-letter topics and the Service Bus topics component |
| [Resiliency Policies: Retries & Breakers](./05-resiliency/readme.md) | The Resiliency spec for timeouts, retries and circuit breakers, plus Container Apps component resiliency (preview) |
| [Dapr Actors: Virtual Actor Model](./06-actors/readme.md) | `Actor`, `IRemindable`, `RegisterTimerAsync`, `RegisterReminderAsync` and `ActorProxy.Create` for `VanActor` |
| [Managed Dapr: Container Apps vs. AKS](./07-managed-dapr/readme.md) | Container Apps component tiers and limits, the AKS `Microsoft.Dapr` extension and the version window |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/review` | Reviews the branch's component YAML and Resiliency spec before a merge, so a wrong target name or trip condition is caught early |
| `/security-review` | Audits the branch for connection strings, master keys and plain-text secrets in component files, which this module asks you to move into a secret store |
| `/doctor` | Checks the Claude Code installation and the MCP servers this class uses for Microsoft Learn lookups, before you start a Dapr session |
| `/cost` | Shows the token use and cost of a session that generates component files and actor code, so you can compare the cost of a long refactor |

## Key Topics covered in this module

- [Microservice APIs powered by Dapr](https://learn.microsoft.com/azure/container-apps/dapr-overview)
- [Dapr documentation](https://docs.dapr.io/)
- [Install the Dapr extension for AKS](https://learn.microsoft.com/azure/aks/dapr)
- [Community Toolkit Dapr integration for Aspire](https://github.com/CommunityToolkit/Aspire/tree/main/src/CommunityToolkit.Aspire.Hosting.Dapr)
