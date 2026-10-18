# Class rebuild: Event-Driven Distributed Apps on Azure

Agreed on 2026-10-18: 9 modules, 56 topics, 4 days. Old material lives in `demos-old/` and `labs-old/`. Order of work: teaching first, then demos and labs on the owner's go.

- [x] Layout agreed (9 modules, 56 topics)
- [x] Sample domain chosen: Contoso Logistics, last-mile parcel delivery
- [x] Teaching: `demos/readme.md`, 9 module readmes, 56 topic readmes
- [ ] Demos: the trainer presents the key point of each lab step, running the lab's solution code (no separate demo projects); a topic the lab does not touch gets a short standalone demo (AKS manifests, Dapr actors, Dapr Workflow, Module 1 concepts)
- [ ] Labs: one per module, building up the Contoso Logistics app (lab N starts from lab N-1); prompt-based, the AI writes all code, participants prompt, review and verify; Aspire integration tests are the gate for every pattern (duplicate message, restart mid-saga, poison message); a solution checkpoint after each major step; tool-neutral prompts (GitHub Copilot, Claude Code); each starter carries agent instructions for the class stack and the Microsoft Learn MCP server

## Open decisions (owner)

- `aspire deploy` and `aspire publish` carry a preview label in the Aspire CLI reference; Module 3 teaches them as preview.
- Learn recommends the Microsoft OpenTelemetry Distro for new apps; Module 3 teaches `UseAzureMonitor` and names the new distro.
- Event Hubs Kafka transactions: one Learn page says preview, another says not supported; Module 5 says preview with a caveat.
- 153 tracked build-output files and 18 tracked `local.settings.json` files stay in git until untracked with `git rm --cached`.

## Authoring contract (read before writing any readme)

### Audience and register

Azure developers and software architects at AI-200 level (Azure AI Cloud Developer Associate, formerly AZ-204), writing C# on .NET 10. Developer register: name the stack, the APIs and the SDK packages. Python appears only where a topic is polyglot or AI.

### Topic readme shape

```markdown
---
minutes: 10
---

# <Topic title, exactly as in the TOC>

<Two short intro paragraphs: the problem in Contoso Logistics terms, then what this topic covers.>

## <Descriptive concept heading>

<Concept-first learning prose, real syntax in fenced code, a comparison table where one fits.>

## Key Topics

- [<Official doc title>](<URL>)
```

- Three to six concept sections under descriptive H2 headings (never `Overview`, `Introduction`, `Summary`, `Demos`).
- 600 to 900 words of prose, not counting code. `minutes` is the prose words divided by 80, rounded to 5, at least 10.
- Show real syntax for every mechanism the prose claims: C# with the real package and API names, JSON, YAML, Bicep, KQL or CLI. Never invent an API, a CLI flag or a config key. If you cannot confirm the syntax, describe the concept without code.
- No `Hands-On Demo` or `Hands-On Lab` sections yet; they are added with the demos and labs.
- Key Topics: three to six official links (learn.microsoft.com, aspire.dev, docs.dapr.io, kafka.apache.org, cloudevents.io).

### Module readme shape

```markdown
# Module N: <Module title>

<Two or three intro paragraphs: the module's role in the class storyline and what the learner can do afterwards, in Contoso Logistics terms.>

## Topics

| Topic | Description |
| --- | --- |
| [<Topic title>](./01-<slug>/readme.md) | <one line naming the APIs and features the topic teaches> |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |

## Key Topics covered in this module

- [<Official doc title>](<URL>)
```

- Slash commands: only `/review`, `/security-review`, `/doctor`, `/cost`, `/find-skills`, `/model`, `/init`, `/update-config`, three to five per module, each explained for this module's topic. Never repeat the same explanation across modules.

### Writing rules

- No em dashes anywhere; use `,` `;` `:` or `()`.
- At most 3 sentences per paragraph.
- No bold or italic in prose.
- Every code fence declares a language.
- Mermaid labels use `"quoted<br/>labels"`, never `\n`.
- Internal links are relative; topic folders link as `./NN-slug/readme.md`.
- It is "Microsoft Foundry", never "Azure AI Foundry". It is "Aspire", not ".NET Aspire", except when quoting an old name.
- Validate every product fact (names, GA or preview status, versions, limits, syntax) on Microsoft Learn through the MS Learn MCP tools, or on aspire.dev and docs.dapr.io. Mark preview features as preview. Prefer the facts below; if Learn says otherwise, follow Learn and say so in your report.

