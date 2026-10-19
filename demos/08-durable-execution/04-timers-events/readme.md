# Durable Timers: External Events & Approvals

> **_NOTE:_** Duration: about 8 minutes.

Every Grey parcel has a promised delivery window. When the window closes without a delivery, the company must tell the customer and offer a reroute, and the customer decides whether to accept it. The orchestration for that parcel therefore waits for three things: a scan that confirms delivery, a timer that marks the deadline, and a customer decision that may never arrive.

This topic shows how a durable orchestration waits for time and for people without holding a worker. It covers durable timers, the `WaitForExternalEvent` call, racing a timer against an event, and the client call that delivers the event.

## Durable Timers

A durable timer is a task that completes at a future time. The orchestrator awaits it with `CreateTimer`, and the orchestration sleeps until the due time. The timer message is stored with the instance, so the wait survives a restart, and the scheduler wakes the instance when the timer fires.

```csharp
public class DeliveryWatchOrchestrator : TaskOrchestrator<string, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, string parcelId)
    {
        await context.CreateTimer(TimeSpan.FromHours(4), CancellationToken.None);
        await context.CallActivityAsync("RaiseDeliveryDelayed", parcelId);
        return "delayed";
    }
}
```

The Durable Task SDK takes a `TimeSpan` or a due date. The four hour span is an example. In production, pass the promised window from the booking as input and compute the due time from `context.CurrentUtcDateTime`, never from `DateTime.UtcNow`.

While a timer is pending, the orchestration keeps processing other events, so a timer never blocks an event from arriving. In Durable Functions, .NET and Java apps support timers of any length, while JavaScript and Python apps limit a single timer to six days. Cancel a timer that you no longer need, because the orchestration does not complete while a timer is still outstanding.

## Waiting for External Events

An external event is a named message that a client sends to a running instance. The orchestrator waits for it with `WaitForExternalEvent<T>`, and the event payload must serialize to `T`. The event name must match on both sides, so a misspelled name waits forever.

An event that arrives before the orchestrator waits is buffered until the orchestrator asks for it. An event sent to an unknown instance is discarded, so check the instance ID before you raise an event.

Delivery is at least once, so a duplicate event can reach the orchestrator. Put a unique ID in the payload, and check it in the orchestrator when a duplicate would cause harm.

Waiting without a limit is rarely right for a person, so pair the event with a timer. The Learn human interaction pattern follows four steps: notify the person, start a timer, wait for the response, and handle whichever outcome arrives first. The race below does that for the delivery window and for the reroute decision.

```csharp
public class DeliveryDeadlineOrchestrator : TaskOrchestrator<DeliveryWatchInput, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, DeliveryWatchInput input)
    {
        using var cts = new CancellationTokenSource();
        var dueTime = context.CurrentUtcDateTime.AddSeconds(input.WindowSeconds);
        var timer = context.CreateTimer(dueTime, cts.Token);
        var delivered = context.WaitForExternalEvent<ParcelDelivered>("ParcelDelivered", cts.Token);

        var winner = await Task.WhenAny(delivered, timer);
        cts.Cancel();

        if (winner == delivered)
        {
            return "delivered";
        }

        await context.CallActivityAsync<string>(nameof(RaiseDeliveryDelayed), input.ParcelId);

        using var rerouteCts = new CancellationTokenSource();
        var rerouteTimer = context.CreateTimer(context.CurrentUtcDateTime.AddHours(24), rerouteCts.Token);
        var approval = context.WaitForExternalEvent<bool>("RerouteApproved", rerouteCts.Token);

        var rerouteWinner = await Task.WhenAny(approval, rerouteTimer);
        rerouteCts.Cancel();

        if (rerouteWinner == approval)
        {
            await context.CallActivityAsync<string>(nameof(RerouteParcel), input.ParcelId);
            return "rerouted";
        }

        return "reroute declined";
    }
}
```

The `RaiseDeliveryDelayed` activity publishes the `DeliveryDelayed` event through the Dapr `pubsub` component to the `parcel-events` topic. Other services react to that event without knowing about this orchestration. The orchestrator then waits up to 24 hours for the customer, and a timeout returns a declined result.

![Three runs of DeliveryDeadlineOrchestrator: ParcelDelivered beats the window timer, the timer is cancelled and the result is delivered; the window timer fires, DeliveryDelayed is published, RerouteApproved beats the 24 h timer, RerouteParcel runs and the result is rerouted; the 24 h timer runs out and the result is reroute declined, while the worker stays free during every wait](_images/timer-vs-event.svg)

On the Functions Consumption plan, no billing charges accrue while an orchestrator awaits an external event, however long the wait lasts. The Learn human interaction pattern uses the same race between a timer and an event, and it recommends canceling timers that the orchestration no longer needs.

## Raising Events from the Outside

A client delivers an event with `RaiseEventAsync`. The call takes the instance ID, the event name, and the payload. The example below raises the customer's approval from a web handler or a queue consumer.

```csharp
public static Task ApproveReroute(DurableTaskClient client, string instanceId)
{
    return client.RaiseEventAsync(instanceId, "RerouteApproved", true);
}
```

Durable Functions exposes the same operation as an HTTP API, so a customer portal can call it without a client SDK. The URL carries the instance ID and the event name. Both paths enqueue the same event message for the instance, so the orchestrator handles them identically.

```http
POST /runtime/webhooks/durabletask/instances/{instanceId}/raiseEvent/RerouteApproved?code={function-key}
Content-Type: application/json

true
```

Treat the HTTP endpoint as a public surface. Protect it with a function key, and validate the approval before you send the event.

## Key Topics

- [Durable timers](https://learn.microsoft.com/azure/durable-task/common/durable-task-timers)
- [Handle external events in durable orchestrations](https://learn.microsoft.com/azure/durable-task/common/durable-task-external-events)
- [Human interaction pattern](https://learn.microsoft.com/azure/durable-task/common/durable-task-human-interaction)
- [Durable orchestrations](https://learn.microsoft.com/azure/durable-task/common/durable-task-orchestrations)
