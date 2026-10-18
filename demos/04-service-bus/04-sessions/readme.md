# Sessions: Ordered Message Processing

> **_NOTE:_** Duration: about 8 minutes.

A parcel's scans must apply in order. A parcel scanned `PickedUp`, then `AtHub`, then `OutForDelivery` must show that sequence on the tracking page. If two scans of the same parcel run in parallel, the page can show the van leaving before the hub scan lands.

This topic covers session-enabled queues and subscriptions, the parcel ID as the session ID, `ServiceBusSessionProcessor`, session state, session locks, and the tier that supports sessions.

## Session-Enabled Entities and Session IDs

A session-enabled queue or subscription sets `RequiresSession` when you create it. Messages that share a `SessionId` form a first-in, first-out sequence. Different sessions run in parallel, while the messages inside one session stay in order.

Contoso uses the tracking number as the session ID, so every scan of one parcel shares a session. A message sent to a session-enabled entity without a session ID dead-letters with the reason `Session ID is null`. The scan processor receives these sessions through a Functions trigger with `IsSessionsEnabled = true` (`src/ScanProcessor/ScanFunctions.cs:11`).

Only one receiver can hold a session's lock at a time. Abandoning a message makes the next receive serve that same message again, so a failing scan holds back the later scans of its parcel until it succeeds or moves to the dead-letter queue.

![Interleaved scans sort into one lane per SessionId on the tracking subscription, and each locked lane drains in order to its own scan-processor receiver while the lanes run in parallel; an abandoned AtHub scan holds back only PKG-100482, and after SessionIdleTimeout the quiet session's lock opens and receiver 3 takes the next session](_images/session-lanes.svg)

```csharp
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

var admin = new ServiceBusAdministrationClient("<NAMESPACE>.servicebus.windows.net", new DefaultAzureCredential());
await admin.CreateQueueAsync(new CreateQueueOptions("scans") { RequiresSession = true });

var client = new ServiceBusClient("<NAMESPACE>.servicebus.windows.net", new DefaultAzureCredential());
await using var sender = client.CreateSender("scans");

await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromString("{\"scanType\":\"PickedUp\"}"))
{
    SessionId = "PKG-100482"
});
```

## Ordered Processing with ServiceBusSessionProcessor

`ServiceBusSessionProcessor` accepts sessions one at a time up to a limit, and it delivers each session's messages in the order they were sent. `MaxConcurrentSessions` sets how many sessions the processor works on at once, and its default is 8. Different sessions run in parallel, so throughput grows with the number of parcels in flight.

```csharp
var processorOptions = new ServiceBusSessionProcessorOptions
{
    MaxConcurrentSessions = 8,
    SessionIdleTimeout = TimeSpan.FromSeconds(30),
    MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(5)
};

await using var processor = client.CreateSessionProcessor("scans", processorOptions);

processor.ProcessMessageAsync += async args =>
{
    string scanType = args.Message.Body.ToString();
    await args.SetSessionStateAsync(BinaryData.FromString(scanType));
};

await processor.StartProcessingAsync();
```

`SessionIdleTimeout` closes a session when the processor waits that long for its next message, and then the processor moves on to another session. When you don't set it, the processor uses the client's `TryTimeout` value. Set it low for sparse sessions with few scans, and set it high for parcels whose scans arrive over hours, because a low value closes sessions too early.

Thread starvation is a common risk with the session processor. Microsoft Learn names a very high `MaxConcurrentSessions` relative to the core count as a frequent cause, and it advises against sync-over-async code in handlers. Scale out to more hosts with moderate concurrency when a single host stalls.

## Session State, Locks, and Tier Requirements

Session state stores a small value with a session. The value survives processor restarts, so a new processor can read the last scan it applied and skip duplicates.

Set the state with `SetSessionStateAsync` and read it with `GetSessionStateAsync`. A state can hold one message's worth of data, up to 256 KB on Standard and 100 MB on Premium. The state stays until you clear it with an empty value, and it counts toward the entity's storage quota.

A state works well as a checkpoint, such as the last step that a parcel reached, because a new processor can resume from it.

The underlying partitions of a session-enabled entity sometimes move to balance load across Service Bus nodes, and a session lock can be lost when they do. A settlement that runs after the lock is lost fails, so keep the lock renewal running while the handler works.

A session lock works like an exclusive lock on a file. The processor renews the lock while it works, up to `MaxAutoLockRenewalDuration`, which defaults to 5 minutes. If the lock expires, the session fails with `SessionLockLost`, and another receiver can take the session.

The idle timeout releases a session once its parcel goes quiet for that long, so other receivers can take other sessions sooner.

Sessions need the Standard or Premium tier, because the Basic tier supports queues only. Standard adds topics, subscriptions, sessions, transactions, and duplicate detection. Premium adds dedicated messaging units, larger messages, and private networking, so plan the tier before you create the namespace.

## Key Topics

- [Message sessions](https://learn.microsoft.com/azure/service-bus-messaging/message-sessions)
- [Azure Service Bus premium messaging tier](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-premium-messaging)
- [ServiceBusSessionProcessor class](https://learn.microsoft.com/dotnet/api/azure.messaging.servicebus.servicebussessionprocessor?view=azure-dotnet)
- [Troubleshooting guide for Azure Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-troubleshooting-guide)
- [Overview of Service Bus dead-letter queues](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dead-letter-queues)
