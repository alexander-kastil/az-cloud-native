# Module 8: Durable Execution, Workflows & Sagas

Contoso Logistics promises a delivery window for every parcel, and a single booking touches capacity, payment, and labels in separate services. A process that restarts halfway through has to resume where it stopped and undo the steps it already finished. This module teaches the code that makes a long-running process survive restarts, from the managed backend to the saga that books a shipment.

The module follows one path. Topic 1 explains replay and determinism, which every later topic depends on.

Topics 2 and 3 cover the managed backend and the Durable Functions model that runs on it, and topics 4 and 5 add timers, external events, and self-hosted workers. Topic 6 shows the portable Dapr alternative, and topic 7 combines the pieces into the booking saga.

After this module, you can write a durable orchestration in C#, pause it for a delivery deadline or a customer approval, scale its workers with KEDA, and design a saga that refunds payment and releases capacity when a later step fails.

## Topics

| Topic | Description |
| --- | --- |
| [Durable Execution: Replay & Determinism](./01-durable-execution/readme.md) | Event history, replay, the orchestrator determinism rules, and the context APIs that replace non-deterministic calls |
| [Durable Task Scheduler: Managed Backend](./02-durable-task-scheduler/readme.md) | Schedulers, task hubs, Dedicated and Consumption SKUs, the emulator, managed identity roles, and the dashboard |
| [Durable Functions: Orchestrations & Entities](./03-durable-functions/readme.md) | `OrchestrationTrigger`, `ActivityTrigger`, `TaskEntityDispatcher`, the `azureManaged` provider in `host.json`, and client starts |
| [Durable Timers: External Events & Approvals](./04-timers-events/readme.md) | `CreateTimer`, `WaitForExternalEvent`, racing a timer against an event, and `RaiseEventAsync` |
| [Durable Task SDK: KEDA-Scaled Workers](./05-durable-task-sdk/readme.md) | `Microsoft.DurableTask.Worker.AzureManaged`, generic host registration, and the `azure-durabletask-scheduler` scale rule |
| [Dapr Workflow: Portable Orchestrations](./06-dapr-workflow/readme.md) | `Workflow<TInput, TOutput>`, `WorkflowActivity<TInput, TOutput>`, `DaprWorkflowClient`, Dapr 1.18 features, and AKS hosting |
| [Saga Orchestration: Compensating Activities](./07-saga/readme.md) | Booking saga compensations in reverse order, retry options, idempotent activities, and choreography versus orchestration |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/review` | Reviews the branch for orchestrator code that reads the clock, creates GUIDs, or calls HTTP directly, because replay makes those calls unsafe. |
| `/security-review` | Audits changes to scheduler connection settings, managed identity bindings, and role assignments on task hubs. |
| `/cost` | Shows token usage and session cost after a long design session on the booking saga, which usually needs several review passes. |
| `/doctor` | Checks the Claude Code installation and the MCP servers that query Microsoft Learn, so the API checks in this module run against current documentation. |

## Key Topics covered in this module

- [Durable Task Scheduler overview](https://learn.microsoft.com/azure/durable-task/scheduler/durable-task-scheduler)
- [Durable Task SDKs overview](https://learn.microsoft.com/azure/durable-task/sdks/durable-task-overview)
- [Durable Functions overview](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-overview)
- [Dapr workflow overview](https://docs.dapr.io/developing-applications/building-blocks/workflow/workflow-overview/)
- [Saga distributed transactions pattern](https://learn.microsoft.com/azure/architecture/patterns/saga)
