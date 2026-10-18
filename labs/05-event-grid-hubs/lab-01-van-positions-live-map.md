# Stream Van Positions to a Live Map

The ops console shows no vans, and no partner learns when a parcel is delivered. By the end, van positions stream into the console as they arrive, and a delivery photo sent to storage tells a partner that the parcel is delivered.

What you practice here: consuming a partitioned stream with checkpoints, then routing events to browsers and partners through managed services.

Duration: about 45 minutes.

Topics practiced: [Event Grid: Custom Topics & Subscriptions](../../demos/05-event-grid-hubs/01-custom-topics/readme.md), [Event Grid: Filters, Retries, Dead-Letter](../../demos/05-event-grid-hubs/02-filters-retries/readme.md), [Event Hubs: Partitions & Checkpoints](../../demos/05-event-grid-hubs/04-partitions-checkpoints/readme.md), [Event Hubs: Kafka-Compatible Streaming](../../demos/05-event-grid-hubs/05-kafka/readme.md), [SignalR Service: Push to Browsers](../../demos/05-event-grid-hubs/06-signalr/readme.md)

------

Open `labs/05-event-grid-hubs/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack, and every path in the prompts is relative to that folder.

Setup: Docker Desktop must run for Steps 1 and 2. Before Step 3, sign in with `az login` if the Azure CLI asks for it, then run `bash labs/05-event-grid-hubs/provision.azcli` from the repository root. The script creates every Azure resource that Steps 3 to 5 use, all in `rg-azn-labs`.

---

## Gate Tests

The starter ships `tests/ContosoLogistics.Tests` with one test per gate. Each gate fails until its step is done. Run one gate from `labs/05-event-grid-hubs/contoso-logistics/` with `dotnet test tests/ContosoLogistics.Tests --filter "Gate=<GateName>"`.

| Gate | What it proves |
| --- | --- |
| `PositionsReachEventHub` | Step 1: the van simulator sends positions over the Kafka endpoint, and `van-positions` holds at least five positions for each of the three vans, with consecutive `Sequence` values per van. |
| `PositionsReachHub` | Step 2: `telemetry-ingest` reads `van-positions` through the `ops-console` consumer group, and a SignalR client connected to `/hubs/van-positions` receives at least five positions for each van. |
| `ProcessorResumesFromCheckpoint` | Step 2: after `telemetry-ingest` stops and starts again, each van's `Sequence` values continue with no gap. |
| `PositionsPushedThroughSignalRService` | Step 3: with the Azure SignalR connection string set, the negotiate response points the client at a `service.signalr.net` host, and positions still arrive. |
| `ParcelDeliveredReachesPartner` | Step 4: a `ParcelDelivered` CloudEvent published to the `egt-contoso-labs05` custom topic arrives in the `partner-delivered` queue with the same `ParcelId` within two minutes. |
| `ProofOfDeliveryReachesPartner` | Step 5: a photo uploaded to the `proof-of-delivery` container raises a `BlobCreated` event, the label service publishes `ParcelDelivered`, and the `partner-delivered` queue receives it within three minutes. |

## Step 1: Send Van Positions to Event Hubs over Kafka

Overview: The telematics vendors already publish to Kafka, so the van simulator uses a Kafka client, and the local Event Hubs emulator accepts it unchanged. Keying each message by van id keeps one van's positions in order within one partition.

Recipe:

```text
Work in this folder. Read AGENTS.md first.

1. In src/Contracts/, add the record VanPositionReported in the namespace Contoso.Contracts with these properties: VanId (string), Sequence (long), Latitude (double), Longitude (double) and ObservedAt (DateTimeOffset).
2. In src/AppHost/, add an Azure Event Hubs resource named event-hubs that runs as the emulator. Add the hub van-positions to it, with one consumer group named ops-console. Set the partition count of van-positions to 4 in the emulator configuration callback, because the emulator reads its configuration only when it starts.
3. Create the console project src/VanSimulator/ with a reference to Contracts and the Confluent.Kafka package. Every second, send one VanPositionReported for each of three vans, VanId van-07, van-12 and van-21, with Sequence increasing by one per van from 1. Use the VanId as the Kafka message key and the JSON-serialized record as the value.
4. Configure the producer for the emulator's Kafka endpoint: bootstrap server localhost:9092, security protocol SASL_PLAINTEXT, SASL mechanism PLAIN, SASL username $ConnectionString, and the SASL password taken from the emulator connection string that the AppHost injects.
5. Register VanSimulator in the AppHost so that it starts after event-hubs and receives the connection string.
6. In tests/ContosoLogistics.Tests/, make the test with the trait Gate=PositionsReachEventHub pass. It starts the AppHost, waits 10 seconds, then reads van-positions from the $Default consumer group and asserts at least five positions for each van, with consecutive Sequence values per van.

