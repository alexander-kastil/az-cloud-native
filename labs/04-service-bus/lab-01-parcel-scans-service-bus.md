# Process Parcel Scans Reliably with Service Bus

Today the tracking service has no ordered, poison-safe or duplicate-safe path for parcel scans. By the end, Contoso Logistics reads scans from a Service Bus topic through a Functions trigger, keeps each parcel in order, dead-letters unreadable scans, stores each scan once, and scales the consumer on Container Apps with a KEDA rule.

What you practice here: consuming one event stream with filtered subscriptions, sessions, dead-lettering and an idempotent store, with a gate test for each pattern.

Duration: about 55 minutes.

Topics practiced: [Topic Subscriptions: SQL Filters](../../demos/04-service-bus/01-subscriptions-filters/readme.md), [Azure Functions: Service Bus Triggers](../../demos/04-service-bus/02-functions-triggers/readme.md), [Competing Consumers: KEDA Scale Rules](../../demos/04-service-bus/03-competing-consumers/readme.md), [Sessions: Ordered Message Processing](../../demos/04-service-bus/04-sessions/readme.md), [Dead-Letter Queues: Poison Messages](../../demos/04-service-bus/05-dead-letter/readme.md), [Idempotent Consumers: Duplicate Detection](../../demos/04-service-bus/06-idempotent-consumers/readme.md)

------

Open `labs/04-service-bus/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack. Start Docker Desktop first, because the Service Bus and Cosmos DB emulators run as containers.

---

## Gate Tests

The starter contains these four gate tests in `tests/ContosoLogistics.Tests`. Each one fails until the step that builds its pattern is done. Run one gate with `dotnet test tests/ContosoLogistics.Tests --filter "Gate=FilteredSubscriptionReceivesOnlyItsEvents"`.

| Gate | What it proves |
| --- | --- |
| `FilteredSubscriptionReceivesOnlyItsEvents` | A ParcelScanned, a ParcelDelivered and a DeliveryDelayed event each reach only the subscriptions whose SQL filter selects them. |
| `ScansOfOneParcelStayInOrder` | Three scans of one parcel, sent with the parcel ID as session ID and interleaved with another parcel's scans, are applied in the order they were sent. |
| `UnreadableScanDeadLettered` | A scan with an unreadable body lands in the tracking dead-letter queue with a reason, and the readable scans of that parcel still apply in order. |
| `DuplicateScanStoredOnce` | Applying the same scan message twice through the consumer's store leaves one scan entry and one processed marker. |

The routing gate reads subscriptions directly, so the test holds the scan processor back while it runs.

## Step 1: Model the Parcel Events Topic

Overview: The shipping API publishes every parcel event to one topic. Each consumer needs only its own events, so the tracking service, the notification service and the exception agent each get a filtered subscription. You define them in the AppHost model, so the local emulator and the deployed namespace get the same topology.

Finding: A good answer adds the topic and three subscriptions in `src/AppHost/Program.cs` with correlation filter rules on the message Subject. Aspire models correlation filters, and the same model deploys to Azure later, which a SQL filter typed into an emulator config file would not. Push back if a subscription keeps a rule that matches every message.

The emulator has no Microsoft Entra ID, autoscale or large-message support. Push back if the answer connects with `DefaultAzureCredential`; local connections use the emulator's SAS connection string, and the KEDA step runs in Azure.

Recipe:

```text
In the AppHost project src/AppHost/Program.cs, add the topic parcel-events to the existing Service Bus resource messaging, which already runs as the emulator locally and deploys as Azure Service Bus. Define three subscriptions on it in the AppHost model, so the emulator and the deployed namespace get the same topology:
- tracking, with one correlation filter rule on Subject = ParcelScanned
- customer-messages, with two correlation filter rules, Subject = ParcelDelivered and Subject = DeliveryDelayed
- exceptions, with one correlation filter rule on Subject = DeliveryDelayed