### Code folders

Every runnable project folder (starter and solution alike) ships its own `.gitignore` for its stack:

| Stack | Patterns |
| --- | --- |
| .NET | `[Bb]in/`, `[Oo]bj/`, `.vs/`, `*.user`, `*.suo`, `[Tt]est[Rr]esult*/`, `*.db`, `*.db-shm`, `*.db-wal`, `.env`, plus `local.settings.json` for Functions projects |
| Node / Angular | `node_modules/`, `dist/`, `out-tsc/`, `.angular/`, `coverage/`, `*.log`, `.env` |
| Python | `.venv/`, `__pycache__/`, `*.pyc`, `.pytest_cache/`, `.env` |

Ship settings that a demo needs as `local.settings.example.json` or `.env.example`, never the real file.

## Contoso Logistics: the sample domain

A last-mile parcel delivery company. Customers book shipments, vans pick up and deliver parcels, scanners and van telematics report every step, and an operations console shows the network live.

| Service | Role | Owns |
| --- | --- | --- |
| `shipping-api` | Books shipments, owns the Shipment aggregate | Event store in Cosmos DB (`shipments`, partition `/shipmentId`) |
| `tracking-service` | Ingests parcel scans, serves the tracking page | Read model `tracking` (partition `/trackingNumber`), Redis cache |
| `routing-service` | Assigns vans and routes, handles reroutes | One Dapr actor per van (`VanActor`) |
| `billing-service` | Charges and refunds | Payments |
| `label-service` | Prints and voids shipping labels, stores proof-of-delivery photos | Blob Storage |
| `notification-service` | Customer email and SMS, live push to browsers | SignalR Service hub |
| `telemetry-ingest` | Receives van GPS positions | Event Hubs (Kafka endpoint) |
| `exception-agent` (Python) | AI agent for delivery exceptions, embeddings of delivery notes | Microsoft Foundry model, Cosmos DB vector index |
| `ops-console` (Angular) | Live map, parcel search, approvals | |

Commands: `BookShipment`, `ReserveCapacity`, `CapturePayment`, `PrintLabel`, `AssignRoute`, `RerouteParcel`, `NotifyCustomer`.

Events: `ShipmentBooked`, `CapacityReserved`, `PaymentCaptured`, `LabelPrinted`, `ParcelScanned` (scan types `PickedUp`, `AtHub`, `OutForDelivery`), `VanPositionReported`, `DeliveryAttempted`, `DeliveryDelayed`, `ParcelDelivered`, `ProofOfDeliveryUploaded`, `RerouteRequested`, `RerouteApproved`, `PaymentRefunded`.

Flows the modules reuse:

- Booking saga (Module 8): `ReserveCapacity`, `CapturePayment`, `PrintLabel`; compensations `ReleaseCapacity`, `RefundPayment`, `VoidLabel`.
- Delivery deadline (Module 8): a durable timer raises `DeliveryDelayed` when a parcel misses its promised window; the customer approves a reroute as an external event.
- Scan ordering (Module 4): Service Bus session id = parcel id, so scans of one parcel are processed in order.
- Poison messages (Module 4): unreadable label scans end in the dead-letter queue.
- Van telemetry (Module 5): a simulator produces GPS positions to Event Hubs over Kafka; the ops console map receives them through SignalR.
- Proof of delivery (Module 5): a photo uploaded to Blob Storage raises an Event Grid system event; partners receive `ParcelDelivered` through a custom topic or namespace pull.
- Tracking page (Module 6): change feed projects shipment events into the tracking read model and invalidates the Redis entry.
- Exception agent (Module 9): reacts to `DeliveryDelayed`, drafts the customer message, proposes a reroute that a human approves.

## Verified facts (Oct 2026, from this session's research)

