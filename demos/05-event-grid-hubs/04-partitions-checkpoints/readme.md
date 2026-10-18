# Event Hubs: Partitions & Checkpoints

> **_NOTE:_** Duration: about 13 minutes.

Every van in the Contoso Logistics fleet reports its GPS position as a steady stream. A further consumer, such as a reporting job, could need every position in order for one van, and the ops console needs the latest position of every van. Event Hubs handles this volume, and the keys that producers set and the way consumers track progress decide whether the results stay correct.

This topic covers partitions and partition keys, consumer groups, the `EventProcessorClient` with a Blob checkpoint store, how processor instances share partitions, and what at-least-once delivery means for replay. It ends with Capture, which archives the stream to storage in Avro format.

## Partitions and Partition Keys

An event hub splits its stream into partitions, and each partition is an ordered log. Event Hubs guarantees order only within one partition, so the design question is which events must stay in order together. Using the van identifier as the partition key keeps every position of one van in one partition, in arrival order.

The producer sets the key, and Event Hubs hashes it to choose the partition. Events without a key go to partitions in round-robin order. Sending to a partition ID directly ties the producer to that partition and lowers availability, so most designs should use keys.

![Positions of van-07, van-12 and van-21 are routed by PartitionKey into the four partition logs of van-positions, positions without a key go round-robin, and the ops-console consumer group and an example reporting group read the same logs with their own cursors](_images/partition-cursors.svg)

```csharp
using Azure.Identity;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

await using var producer = new EventHubProducerClient(
    "contoso-telemetry.servicebus.windows.net",
    "van-positions",
    new DefaultAzureCredential());

var batchOptions = new CreateBatchOptions { PartitionKey = "van-07" };
using EventDataBatch batch = await producer.CreateBatchAsync(batchOptions);

var position = new EventData("{\"VanId\":\"van-07\",\"Sequence\":1,\"Latitude\":48.2082,\"Longitude\":16.3738}");
batch.TryAdd(position);

await producer.SendAsync(batch);
```

The `CreateBatchOptions` object carries the partition key for the whole batch. Every event added to that batch goes to the same partition, which is the behavior that an ordered consumer relies on.

## Consumer Groups

A consumer group is a separate view of the whole event hub. Each group reads the full stream at its own pace, so a further consumer and the ops console can both read every position. Event Hubs allows up to five concurrent readers per partition in a consumer group, but you should run one active reader per partition and group.

Each reader in a group receives every event from its partitions. Two active readers in the same group therefore process the same events twice. Give each consuming service its own consumer group and its own checkpoint container.

| Consumer group | Reader | Purpose |
| --- | --- | --- |
| `reporting` (example) | Further consumer, added later | Reads the same positions at its own pace |
| `ops-console` | `telemetry-ingest` ASP.NET Core service | Pushes positions to the browser through SignalR |

## EventProcessorClient and the Blob Checkpoint Store

The `EventProcessorClient` in the `Azure.Messaging.EventHubs.Processor` package reads all partitions of a consumer group. It writes ownership and checkpoint records to a `BlobContainerClient`, which the processor uses as its checkpoint store. The container must exist before the processor starts, because the processor never creates it, so the lab calls `CreateIfNotExistsAsync` first.

```csharp
using Azure.Identity;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

var checkpointContainer = new BlobContainerClient(
    new Uri("https://contosologisticscp.blob.core.windows.net/ops-console"),
    new DefaultAzureCredential());

var processor = new EventProcessorClient(
    checkpointContainer,
    "ops-console",
    "contoso-telemetry.servicebus.windows.net",
    "van-positions",
    new DefaultAzureCredential());

processor.ProcessEventAsync += async args =>
{
    if (!args.HasEvent)
    {
        return;
    }

    Console.WriteLine(args.Data.EventBody.ToString());
    await args.UpdateCheckpointAsync();
};

processor.ProcessErrorAsync += args =>
{
    Console.WriteLine(args.Exception.Message);
    return Task.CompletedTask;
};

await processor.StartProcessingAsync();
Console.ReadLine();
await processor.StopProcessingAsync();
```