Do not change the existing services. Write no code comments.
```

Expected Outcome: The Aspire dashboard lists `event-hubs` and `van-simulator` as running, and the simulator runs without errors. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PositionsReachEventHub"` passes.

## Step 2: Checkpoint the Processor and Push to the Live Map

Overview: The processor must resume where it stopped, and the map must show each position as it arrives. This step adds a processor with a blob checkpoint store, a SignalR hub, and a map panel in the ops console.

Research:

```text
Read the Microsoft Learn pages on Event Hubs event processing and checkpoints, using the Microsoft Learn MCP server. Compare two checkpoint policies for a processor that pushes each van position to a SignalR hub: checkpoint after every position, and checkpoint after each batch of positions. For each policy, explain what a crash between the push and the checkpoint means for the positions a restart delivers. Do not change any file.
```

Finding: A good answer names at-least-once delivery, says that a restart replays the events after the last stored checkpoint, and weighs one blob write per position against the replays after a crash. Push back if it claims exactly-once delivery, or if it keeps checkpoints in memory where the blob container should hold them.

Recipe:

```text
Work in this folder. Keep the AppHost and the van simulator from Step 1.

1. In src/AppHost/, add an Azure Storage resource named storage that runs as the Azurite emulator, with a blob container named ops-console. Give telemetry-ingest references to event-hubs and storage.
2. Create the ASP.NET Core project src/TelemetryIngest/ with a reference to ServiceDefaults, and register it in the AppHost. Use the Azure.Messaging.EventHubs.Processor package with an EventProcessorClient that reads van-positions through the consumer group ops-console. Store its checkpoints in the ops-console blob container with the Azure.Storage.Blobs package. Use the connection strings that Aspire injects.
3. For each event, deserialize VanPositionReported, send it to every client connected to the hub method vanPositionReported, and then update the checkpoint. Report processor errors through the processor's error handler.
4. Add the hub class VanPositionHub in src/TelemetryIngest/, mapped at /hubs/van-positions. Allow the origin of the ops-console in CORS, with credentials.
5. In web/ops-console/, add a map panel that connects to /hubs/van-positions with the @microsoft/signalr package, listens for vanPositionReported, and draws the latest position of each van as a labelled dot on an SVG plane. Plot longitude as x and latitude as y.
6. Make the gate tests PositionsReachHub and ProcessorResumesFromCheckpoint pass. ProcessorResumesFromCheckpoint stops telemetry-ingest after at least 20 positions for each van, starts it again, and asserts that each van's Sequence values continue without a gap. Restart only telemetry-ingest, because the emulator loses its entities when its container restarts.

Do not add a second consumer group. Write no code comments.
```

Expected Outcome: The ops console draws one dot per van, and the dots move while the simulator runs. Stop `telemetry-ingest` in the Aspire dashboard and start it again: the dots keep moving with no gap in any van's sequence numbers. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PositionsReachHub"` and `dotnet test tests/ContosoLogistics.Tests --filter "Gate=ProcessorResumesFromCheckpoint"` pass.

## Step 3: Switch the Hub to Azure SignalR Service

Overview: Browsers should connect through a managed push service, which then holds the client connections for the hub. This step adds one switch: when the Azure SignalR connection string is set, the hub runs through Azure SignalR Service in Default mode, and the ops console keeps the same hub URL.

The provision script allows all origins for the lab, so restrict the origin list before any real use.

Run from `labs/05-event-grid-hubs/contoso-logistics/`:

```bash
dotnet user-secrets init --project src/TelemetryIngest
dotnet user-secrets set "Azure:SignalR:ConnectionString" "$(az signalr key list --name sigr-contoso-labs05 --resource-group rg-azn-labs --query primaryConnectionString --output tsv)" --project src/TelemetryIngest
```

In PowerShell, run the `az signalr key list` command on its own and pass its output as the value of `dotnet user-secrets set`.

Recipe:

```text
Work in this folder. The Azure SignalR connection string is in the user secrets of src/TelemetryIngest/ under Azure:SignalR:ConnectionString.

1. Add the Microsoft.Azure.SignalR package to src/TelemetryIngest/.
2. In src/TelemetryIngest/Program.cs, keep the AddSignalR registration. When the configuration key Azure:SignalR:ConnectionString has a value, also call AddAzureSignalR on that registration. When the key is empty, the hub keeps running locally.
3. Keep the hub route /hubs/van-positions and the method name vanPositionReported unchanged, so web/ops-console needs no change.
4. Make the gate test PositionsPushedThroughSignalRService pass. The test reads the connection string from the user secrets of src/TelemetryIngest/, starts the AppHost, negotiates with the hub, and asserts that the negotiate response points at a host that ends in service.signalr.net. It then asserts that a position arrives at a connected client.

Write no code comments.
```