- .NET 10 is the current LTS; .NET 8 and the Azure Functions in-process model reach end of support on 10 Nov 2026. Functions use the isolated worker (`Microsoft.Azure.Functions.Worker`); Flex Consumption is the recommended serverless plan; Linux Consumption retires 30 Sep 2028.
- Aspire 13.6 (29 Sep 2026; the name dropped ".NET" with Aspire 13 in Nov 2025). `RunAsEmulator` for Service Bus, Event Hubs, Cosmos DB (selects the vNext Linux emulator in 13.6) and Storage (Azurite). `Aspire.Hosting.Azure.Functions` 13.6.1 with `AddDurableTaskScheduler`. ServiceDefaults: OpenTelemetry, `AddStandardResilienceHandler`, service discovery, `/health` and `/alive` in Development. `aspire publish` and `aspire deploy` target Container Apps; Kubernetes publish is supported, AKS ingress and Gateway API are preview. `Aspire.Hosting.Testing` with `DistributedApplicationTestingBuilder`. Dapr integration is the Community Toolkit package `CommunityToolkit.Aspire.Hosting.Dapr`. Docs: https://aspire.dev
- Container Apps: internal ingress and calls by app name inside an environment; revision modes Single, Multiple (traffic splitting) and labels (preview); KEDA scale rules for Service Bus and Event Hubs support managed identity through the `identity` property (`identity: 'system'` or a resource id, CLI or ARM/Bicep only, not the portal); managed identity for ACR pulls (`acrPull`); the managed OpenTelemetry agent is preview; Container Apps Express (GA Sep 2026) has no Dapr, no jobs, no sidecars, no traffic splitting.
- Container Apps managed Dapr: "the Dapr server extensions, actor, and workflow SDK packages aren't compatible with Azure Container Apps"; no version choice; Dapr component resiliency is preview; Dapr is not supported for jobs; actor reminders need minReplicas 1. AKS Dapr extension (`Microsoft.Dapr`) supports the current and previous Dapr versions. https://learn.microsoft.com/azure/container-apps/dapr-overview
- Dapr 1.17 (27 Feb 2026, .NET `Dapr.Workflow` rewrite without DurableTask, .NET 10), 1.18 (Jun 2026, workflow release: signed history, `WorkflowAccessPolicy`, concurrency limits), 1.19 in preview. Dapr Agents 1.0 GA 23 Mar 2026 (Python).
- Service Bus: sessions, duplicate detection (default window 10 minutes, keyed on MessageId), dead-letter queue (default MaxDeliveryCount 10, reasons such as MaxDeliveryCountExceeded and TTLExpiredException), scheduled and deferred messages, transactions with send-via, auto-forwarding (max 4 hops), SQL filters and actions on subscriptions. Sessions, duplicate detection, transactions need Standard or Premium. Legacy SDKs `Microsoft.Azure.ServiceBus` and `WindowsAzure.ServiceBus` retired 30 Sep 2026; use `Azure.Messaging.ServiceBus`. Emulator: Linux container, no SLA, no Entra ID, 50 entities.
- Event Grid: custom topics and system topics (Blob Storage and others); retry schedule up to 24 hours, max 30 attempts, TTL up to 1440 minutes, dead-letter to a storage account; namespaces with pull delivery (receive, acknowledge, release, reject, renew lock), CloudEvents 1.0, MQTT broker, push from namespaces to Event Hubs.
- Event Hubs: Kafka endpoint on Standard, Premium and Dedicated (not Basic); `EventProcessorClient` with a Blob checkpoint store, one owner per partition, at-least-once; Capture writes Avro.
- Cosmos DB: change feed modes latest version and all versions and deletes (all versions and deletes GA Jun 2026, needs continuous backup); change feed processor with a lease container, at-least-once; transactional batch on one partition key (100 operations, 2 MB); vector search with DiskANN and full-text and hybrid search GA; vNext Linux emulator GA Jun 2026. Architecture Center "Transactional Outbox with Azure Cosmos DB" (updated Oct 2026).
- Durable Task Scheduler: Dedicated and Consumption SKUs GA (the older Durable Functions billing page still says preview); emulator `mcr.microsoft.com/dts/dts-emulator:latest` (dashboard on port 8082); Durable Task .NET SDK GA (`Microsoft.DurableTask.Worker.AzureManaged`, `Microsoft.DurableTask.Client.AzureManaged`), runs on Container Apps with the KEDA scaler `azure-durabletask-scheduler`; Durable Functions isolated with `storageProvider.type` `azureManaged`. Payload limit 1 MB per orchestrator, activity or entity input.
- Microsoft Agent Framework 1.0 (Apr 2026, .NET and Python); durable agents through the Durable Task extension; workflow checkpoints with `CosmosCheckpointStorage` (Python). Microsoft.Extensions.AI `IChatClient`. A2A 1.0 is a Linux Foundation project (150+ organizations).
- Architecture Center pattern pages updated in 2026: saga (choreography and orchestration), CQRS, event sourcing, competing consumers, idempotent consumer, claim check, priority queue.