The handler runs for each event, and `HasEvent` tells the handler whether it received a real event or an empty wake-up. The error handler must be registered, because the processor reports its internal failures there. Call `StopProcessingAsync` before the process exits, because stopping releases the partitions so that other instances can take them over promptly.

The lab's telemetry-ingest service uses the connection-string overloads instead, with the strings that Aspire injects, because the lab runs against emulators.

Use a separate checkpoint container for each consumer group, and don't store anything else in it. Learn recommends disabling hierarchical namespace, blob soft delete and versioning on the storage account. The identity needs the Storage Blob Data Contributor or Storage Blob Data Owner role on the container.

With Microsoft Entra ID, the processor identity also needs the Azure Event Hubs Data Owner role on the event hub, because a grant on the consumer group alone isn't enough.

## Load Balancing and Ownership

Each processor instance writes an ownership record for the partitions it reads. Instances check the checkpoint store regularly, see which partitions the others own, and claim partitions until the load is balanced. The checkpoint store guarantees that only one instance claims a partition at a time, so at most one processor reads a given partition at any moment.

When an instance crashes, its records stop changing. The other instances see the old last-modified time, and they take over its partitions from the last checkpoint.

![Two telemetry-ingest instances on the ops-console consumer group write a checkpoint after every event: instance A owns partitions 0 and 1 and instance B owns 2 and 3, B pushes event 8 of partition 2 and crashes before its checkpoint, its ownership records stop changing, and A takes over partitions 2 and 3 from the last checkpoint and processes event 8 again](_images/ownership-takeover.svg)

Scaling out works the same way: a new instance causes the existing instances to release some of their partitions. The number of partitions sets the upper limit on parallel readers in a group, so an instance beyond that limit stays idle.

## At-Least-Once Delivery, Replay and Capture

Event Hubs delivers at least once. A processor that crashes after it handles an event and before it writes the checkpoint sees that event again after the takeover. Make the handler idempotent, for example by storing each event ID and skipping IDs that it has already processed.

Checkpoint frequency is a trade-off. A checkpoint after every event costs a blob write per event, while less frequent checkpoints reduce that cost and let a crash replay a few more events. Replay also works in the other direction, because a processor can start from an earlier position to reprocess history, within the retention period.

The lab's processor sets `DefaultStartingPosition` to `EventPosition.Earliest` in a `PartitionInitializingAsync` handler, so a partition without a checkpoint replays its retained events from the beginning. It calls `UpdateCheckpointAsync` after every push, which is the per-event policy that this section describes.

Capture writes the stream to Blob Storage or Data Lake Storage in Avro format. A capture window closes at a minimum size or a minimum time, whichever comes first.

Capture is included in the Premium and Dedicated tiers, while the Standard tier charges for it monthly by throughput unit. Retention limits replay: Standard keeps events for up to 7 days, Premium and Dedicated for up to 90 days, and Capture archives only events that arrive after you enable it.

## Key Topics

- [Partition load balancing for event processing](https://learn.microsoft.com/azure/event-hubs/event-processor-balance-partition-load)
- [Event Hubs features and terminology](https://learn.microsoft.com/azure/event-hubs/event-hubs-features)
- [Availability and consistency in Event Hubs](https://learn.microsoft.com/azure/event-hubs/event-hubs-availability-and-consistency)
- [Capture events through Azure Event Hubs in Azure Blob Storage or Azure Data Lake Storage](https://learn.microsoft.com/azure/event-hubs/event-hubs-capture-overview)
- [Quickstart: Send events to and receive events from Azure Event Hubs using .NET](https://learn.microsoft.com/azure/event-hubs/event-hubs-dotnet-standard-getstarted-send)
