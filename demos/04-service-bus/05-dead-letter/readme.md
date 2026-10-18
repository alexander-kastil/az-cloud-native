# Dead-Letter Queues: Poison Messages

> **_NOTE:_** Duration: about 9 minutes.

Some scans can't be read. A label scanner that prints a malformed barcode produces a message that no handler can parse, and every redelivery wastes handler time on it. Contoso Logistics moves these messages to the dead-letter queue of the `tracking` subscription, where an operator can inspect them and resubmit a repaired copy.

This topic covers the delivery limit, the system dead-letter reasons, explicit dead-lettering with a reason and description, reading the dead-letter queue, resubmitting messages, and expiry.

## Delivery Limits and System Reasons

Each queue and subscription sets `maxDeliveryCount`, which defaults to 10. Every abandon and every expired lock increases a message's delivery count. When the count exceeds the limit, the broker moves the message to the dead-letter queue with the reason `MaxDeliveryCountExceeded`.

![An unreadable label scan on the tracking subscription is abandoned until DeliveryCount reaches 10 and the broker dead-letters it with MaxDeliveryCountExceeded, while scan-processor dead-letters it on the first delivery with UnreadableLabel once ScanValidator.Validate fails](_images/dead-letter-bounces.svg)

The limit is adjustable, and the behavior is always on.

The broker also dead-letters messages for other reasons. The system sets the reason and description, and your code can read them from the dead-lettered message.

| Reason | Cause |
| --- | --- |
| `MaxDeliveryCountExceeded` | The delivery count passed `maxDeliveryCount` |
| `TTLExpiredException` | The message expired while dead-lettering on expiration was enabled |
| `HeaderSizeExceeded` | The message headers exceeded the size limit |
| `Session ID is null` | A session-enabled entity received a message without a session ID |
| `MaxTransferHopCountExceeded` | Auto-forwarding exceeded four hops |

Most unexpected dead letters trace back to a few causes. A handler that never settles its messages exhausts the delivery count. 

A receiver that closes before it settles leaves the lock to expire, and each expiry counts as a delivery. A message larger than the header limit fails with `HeaderSizeExceeded`, and the Premium tier raises those limits.

Subscriptions can also dead-letter messages when a filter rule throws an error. Set `deadLetteringOnFilterEvaluationExceptions` to capture those messages. Microsoft Learn warns against that setting in production when some messages have no matching subscription, because the dead-letter queue can grow quickly.

When dead-lettering on expiration is enabled, every expiring message moves to the dead-letter queue. Set `deadLetteringOnMessageExpiration` to `true` on the queue or subscription to enable it. Deferred messages don't move to the dead-letter queue when they expire, by design.

Each received message exposes `EnqueuedTime` and `ExpiresAt`, so a handler can compare the time left with the work it needs. A handler that sees too little time left can dead-letter the message with a clear reason before it expires.

## Dead-Lettering Explicitly

Dead-letter a message yourself when you know that it can't succeed. Call `DeadLetterMessageAsync` with a reason and a description, so that operators see why the message failed. Both values are limited to 4,096 characters, and the call works only in `PeekLock` mode.

PeekLock is the default receive mode, so the receiver holds a lock on each message until it settles that message.

Use a short, stable reason code so that dashboards and operators can group failures by cause.

```csharp
using Azure.Identity;
using Azure.Messaging.ServiceBus;

var client = new ServiceBusClient("<NAMESPACE>.servicebus.windows.net", new DefaultAzureCredential());
await using var receiver = client.CreateReceiver("label-scans");

ServiceBusReceivedMessage message = await receiver.ReceiveMessageAsync();
var validScanTypes = new[] { "PickedUp", "AtHub", "OutForDelivery" };

if (!validScanTypes.Contains(message.Subject))
{
    await receiver.DeadLetterMessageAsync(message, "UnreadableLabel", $"Unknown scan type '{message.Subject}'.");
}
else
{
    await receiver.CompleteMessageAsync(message);
}
```

The Contoso Logistics scan processor dead-letters from its Functions trigger instead, through `ServiceBusMessageActions` with the reason `UnreadableLabel` (`src/ScanProcessor/ScanFunctions.cs:20`). The `label-scans` queue in the examples here is illustrative and doesn't exist in the Contoso Logistics topology.

Settle each message before the receiver closes. A settlement that runs after the receiver closes doesn't reach the service, so the message redelivers and eventually dead-letters with `MaxDeliveryCountExceeded`.

A late settlement can also fail with `MessageLockLost`. The service releases a message whose lock expires before settlement, so the message returns to the queue for another receiver. Renew the lock during long work, or shorten the work, so that settlement happens while the lock is still held.

## Reading and Resubmitting the Dead-Letter Queue

Each queue and subscription has its own dead-letter queue, so the dead letters of one subscription never mix with another's. Read the dead-letter queue with a receiver that sets `SubQueue.DeadLetter` in `ServiceBusReceiverOptions`.

The `DeadLetterReason` and `DeadLetterErrorDescription` properties on each message explain the failure. The reason travels with the message, so an operator sees it next to the payload without a separate lookup.

Resubmit a message by sending a repaired copy, and then complete the original in the dead-letter queue. Send the copy first, so that a failed send leaves the original in place.

```csharp
var deadLetterOptions = new ServiceBusReceiverOptions
{
    ReceiveMode = ServiceBusReceiveMode.PeekLock,
    SubQueue = SubQueue.DeadLetter
};

await using var deadLetters = client.CreateReceiver("label-scans", deadLetterOptions);
await using var sender = client.CreateSender("label-scans");

ServiceBusReceivedMessage poisoned = await deadLetters.ReceiveMessageAsync();

var repaired = new ServiceBusMessage(poisoned)
{
    Subject = "PickedUp"
};

await sender.SendMessageAsync(repaired);
await deadLetters.CompleteMessageAsync(poisoned);
```

Repairing a message means changing its content or properties to a valid form. Resubmit only after you have fixed the cause, because a copy that fails again returns to the dead-letter queue and needs another repair.

## Key Topics

- [Overview of Service Bus dead-letter queues](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dead-letter-queues)
- [ServiceBusReceiver.DeadLetterMessageAsync](https://learn.microsoft.com/dotnet/api/azure.messaging.servicebus.servicebusreceiver.deadlettermessageasync?view=azure-dotnet)
- [Service Bus messaging exceptions](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-messaging-exceptions-latest)
- [Message transfers, locks, and settlement](https://learn.microsoft.com/azure/service-bus-messaging/message-transfers-locks-settlement)