## Module and topic slugs (titles are the TOC, H1 and module table text)

| Folder | Title |
| --- | --- |
| `01-distributed-design` | Designing Event-Driven Distributed Systems |
| `01-distributed-design/01-monolith-vs-microservices` | Modular Monolith vs. Microservices |
| `01-distributed-design/02-sync-async` | Sync vs. Async: Commands & Events |
| `01-distributed-design/03-event-storming` | Event Storming: Find Service Boundaries |
| `01-distributed-design/04-event-types` | Event Types: Domain, Integration, CloudEvents |
| `01-distributed-design/05-reliability-patterns` | Reliability Patterns: Outbox, Saga, Idempotency |
| `01-distributed-design/06-pick-the-broker` | Azure Messaging: Pick the Broker |
| `02-aspire` | Building Distributed Apps with Aspire |
| `02-aspire/01-apphost` | Aspire AppHost: Model the System |
| `02-aspire/02-service-defaults` | ServiceDefaults: Telemetry, Resilience, Discovery |
| `02-aspire/03-integrations` | Aspire Integrations: Hosting & Client |
| `02-aspire/04-dashboard` | Aspire Dashboard: Traces Across Services |
| `02-aspire/05-polyglot` | Polyglot AppHost: Angular & Python |
| `02-aspire/06-testing` | Aspire Testing: Distributed Integration Tests |
| `03-hosting` | Hosting on Container Apps & AKS |
| `03-hosting/01-aspire-deploy` | Aspire Deploy: ACR & Container Apps |
| `03-hosting/02-key-vault-app-config` | Key Vault & App Configuration |
| `03-hosting/03-revisions` | Revisions: Traffic Splitting & Draining |
| `03-hosting/04-aks` | AKS: Aspire-Generated Kubernetes Manifests |
| `03-hosting/05-app-insights` | Application Insights: Traces & KQL |
| `04-service-bus` | Reliable Messaging with Service Bus |
| `04-service-bus/01-subscriptions-filters` | Topic Subscriptions: SQL Filters |
| `04-service-bus/02-functions-triggers` | Azure Functions: Service Bus Triggers |
| `04-service-bus/03-competing-consumers` | Competing Consumers: KEDA Scale Rules |
| `04-service-bus/04-sessions` | Sessions: Ordered Message Processing |
| `04-service-bus/05-dead-letter` | Dead-Letter Queues: Poison Messages |
| `04-service-bus/06-idempotent-consumers` | Idempotent Consumers: Duplicate Detection |
| `05-event-grid-hubs` | Events & Streams with Event Grid & Event Hubs |
| `05-event-grid-hubs/01-custom-topics` | Event Grid: Custom Topics & Subscriptions |
| `05-event-grid-hubs/02-filters-retries` | Event Grid: Filters, Retries, Dead-Letter |
| `05-event-grid-hubs/03-namespaces-pull` | Event Grid Namespaces: Pull Delivery |
| `05-event-grid-hubs/04-partitions-checkpoints` | Event Hubs: Partitions & Checkpoints |
| `05-event-grid-hubs/05-kafka` | Event Hubs: Kafka-Compatible Streaming |
| `05-event-grid-hubs/06-signalr` | SignalR Service: Push to Browsers |
| `06-event-sourcing-cqrs` | Event Sourcing & CQRS with Cosmos DB |
| `06-event-sourcing-cqrs/01-outbox` | Transactional Outbox: Atomic Publishing |
| `06-event-sourcing-cqrs/02-change-feed-processor` | Change Feed Processor: Projections |
| `06-event-sourcing-cqrs/03-cqrs` | CQRS: Separate Writes from Reads |
| `06-event-sourcing-cqrs/04-read-models` | Read Models: Indexing, RUs, Consistency |
| `06-event-sourcing-cqrs/05-redis-cache` | Managed Redis: Change-Feed Cache Invalidation |
| `06-event-sourcing-cqrs/06-event-store` | Event Store: Aggregates & Snapshots |
| `06-event-sourcing-cqrs/07-event-versioning` | Event Versioning: Evolve Schemas Safely |
| `07-dapr` | Dapr Building Blocks for Portable Services |
| `07-dapr/01-sidecar` | Dapr Sidecar: Run with Aspire |
| `07-dapr/02-service-invocation` | Service Invocation: mTLS & Retries |
| `07-dapr/03-state-stores` | State Stores: ETags & Concurrency |
| `07-dapr/04-pub-sub` | Pub/Sub: Swap Brokers via Components |
| `07-dapr/05-resiliency` | Resiliency Policies: Retries & Breakers |
| `07-dapr/06-actors` | Dapr Actors: Virtual Actor Model |
| `07-dapr/07-managed-dapr` | Managed Dapr: Container Apps vs. AKS |
| `08-durable-execution` | Durable Execution, Workflows & Sagas |
| `08-durable-execution/01-durable-execution` | Durable Execution: Replay & Determinism |
| `08-durable-execution/02-durable-task-scheduler` | Durable Task Scheduler: Managed Backend |
| `08-durable-execution/03-durable-functions` | Durable Functions: Orchestrations & Entities |
| `08-durable-execution/04-timers-events` | Durable Timers: External Events & Approvals |
| `08-durable-execution/05-durable-task-sdk` | Durable Task SDK: KEDA-Scaled Workers |
| `08-durable-execution/06-dapr-workflow` | Dapr Workflow: Portable Orchestrations |
| `08-durable-execution/07-saga` | Saga Orchestration: Compensating Activities |
| `09-ai-agents` | AI Agents in Event-Driven Systems |
| `09-ai-agents/01-extensions-ai` | Microsoft.Extensions.AI: Foundry Model Client |
| `09-ai-agents/02-agent-subscribers` | Agent Subscribers: React to Events |
| `09-ai-agents/03-durable-llm-calls` | LLM Calls: Durable Activities |
| `09-ai-agents/04-rate-limits` | Rate Limits: Priority Queues & KEDA |
| `09-ai-agents/05-vector-search` | Vector Search: Change Feed Embeddings |
| `09-ai-agents/06-agent-choreography` | Agent Choreography: Events vs. A2A |