Each subscription holds only its own rules and no rule that matches every message. Reference messaging from every project that sends or receives on the topic. Add no comments that restate the code.
```

Expected Outcome: The AppHost starts the Service Bus emulator with the topic parcel-events and its three subscriptions. Run the gate `FilteredSubscriptionReceivesOnlyItsEvents`. It passes, and it fails if customer-messages receives a ParcelScanned event or exceptions receives a ParcelDelivered event.

Restart the AppHost after any change to the topology, because the emulator reads it only at startup.

## Step 2: Process Scans in an Isolated Functions Trigger

Overview: The tracking service runs its scan processor as an isolated Functions app. The function starts for each message on the tracking subscription and settles the message only after its work succeeds, so a crash leaves the scan on the subscription for another attempt.

Finding: A good answer has a `ScanFunctions` class with a Service Bus trigger on parcel-events and tracking, AutoCompleteMessages set to false, a ServiceBusReceivedMessage binding with message actions, and maxConcurrentCalls set to 8 in `host.json`. Push back if the answer keeps AutoCompleteMessages at true while it also binds message actions, because the runtime then settles the message before the code decides. Push back if it hard-codes a connection string in the function project; the AppHost passes the emulator connection string instead.

Recipe:

```text
Create the isolated Azure Functions project src/ScanProcessor/ for .NET 10. Reference Microsoft.Azure.Functions.Worker.Extensions.ServiceBus version 5.14.1 or later.

In src/ScanProcessor/ScanFunctions.cs, add a class ScanFunctions with one function named ProcessScan. Its Service Bus trigger listens to the topic parcel-events and the subscription tracking, reads its connection from the app setting ServiceBusConnection, and sets AutoCompleteMessages to false. The function binds the message as ServiceBusReceivedMessage and binds the message actions, logs the MessageId and Subject, and then completes the message.

In src/ScanProcessor/host.json, set extensions.serviceBus.maxConcurrentCalls to 8 and extensions.serviceBus.prefetchCount to 0.

Add src/ScanProcessor/.gitignore with the .NET patterns and local.settings.json included. Add src/ScanProcessor/local.settings.example.json that lists the setting names the project reads.

In src/AppHost/Program.cs, add the ScanProcessor project to the AppHost and pass the emulator connection string to it as the ServiceBusConnection setting. Add no comments that restate the code.
```

Expected Outcome: No gate covers this step. With the AppHost running, run `dotnet test tests/ContosoLogistics.Tests --filter "Gate=ScansOfOneParcelStayInOrder"`. The test fails its final assertion until step 3, which is expected. Until then, the Functions console logs one line per scan with its MessageId and Subject.

## Step 3: Keep Each Parcel's Scans in Order with Sessions

Overview: Scans of one parcel must apply in order, or the tracking page can show a van leaving before the hub scan lands. Sessions give each parcel its own ordered lane, and the parcel ID serves as the session ID.

Finding: A good answer sets RequiresSession on the tracking subscription, sets IsSessionsEnabled on the trigger, and writes one read-model document per parcel with an ordered scans array in the tracking container (partition key `/trackingNumber`). Push back if the answer processes two scans of one session in parallel or keys the ordering on MessageId, because the session lock guards the order.

Once tracking requires sessions, a scan without a session ID dead-letters with the reason `Session ID is null`. The publisher must set the parcel ID as the session ID on every scan.

Recipe:

```text
In src/AppHost/Program.cs, set RequiresSession to true on the tracking subscription of parcel-events.

In src/ScanProcessor/ScanFunctions.cs, set IsSessionsEnabled to true on the ProcessScan trigger, and settle each message with the session-aware message actions the trigger provides.

In src/ScanProcessor/host.json, set extensions.serviceBus.maxConcurrentSessions to 8.

Create src/ScanProcessor/ParcelScanStore.cs with a public class ParcelScanStore. Its constructor takes the Microsoft.Azure.Cosmos Container for the existing tracking container (database logistics, partition key /trackingNumber). It has a public method Task<bool> TryAppendAsync(string trackingNumber, string scanType, string messageId) that appends the scan type as a string to an ordered array named scans in the document with id parcel-{trackingNumber}, creating the document when it does not exist, and returns true.

In src/AppHost/Program.cs, give scan-processor a reference to the cosmos resource and wait for it. In src/ScanProcessor/Program.cs, register the Cosmos DB client with the Aspire client integration for the connection named cosmos, and register ParcelScanStore with the container tracking of the database logistics.