Expected Outcome: The ops console still draws the vans. The negotiate request to `/hubs/van-positions` returns an endpoint on `sigr-contoso-labs05.service.signalr.net`, which you can see in the browser's network panel. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PositionsPushedThroughSignalRService"` passes.

## Step 4: Publish ParcelDelivered to the Partner Queue

Overview: A partner receives `ParcelDelivered` as a CloudEvent from a queue it can read on its own schedule. `provision.azcli` created the custom topic `egt-contoso-labs05`, which accepts CloudEvents, and the `partner-delivered` subscription, which routes `ParcelDelivered` to the `partner-delivered` queue. This step writes the publisher.

Recipe:

```text
Work in this folder.

1. In src/Contracts/, add the record ParcelDelivered with ParcelId (string) and DeliveredAt (DateTimeOffset).
2. Create the worker project src/LabelService/ with the Azure.Messaging.EventGrid, Azure.Identity and Azure.Storage.Queues packages. Its settings come from user secrets: Azure:EventGrid:TopicEndpoint and Azure:Storage:AccountName.
3. In src/LabelService/, add the class ParcelDeliveredPublisher with a method that sends one ParcelDelivered CloudEvent to the custom topic. Use the source /label-service, the event type ParcelDelivered, and the record as the data. Authenticate with DefaultAzureCredential through EventGridPublisherClient.
4. Reference src/LabelService/ from tests/ContosoLogistics.Tests/. Make the gate test ParcelDeliveredReachesPartner pass. The test publishes a ParcelDelivered for a new ParcelId, then peeks at the partner-delivered queue of the storage account with DefaultAzureCredential until it finds a message for that ParcelId with the type ParcelDelivered, or fails after two minutes. The test reads the two settings from the user secrets of src/LabelService/.

Do not register src/LabelService in the AppHost. Write no code comments.
```

After the agent creates `src/LabelService/`, run this block from `labs/05-event-grid-hubs/contoso-logistics/`:

```bash
dotnet user-secrets init --project src/LabelService
dotnet user-secrets set "Azure:EventGrid:TopicEndpoint" "$(az eventgrid topic show --name egt-contoso-labs05 --resource-group rg-azn-labs --query endpoint --output tsv)" --project src/LabelService
dotnet user-secrets set "Azure:Storage:AccountName" "stcontosolabs05" --project src/LabelService
```

Expected Outcome: `dotnet test tests/ContosoLogistics.Tests --filter "Gate=ParcelDeliveredReachesPartner"` passes. The message stays in the `partner-delivered` queue after the test, because the test only peeks.

## Step 5: Turn a Proof-of-Delivery Photo into ParcelDelivered

Overview: A proof-of-delivery photo is the real trigger for `ParcelDelivered`, so the label service reacts to the storage event on its own, without a manual call. `provision.azcli` created the system topic that raises `BlobCreated` for the `proof-of-delivery` container and routes it to the `pod-events` queue. This step writes the handler that publishes `ParcelDelivered`.

Recipe:

```text
Work in this folder.

1. In src/LabelService/, add a background worker that reads the pod-events queue of the storage account named by the setting Azure:Storage:AccountName, with DefaultAzureCredential and the Azure.Storage.Queues package.
2. For each message, parse the CloudEvent. When its type is Microsoft.Storage.BlobCreated and its subject names the proof-of-delivery container, take the ParcelId from the blob name without its extension (PKG-10482.jpg gives PKG-10482), publish it with ParcelDeliveredPublisher, and delete the message only after the publish succeeds.
3. Delete messages of any other type or container without publishing them.
4. Run the worker inside the gate test. Make the gate test ProofOfDeliveryReachesPartner pass. The test uploads a small photo for a new ParcelId to the proof-of-delivery container of the storage account, then peeks at partner-delivered for up to three minutes until it finds a ParcelDelivered message for that ParcelId.

Write no code comments.
```

Expected Outcome: Uploading a photo named for a parcel to the `proof-of-delivery` container in the Azure portal produces one `ParcelDelivered` message in `partner-delivered` for that parcel, with no manual publish step. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=ProofOfDeliveryReachesPartner"` passes.

## Solution

The finished state is in `labs/05-event-grid-hubs/contoso-logistics-solution/`. Compare your folder with it when the gate tests pass.

## Cleanup

Keep the Azure environment: the next lab deploys into the same resource group `rg-azn-labs`. Stop the AppHost. Run `deprovision.azcli` only if you end the class before lab 9; it deletes `rg-azn-labs` with everything the labs created.