## Lab contract (read before writing or running any lab)

### Storyline

| Folder | Lab H1 | What the participant's AI builds | Topics practiced | Shown only in a demo | Azure | Minutes / steps |
| --- | --- | --- | --- | --- | --- | --- |
| `labs/01-distributed-design` | Event-Storm the Parcel Delivery Flow | `docs/events.md` (events, commands, aggregates), `docs/architecture.md` (bounded contexts as a Mermaid service map, broker per flow, reliability pattern per flow) | all six | none | no | 45 / 5 |
| `labs/02-aspire` | Model Contoso Logistics in an Aspire AppHost | AppHost, ServiceDefaults, `Contoso.Contracts` (event records from lab 1), shipping-api, tracking-service, Service Bus and Cosmos DB emulators, Angular ops-console and Python exception-agent stub in the AppHost, first gate test | all six | none | no | 55 / 7 |
| `labs/03-hosting` | Ship Contoso Logistics to Container Apps | `aspire deploy` to Container Apps, Key Vault secret (carrier API key) and App Configuration setting over managed identity, a tracking-service v2 revision with a traffic split, a KQL query over the booking trace | 01, 02, 03, 05 | 04 AKS | yes | 55 / 7 |
| `labs/04-service-bus` | Process Parcel Scans Reliably with Service Bus | Topic `parcel-events` with SQL-filter subscriptions, isolated Functions scan processor, session per parcel id, dead-lettering of unreadable scans, duplicate-safe consumer with a processed-message store, KEDA scale rule | all six | none | KEDA step | 55 / 7 |
| `labs/05-event-grid-hubs` | Stream Van Positions to a Live Map | Van simulator producing over Kafka to Event Hubs, telemetry-ingest with `EventProcessorClient` and checkpoints, SignalR push to the ops-console map, `ParcelDelivered` on an Event Grid custom topic, proof-of-delivery Blob system event | 01, 02, 04, 05, 06 | 03 namespace pull | Event Grid steps | 45 / 5 |
| `labs/06-event-sourcing-cqrs` | Event-Source Shipments with an Outbox | Shipment event store with a concurrency check, outbox in a transactional batch, change feed relay to Service Bus and tracking projection, Redis cache invalidation, indexing policy for the tracking queries | 01 to 06 | 07 versioning | no | 60 / 7 |
| `labs/07-dapr` | Make the Services Portable with Dapr | Service Bus SDK swapped for Dapr pub/sub, routing state in a Dapr state store, shipping-api to routing-service invocation with a resiliency policy, a local `VanActor`, the components on Container Apps | all seven | none | managed Dapr step | 65 / 8 |
| `labs/08-durable-execution` | Orchestrate the Booking Saga | Booking saga (ReserveCapacity, CapturePayment, PrintLabel, compensations in reverse) on the Durable Task Scheduler emulator, delivery deadline timer raising `DeliveryDelayed`, reroute approval external event, KEDA-scaled Durable Task SDK worker | 01 to 05, 07 | 06 Dapr Workflow | KEDA step | 70 / 9 |
| `labs/09-ai-agents` | Add an Exception Agent to the Event Flow | `DeliveryDelayed` subscriber using `IChatClient` against a Microsoft Foundry deployment, the model call as a durable activity with retry, a priority queue with a KEDA replica cap, the Python embedding worker with Cosmos DB vector search | 01 to 05 | 06 events vs A2A | Foundry model | 55 / 7 |