The function deserializes the message body into the ParcelScanned record from src/Contracts with JsonSerializerDefaults.Web and a JsonStringEnumConverter. It passes the session ID (the tracking number), the record's scan type as a string and the MessageId to the store, and completes the message after the store returns. Add no comments that restate the code.
```

Expected Outcome: Run the gate `ScansOfOneParcelStayInOrder`. It passes, and the parcel's document holds PickedUp, AtHub and OutForDelivery in the order they were sent, even though another parcel's scans arrived in between.

## Step 4: Dead-Letter Unreadable Scans with a Reason

Overview: A label scanner can produce a scan that no handler can parse, and every redelivery spends handler time on it. The function dead-letters that message with a reason, so an operator sees why it failed while the parcel's other scans keep moving.

Finding: A good answer checks each scan before it reaches the store, and it dead-letters an unreadable scan with the reason `UnreadableLabel` and a description that names the error. It dead-letters only that message, so the session stays open for the parcel's later scans. Push back if the answer abandons unreadable scans, because each abandon adds a delivery until the broker dead-letters the scan with the reason `MaxDeliveryCountExceeded`, which says nothing about the label.

Recipe:

```text
In src/ScanProcessor/ScanValidator.cs, add a static method that takes the message body and returns either the parsed ParcelScanned record from src/Contracts or an error description. Deserialize with JsonSerializerDefaults.Web and a JsonStringEnumConverter. A scan is unreadable when its body does not deserialize into ParcelScanned, or when its scan type is not PickedUp, AtHub or OutForDelivery.

In src/ScanProcessor/ScanFunctions.cs, call the validator before the store. For an unreadable scan, dead-letter that message with the reason UnreadableLabel and a description that names the error, using the session-aware message actions. Dead-letter only that message, so the session stays open for the parcel's later scans.

Add no comments that restate the code.
```

Expected Outcome: Run the gate `UnreadableScanDeadLettered`. It passes, and the malformed scan sits in the dead-letter queue of the tracking subscription with the reason UnreadableLabel. The readable scans of that parcel are applied in order around it.

## Step 5: Make the Consumer Duplicate-Safe

Research: Ask the agent to reason before it writes code. Its answer should come from the Microsoft Learn pages it reads, not from memory.

```text
Read the Microsoft Learn pages on Service Bus duplicate detection and on Cosmos DB transactional batch operations. Explain where duplicate protection for a parcel scan must live in Contoso Logistics: at the broker, at the consumer, or at both. Cover the duplicate detection window, what a crash between the store write and the message completion does on redelivery, and how a single write can store a scan and its marker together.
```

Finding: A good answer names the 10 minute default duplicate detection window and says that the broker checks only sends. It says the consumer needs its own marker, because a crash or a lost lock redelivers the message to the handler. It proposes one Cosmos DB transactional batch that writes the marker and the scan update together, on partition `/trackingNumber`.

Push back if it proposes a check-then-write sequence, because a crash between the two writes repeats or loses the work, or if the marker sits in a different partition from the parcel document, because a batch stays in one partition.

Recipe:

```text
In src/AppHost/Program.cs, enable duplicate detection on the parcel-events topic with a history window of 10 minutes.

In src/ScanProcessor/ParcelScanStore.cs, change TryAppendAsync so it writes the scan and a processed marker in one Cosmos DB transactional batch on partition key /trackingNumber. The marker item has the id processed-{messageId}. The same batch appends the scan type to the parcel document's scans array. When the batch conflicts on the marker, the method returns false and writes nothing.

In src/ScanProcessor/ScanFunctions.cs, a false result completes the message and logs its MessageId as a duplicate. Any other failure throws, so the message returns to the subscription.

