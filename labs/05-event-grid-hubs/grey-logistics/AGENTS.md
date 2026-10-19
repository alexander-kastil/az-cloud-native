# Grey Logistics: agent instructions

Last-mile parcel delivery system built in the class "Event-Driven Distributed Apps on Azure". You write all code and documents; the participant prompts, reviews and verifies.

## Stack

- .NET 10 and C# 14
- Aspire 13.6 for the AppHost and ServiceDefaults
- Azure Functions isolated worker (`Microsoft.Azure.Functions.Worker`), never the in-process model
- `Azure.Messaging.ServiceBus`, never the retired `Microsoft.Azure.ServiceBus`
- `Microsoft.Azure.Cosmos` v3
- Dapr 1.18
- Durable Task SDK with the Durable Task Scheduler
- Microsoft.Extensions.AI (`IChatClient`)
- Angular for the ops console, Python for the exception agent

## Domain names (use them exactly)

- Services: `shipping-api`, `tracking-service`, `routing-service`, `billing-service`, `label-service`, `notification-service`, `telemetry-ingest`, `exception-agent`, `ops-console`
- Commands: `BookShipment`, `ReserveCapacity`, `CapturePayment`, `PrintLabel`, `AssignRoute`, `RerouteParcel`, `NotifyCustomer`
- Events: `ShipmentBooked`, `CapacityReserved`, `PaymentCaptured`, `LabelPrinted`, `ParcelScanned` (scan types `PickedUp`, `AtHub`, `OutForDelivery`), `VanPositionReported`, `DeliveryAttempted`, `DeliveryDelayed`, `ParcelDelivered`, `ProofOfDeliveryUploaded`, `RerouteRequested`, `RerouteApproved`, `PaymentRefunded`
- Compensations: `ReleaseCapacity`, `RefundPayment`, `VoidLabel`

## Code layout

- `GreyLogistics.sln`
- `src/<ServiceName>/`, `src/AppHost/`, `src/ServiceDefaults/`, `src/Contracts/`
- `web/ops-console/`, `agents/exception-agent/`
- `docs/`

## Rules

- No secrets in code or in committed files; use Aspire parameters, Key Vault or managed identity.
- No code comments; no em dashes in documents.
- Look up product facts, APIs and package names with the Microsoft Learn MCP server (`.mcp.json`) before using them.
- Do only what the prompt asks.