Each lab starts from the previous lab's solution (`starter` of lab N = `solution` of lab N-1). Lab 1 starts from an empty `contoso-logistics/` folder.

### Folder layout per lab

- `labs/NN-<module>/lab-01-<slug>.md`: the guide (one lab per module, so always `lab-01`).
- `labs/NN-<module>/contoso-logistics/`: the starter (built in the execution phase, not by guide writers).
- `labs/NN-<module>/contoso-logistics-solution/`: the finished state, produced by running the guide.
- `labs/NN-<module>/provision.azcli` and `deprovision.azcli` for labs that need Azure.
- Code layout inside `contoso-logistics/`: `ContosoLogistics.sln`, `src/<ServiceName>/`, `src/AppHost/`, `src/ServiceDefaults/`, `src/Contracts/`, `web/ops-console/`, `agents/exception-agent/`, `tests/ContosoLogistics.Tests/`, `docs/`, `AGENTS.md`, `CLAUDE.md` (points to `AGENTS.md`), `.github/copilot-instructions.md` (points to `AGENTS.md`), `.mcp.json` and `.vscode/mcp.json` with the Microsoft Learn MCP server (`https://learn.microsoft.com/api/mcp`), `.gitignore`.

### Guide shape (prompt-recipe, the AI writes all code)

````markdown
# <Lab H1 from the storyline>

<At most three sentences: what is broken or missing in Contoso Logistics today and what the participant holds at the end.>

What you practice here: <one sentence>.

Duration: about <NN> minutes.

Topics practiced: <links to the topic readmes, ../../demos/NN-module/NN-topic/readme.md>

------

