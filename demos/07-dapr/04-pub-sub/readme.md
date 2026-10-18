---
minutes: 10
---

# Pub/Sub: Swap Brokers via Components

Every scan of a parcel at a hub produces a `ParcelScanned` event, and several services need it: `tracking-service` updates the read model, and `notification-service` tells the customer where the parcel is. The scanner should not know who listens, and it should not wait for them. A broker in the middle makes that possible, but the Azure SDK ties the code to one broker.

This topic publishes and subscribes through the Dapr pub/sub API in ASP.NET Core. It covers the `Topic` attribute, CloudEvents wrapping, the Azure Service Bus topics component, swapping to Redis or Kafka for local work, dead-letter topics, and what Dapr hides from the native Service Bus SDK.

## Publish with DaprClient

The publisher calls `PublishEventAsync` with the name of the pub/sub component, the topic name and the payload. The component name comes from the component file, so the code never contains a connection string or a broker type.

```csharp
using Dapr.Client;

public record ParcelScannedEvent(string ParcelId, string ScanType, DateTimeOffset ScannedAt);

var client = new DaprClientBuilder().Build();

var scan = new ParcelScannedEvent("PKG-778", "AtHub", DateTimeOffset.UtcNow);

await client.PublishEventAsync("pubsub", "parcel-scans", scan);
```

Some brokers accept per-message metadata. The Service Bus component reads a `SessionId` value from the publish metadata, which the next topics use for ordered processing. The overload takes a `Dictionary<string, string>` after the payload.

```csharp
using Dapr.Client;

var metadata = new Dictionary<string, string> { ["SessionId"] = "PKG-778" };

await client.PublishEventAsync("pubsub", "parcel-scans", scan, metadata);
```

## Subscribe with the Topic Attribute

The subscriber declares the subscription on a controller action with the `Topic` attribute from the `Dapr` namespace. The constructor takes the pub/sub component name and the topic name. The sidecar delivers each matching message to the action as a POST request.

```csharp
using Dapr;
using Microsoft.AspNetCore.Mvc;

public record ParcelScannedEvent(string ParcelId, string ScanType, DateTimeOffset ScannedAt);

[ApiController]
public sealed class ScansController : ControllerBase
{
    [Topic("pubsub", "parcel-scans")]
    [HttpPost("scans")]
    public IActionResult OnParcelScanned(ParcelScannedEvent scan)
    {
        return Ok();
    }
}
```

The application registers the Dapr pieces in `Program.cs`. `UseCloudEvents` unwraps the envelope, and `MapSubscribeHandler` exposes the endpoint that the sidecar calls at startup to read the subscriptions.

```csharp
using Dapr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddDapr();

var app = builder.Build();

app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();
```

Dapr wraps each payload in a CloudEvents 1.0 envelope, and it copies the `Content-Type` header into the `datacontenttype` attribute. Your handler receives the data, and the envelope carries the source, type and id that the broker needs for tracing and deduplication.

## Declarative Subscriptions and Dead-Letter Topics

A subscription can also be declared in a file. The Dapr documentation for version 1.18 uses the `dapr.io/v2alpha1` form of the `Subscription` kind, and the file below routes the topic to a route and sends failed messages to a dead-letter topic.

```yaml
apiVersion: dapr.io/v2alpha1
kind: Subscription
metadata:
  name: parcel-scans
spec:
  topic: parcel-scans
  routes:
    default: /scans
  pubsubname: pubsub
  deadLetterTopic: poison-scans
scopes:
- tracking-service
```

The `Topic` overload that takes a dead-letter topic sets the same value in code:

```csharp
using Dapr;
using Microsoft.AspNetCore.Mvc;

public record ParcelScannedEvent(string ParcelId, string ScanType, DateTimeOffset ScannedAt);

public sealed class ScansController : ControllerBase
{
    [Topic("pubsub", "parcel-scans", "poison-scans", false)]
    [HttpPost("scans")]
    public IActionResult OnParcelScanned(ParcelScannedEvent scan) => Ok();
}
```

A message that still fails after the retries lands on `poison-scans`. The Service Bus broker has its own limit: a message that exceeds the default `MaxDeliveryCount` of 10 moves to the broker dead-letter queue with the reason `MaxDeliveryCountExceeded`. Dapr retries are controlled by the resiliency spec, which [topic 5](../05-resiliency/readme.md) covers.

## The Service Bus Topics Component

The `pubsub.azure.servicebus.topics` component connects Dapr to a Service Bus namespace. It is a Tier 1 component in Container Apps. The file below uses a connection string, which the component requires unless you switch to Microsoft Entra ID through `namespaceName`.

```yaml
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: pubsub
spec:
  type: pubsub.azure.servicebus.topics
  version: v1
  metadata:
  - name: connectionString
    value: <service-bus-connection-string>
  - name: maxDeliveryCount
    value: "10"
```

The component settings apply to every topic that uses the component. `maxDeliveryCount` and the other subscription-creation settings take effect only when Dapr creates the subscription, so changing them later does not alter an existing subscription.

## Swap Brokers Without Code Changes

The publisher and subscriber code in this topic names only the component `pubsub` and the topic `parcel-scans`. To change the broker, replace the component file and keep the code. A local run can use Redis Streams, because the default self-hosted initialization already runs a Redis container.

| Broker | Component type | Container Apps tier |
| --- | --- | --- |
| Azure Service Bus topics | `pubsub.azure.servicebus.topics` | Tier 1 |
| Azure Event Hubs | `pubsub.azure.eventhubs` | Tier 1 |
| Redis Streams | `pubsub.redis` | Tier 2 |
| Apache Kafka | `pubsub.kafka` | Tier 2 |

Tier 2 components are supported with a lower investigation priority, so a production system on Container Apps should prefer Tier 1 where it fits. The swap changes `spec.type` and the metadata for that broker. The publisher, the subscriber and the subscription file stay as they are.

## What Dapr Hides from the Native SDK

The native `Azure.Messaging.ServiceBus` client exposes processors, session receivers, lock handling and settlement, and dead-letter receivers. Dapr hides most of that behind the sidecar, and it exposes a smaller set of settings through the component metadata. The Service Bus component documents `lockRenewalInSec`, `maxConcurrentHandlers` and `maxActiveMessages` for concurrency, and it renews message locks on your behalf.

Sessions are the clearest example of the gap.

The component documents `requireSessions`, `sessionIdleTimeoutInSec` and `maxConcurrentSessions` as subscription metadata, and it passes the session id through publish metadata. The Dapr page does not document session state or the session receiver, so use the native SDK when the processing logic depends on them. The [sessions topic](../../04-service-bus/04-sessions/readme.md) covers ordered processing for the scan flow.

## Key Topics

- [Publish and subscribe overview](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview)
- [Dead-letter topics](https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-deadletter/)
- [Azure Service Bus Topics component](https://docs.dapr.io/reference/components-reference/supported-pubsub/setup-azure-servicebus-topics)
- [Dead-letter queues in Azure Service Bus](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-dead-letter-queues)