Add no comments that restate the code.
```

Expected Outcome: Run the gate `DuplicateScanStoredOnce`. It passes. After the same message reaches the store twice, the parcel document holds one entry for the scan, and the tracking container holds one `processed-{MessageId}` marker.

## Step 6: Deploy Once and Scale the Consumer with KEDA

Overview: Scan bursts at a depot can outrun one consumer, so the consumer app scales on the backlog of the tracking subscription. This step adds one replica for every ten active scans, up to ten replicas. The scaler reads the backlog with a managed identity, so the app holds no connection string. It runs in Azure, because the emulator has no autoscale.

Run the provisioning script once before you start: `bash labs/04-service-bus/provision.azcli`. It grants your signed-in user the right to send to the Service Bus namespace that lab 3 deployed into `rg-azn-labs`, so the backlog sender can authenticate with your account.

Recipe:

```text
Deploy the AppHost once with aspire deploy into the existing Container Apps environment in resource group rg-azn-labs. The deployment adds the topic parcel-events with its three subscriptions to the deployed Service Bus namespace and adds the scan-processor app.

Create the console project src/ScanBacklog/ScanBacklog.csproj for .NET 10 that references Azure.Messaging.ServiceBus and Azure.Identity, and add it to ContosoLogistics.sln. src/ScanBacklog/Program.cs takes the fully qualified Service Bus namespace as its first argument and a message count as its second. It sends that many ParcelScanned messages to parcel-events through DefaultAzureCredential, each with the Subject ParcelScanned, a SessionId equal to its tracking number and a web-default JSON body with the scan type as a string, cycling the scan type through PickedUp, AtHub and OutForDelivery.

Create infra/scan-consumer.azcli as a bash script. It reads the Service Bus namespace name and the scan-processor app's user-assigned identity from resource group rg-azn-labs, then updates the container app scan-processor with minimum replicas 0, maximum replicas 10 and one custom scale rule named scan-backlog of type azure-servicebus with the metadata namespace, topicName parcel-events, subscriptionName tracking and messageCount 10, authenticated with that identity.

Add no comments that restate the code.
```

Expected Outcome: Run the script, send 60 scans, then count the replicas. From the repository root in Git Bash or Cloud Shell:

```bash
bash infra/scan-consumer.azcli
dotnet run --project src/ScanBacklog -- $(az servicebus namespace list --resource-group rg-azn-labs --query "[0].name" --output tsv).servicebus.windows.net 60
az containerapp replica list --name scan-processor --resource-group rg-azn-labs --query "length(@)" --output tsv
```

The sender exits after it sends 60 messages. The replica count rises from zero toward six over several 30-second polling intervals.

The scan-processor drains the backlog while it scales, so the count rises and then falls back toward zero once the subscription is empty. If the count stays at zero for several minutes, the role assignment is still propagating; run the replica command again.

## Step 7: Record the Patterns and Run Every Gate

Overview: The consumer now has four patterns and one scale rule, and the next team needs to find the code behind each one. This step records that map and runs the four gates together against the final consumer.

Recipe:

```text
Create docs/scan-processing.md with a table that has one row per pattern. Columns: Gate, Service Bus feature, Implemented in. The rows are FilteredSubscriptionReceivesOnlyItsEvents (subscription SQL filters), ScansOfOneParcelStayInOrder (sessions), UnreadableScanDeadLettered (dead-lettering), and DuplicateScanStoredOnce (duplicate detection with a processed marker). Name the file that implements each feature. Add a final row for the KEDA scale rule that names infra/scan-consumer.azcli.

Keep connection strings, keys and resource IDs out of the document.
```

Expected Outcome: Run `dotnet test tests/ContosoLogistics.Tests --filter "Gate=FilteredSubscriptionReceivesOnlyItsEvents|Gate=ScansOfOneParcelStayInOrder|Gate=UnreadableScanDeadLettered|Gate=DuplicateScanStoredOnce"`. All four gates pass in one run. The file `docs/scan-processing.md` lists the four patterns and the KEDA rule, each with its implementing file.

## Solution

The finished state is in `labs/04-service-bus/contoso-logistics-solution/`, which the steps of this guide produce. Compare your `src/`, `infra/` and `docs/` folders with it when you finish.

## Cleanup

Keep the Azure environment: the next lab deploys into the same resource group `rg-azn-labs`. Stop the AppHost. Run `deprovision.azcli` only if you end the class before lab 9; it deletes `rg-azn-labs` with everything the labs created.