Open `labs/NN-<module>/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack.

---

## Gate Tests

| Gate | What it proves |
| --- | --- |
| `<GateName>` | <one sentence> |

## Step <n>: <Title>

Overview: <what this step achieves and why it matters>

Research: <only for a real design decision; an open prompt in a text block>

Finding: <what a good answer contains; when to push back>

Recipe:

```text
<self-contained, tool-neutral prompt that names every file and folder it reads or writes>
```

Expected Outcome: <observable result, and the gate command where one applies>

## Solution

<names contoso-logistics-solution once>

## Cleanup

<only for Azure labs: run deprovision.azcli>
````

- Step count and minutes from the storyline table; 8 minutes per step.
- Steps form one storyline toward the named deliverable; later steps consume earlier outputs.
- Prompts are tool-neutral: no slash commands, no tool-specific features, no personal skills. They state the outcome and the constraints, never paste the solution code.
- Gate tests: xUnit with Aspire.Hosting.Testing in `tests/ContosoLogistics.Tests`, one trait per gate, run with `dotnet test tests/ContosoLogistics.Tests --filter "Gate=<GateName>"`. The guide names each gate and what it asserts; the execution phase writes the tests into the starter before the prompts run, and they fail until the step is done.
- Expected outcomes describe signals in words until the execution phase replaces them with trimmed real output; never invent output.
- Azure: subscription "Visual Studio Enterprise Subscription", one shared resource group `rg-azn-labs` in `swedencentral`, created in lab 3 and reused by labs 4 to 9; each lab's `provision.azcli` only adds its own resources; the environment stays up between labs and lab 9's Cleanup deletes the group. Lab 9 uses a Microsoft Foundry project in the shared resource group with a chat deployment `contoso-chat` and an embedding deployment `contoso-embed`.
- Writing rules from the authoring contract apply (no em dashes, at most 3 sentences per paragraph, every fence declares a language, prompts in `text` fences).

### Execution phase (one runner per lab, strictly one at a time)

1. Starter `labs/NN-<module>/contoso-logistics/`:
   - Lab 1: create it with only the agent files: `AGENTS.md` (class stack: .NET 10, Aspire 13.6, isolated Functions, `Azure.Messaging.ServiceBus`, `Microsoft.Azure.Cosmos` v3, Dapr 1.18, Durable Task SDK with the Durable Task Scheduler, Microsoft.Extensions.AI; the Contoso domain names and the code layout above; the gate command; no secrets in code; look facts up with the Microsoft Learn MCP server), `CLAUDE.md` and `.github/copilot-instructions.md` (both point to `AGENTS.md`), `.mcp.json`, `.vscode/mcp.json`, `.gitignore`, empty `docs/`.
   - Lab 2 to 9: a copy of the previous lab's `contoso-logistics-solution/` without build output (`bin`, `obj`, `node_modules`, `.venv`, `dist`, `.angular`).
   - Then write this lab's gate tests into `tests/ContosoLogistics.Tests` (xUnit, Aspire.Hosting.Testing, `[Trait("Gate", "<GateName>")]`; earlier labs' gates stay). Confirm they fail before the prompts run.
2. Run copy: copy the starter to `labs/NN-<module>/contoso-logistics-run-tmp/` and run the guide from there, the participant's workspace: `python C:/Users/AlexanderKastil/.claude/skills/create-class/scripts/run-guide-recipes.py --guide <guide> --cwd <run-tmp> --out <scratchpad>/runs/labNN-<segment> --marker "Recipe:" --model sonnet --steps <a-b>`, .
3. Prompts are workspace-relative: participants open `contoso-logistics/` as the workspace, so a prompt never names `labs/NN-<module>/` paths. Fix any guide prompt that does.
4. After each segment run the gates of the finished steps. A failing gate means the prompt or the setup is wrong: fix the guide (smallest change), restore the run copy from the last checkpoint and rerun the step, at most twice; then stop and report.
5. Azure labs: run `provision.azcli` before the first Azure step, in subscription "Visual Studio Enterprise Subscription", shared resource group `rg-azn-labs`, `swedencentral`; reuse what earlier labs deployed and keep it running. Only the lab 9 runner runs `deprovision.azcli` and confirms the deletion started. Report every resource created.
6. Outputs: `contoso-logistics-solution/` at the end, copied from the run copy without build output; then delete the run copy. Run `python C:/Users/AlexanderKastil/.claude/skills/create-class/scripts/class-gitignores.py --root labs/NN-<module> --apply`. `git ls-files -s | grep ^160000` prints nothing.
7. Replace the guide's worded expected outcomes with trimmed real output where the run produced it; never invent output.
8. Leave nothing running: stop the AppHost, emulator containers, Dapr processes and workers the run started (`docker ps` shows none of them).
9. No prompt may depend on a long-running foreground process (`aspire run`, `func start`, `dapr run`): a headless run hangs until the 10-minute limit. Use bounded commands (`aspire start` then `aspire stop`, the gate tests, a script that exits) and make watching the dashboard a participant action in the Expected Outcome.
10. This machine sets `DOTNET_ASPIRE_CONTAINER_RUNTIME=podman`; export `DOTNET_ASPIRE_CONTAINER_RUNTIME=docker` in the runner's shell before any Aspire command.
12. No time box: every step is executed and fixed until its gates pass, never skipped or marked unverified. The coordinator watches every run continuously and steps in at once when a step stalls or loops.
11. Local first, deploy once: every lab builds and tests its code locally (AppHost with emulators, or Azure resources that Aspire provisions for local runs) until its gates pass, then runs `aspire deploy` exactly once near the end. Cloud-only behaviour (revisions and traffic splits, KEDA scale rules, managed Dapr components, Event Grid delivery) is configured and checked after that single deploy with `az` commands, never with another deploy. A full deploy takes about 2 minutes and resets revision settings. Restructure a guide before running it when it deploys more than once.

## One capability, one home

Each topic teaches only its own capability and links to the topic that owns a neighbour, for example the outbox is taught in `06-event-sourcing-cqrs/01-outbox` and only named in Module 1. Sibling classes own: AZ-204/AI-200 basics (App Service, Storage basics), `pro-code-agents` (Agent Framework depth, MCP servers, A2A hosting, durable agents), `az-400` (CI/CD, Bicep pipelines).
