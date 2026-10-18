---
minutes: 10
---

# Event Grid Namespaces: Pull Delivery

Not every partner can open an inbound HTTPS port. A regional carrier might run its own scheduler, or its security team might refuse webhooks. Event Grid namespaces let the partner's application pull `ParcelDelivered` events when it is ready, with queue-like semantics and explicit acknowledgment.

This topic introduces namespaces and namespace topics, the five pull operations, the .NET client in the `Azure.Messaging.EventGrid.Namespaces` package, and the push path to Event Hubs. It ends with the MQTT broker, so the learner knows where that feature fits in a later class.

## Namespaces and Namespace Topics

An Event Grid namespace is a container for topics, clients, client groups, topic spaces, and permission bindings. A namespace topic receives CloudEvents 1.0 events from publishers. Each namespace topic has event subscriptions, and each subscription selects a delivery mode: `queue` for pull, or `push` for delivery to a destination such as Event Hubs.

The quota page sets the boundaries that matter for this design. A namespace topic keeps events for at most 7 days, a topic accepts up to 500 subscriptions, and a single event can be at most 1 MB. The ingress and egress limits are set per TU, so measure the expected throughput before you size the partner traffic.

| Setting | Limit |
| --- | --- |
| Maximum event retention on a namespace topic | 7 days |
| Subscriptions per topic | 500 |
| Maximum event size | 1 MB |
| Events per receive request | 100 maximum |

## Pull Delivery and Lock Tokens

With pull delivery, the consumer controls the rate and timing of consumption. Each received event carries a lock token, which identifies the event and controls its state. The five operations work on those tokens.

| Operation | .NET method | Effect |
| --- | --- | --- |
| Receive | `ReceiveAsync` | Reads up to 100 events and locks them for the consumer. |
| Acknowledge | `AcknowledgeAsync` | Deletes the event from the subscription, so no consumer receives it again. |
| Release | `ReleaseAsync` | Makes the event available again, immediately or after a delay. |
| Reject | `RejectAsync` | Dead-letters the event when a dead-letter destination exists, or drops it otherwise. |
| Renew lock | `RenewLocksAsync` | Extends the lock while the consumer still works on the event. |

A receive call waits up to 60 seconds by default, within a range of 10 to 120 seconds. A release delay must be 0, 10, 60, 600, or 3600 seconds.

A lock can be renewed within one hour of the moment the event was first received, and that one-hour window doesn't cap the total lock time, because each renewal can extend it again. Acknowledgment applies only to the subscription that received the event, so other subscriptions keep their own copy.

## The .NET Client for Namespaces

Install the `Azure.Messaging.EventGrid.Namespaces` package, version 1.1.0 at the time of writing. It contains `EventGridSenderClient` for publishing and `EventGridReceiverClient` for pulling. Both take the namespace endpoint and the topic name, and the receiver also takes the subscription name.

```csharp
using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid.Namespaces;

var endpoint = new Uri(Environment.GetEnvironmentVariable("EVENTGRID_NAMESPACE_ENDPOINT"));
var key = new AzureKeyCredential(Environment.GetEnvironmentVariable("EVENTGRID_NAMESPACE_KEY"));

var sender = new EventGridSenderClient(endpoint, "parcel-events", key);
await sender.SendAsync(new CloudEvent("/label-service", "ParcelDelivered", new { ParcelId = "PKG-10482" }));

var receiver = new EventGridReceiverClient(endpoint, "parcel-events", "partner-delivery", key);
var received = await receiver.ReceiveAsync(maxEvents: 10, maxWaitTime: TimeSpan.FromSeconds(30));

var toAcknowledge = new List<string>();
var toRelease = new List<string>();
var toReject = new List<string>();

foreach (ReceiveDetails detail in received.Value.Details)
{
    ParcelDelivered parcel = detail.Event.Data.ToObjectFromJson<ParcelDelivered>();

    if (string.IsNullOrWhiteSpace(parcel.ParcelId))
    {
        toReject.Add(detail.BrokerProperties.LockToken);
    }
    else if (parcel.DeliveredAt is null)
    {
        toRelease.Add(detail.BrokerProperties.LockToken);
    }
    else
    {
        toAcknowledge.Add(detail.BrokerProperties.LockToken);
    }
}

await receiver.AcknowledgeAsync(toAcknowledge);
await receiver.ReleaseAsync(toRelease, ReleaseDelay.OneMinute);
await receiver.RejectAsync(toReject);

public record ParcelDelivered(string ParcelId, DateTimeOffset? DeliveredAt);
```

The receive result exposes its events through `Value.Details`. Each `ReceiveDetails` item holds the CloudEvent in `Event` and the lock token in `BrokerProperties`. The sample releases the events that it can't process yet with a one-minute delay, using `ReleaseDelay.OneMinute`.

## Partners Pulling ParcelDelivered

Each partner gets its own event subscription on the same namespace topic, and each subscription keeps its own state for every event. A slow partner delays only its own subscription. The label-service publishes once, and every partner pulls at its own pace.

Publishers need the EventGrid Data Sender role on the namespace when they authenticate with Microsoft Entra ID, because that role includes the permission to send events. A partner can also use an access key for a namespace, but Entra ID roles give each partner an identity that you can revoke on its own. The partner's application runs the receive loop, and it stops the loop only after it has acknowledged, released or rejected every lock token it holds.

## Push Delivery to Event Hubs and the MQTT Broker

Push delivery is the other delivery mode for a namespace subscription. Event Grid sends the events to a destination that the subscription configures. Event Hubs is the only destination that namespace push delivery supports today.

A push subscription has a maximum delivery count from 1 to 10, with a default of 10, and a retention period in ISO 8601 format with a minimum of `PT1M`. Use this path when the analytics team needs the same `ParcelDelivered` stream in an Event Hub, for example to archive it with Capture.

The namespace also includes an MQTT broker, which accepts MQTT connections from devices and can route those messages to a custom topic. This module doesn't use the broker and mentions it only as a pointer for later work.

## Key Topics

- [Pull delivery with HTTP](https://learn.microsoft.com/azure/event-grid/pull-delivery-overview)
- [Azure Event Grid namespace concepts](https://learn.microsoft.com/azure/event-grid/concepts-event-grid-namespaces)
- [Message push delivery and retry with namespace topics](https://learn.microsoft.com/azure/event-grid/namespace-delivery-retry)
- [Azure Event Grid - subscriber operations](https://learn.microsoft.com/azure/event-grid/subscriber-operations)
- [Send and receive messages with an Azure Event Grid namespace topic (.NET)](https://learn.microsoft.com/azure/event-grid/event-grid-dotnet-get-started-pull-delivery)
