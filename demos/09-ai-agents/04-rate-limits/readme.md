# Rate Limits: Priority Queues & KEDA

> **_NOTE:_** Duration: about 11 minutes.

A van breakdown can delay 300 parcels in ten minutes, and every delay raises an exception that the agent should handle. The Microsoft Foundry deployment behind the agent has a fixed budget of tokens and requests per minute. Without a plan, the burst exhausts the budget, and urgent customer notices wait behind bulk work.

This topic explains how model quotas push back with 429 responses, how priority queues keep urgent exceptions ahead of bulk work, how KEDA caps the number of replicas to the quota, and how the queue absorbs the rest. The API Management token limit policy appears at the end as the gateway option.

## How Model Quotas Push Back

Each deployment receives a tokens-per-minute (TPM) allocation, and the requests-per-minute (RPM) limit follows from it. A request that exceeds either limit receives a 429 response. Requests that fail still count against the budget, so retries during a burst make the throttling worse unless they back off.

The token estimate for each request covers the prompt and the max_tokens value. The rate limit does not wait for the model to report its real output size.

A request with a large max_tokens value reserves budget that the model may never spend, so set max_tokens to the size of a customer notice. The RPM limit is checked over short windows, so a burst of requests can receive 429 responses even when the minute total is still below the limit.

When a 429 arrives, the response includes retry-after-ms and retry-after headers. The Azure OpenAI SDKs retry 429 responses by default and respect those headers. The retry count is a client setting, and the Foundry quota guidance recommends exponential backoff with jitter and a fixed maximum.

## Priority Queues for Urgent and Bulk Work

Two kinds of work reach the agent. Urgent exceptions, such as a parcel with a medical or time-critical label, need a draft within minutes. Bulk exceptions, such as a weekly sweep of delayed parcels for analytics, can wait an hour.

The Priority Queue pattern separates them so the urgent work reaches the model first.

The pattern offers three layouts, and the layout decides whether low-priority work can starve.

![Three queue layouts side by side, each fed by booking-worker: one queue with a priority value feeding one consumer pool; an urgent and a bulk queue feeding one pool that is always busy with urgent while bulk piles up, marked as the layout the lab uses; and an urgent and a bulk queue with one pool each](_images/priority-queues.svg)

| Layout | How it orders work | Starvation risk |
| --- | --- | --- |
| One queue with a priority value | Consumers take the highest priority message first | Depends on the broker and the consumer logic |
| Several queues with one consumer pool | Consumers drain the high queue before the low queue | High: low-priority messages can wait indefinitely |
| Several queues with one consumer pool each | Each pool drains its own queue at its own pace | Low: each pool has its own capacity |

Give each priority its own queue and its own scaling rule. The lab's replicas form one pool, so a bulk backlog can still scale every replica, and once the dispatcher starts a bulk orchestration, its model call competes with urgent calls for the same activity slots.

## KEDA Scaling Bounded by Quota

Azure Container Apps scales the agent with KEDA rules. A Service Bus rule scales on the queue depth, and maxReplicas is the ceiling. The scale block in the lab caps the agent at three replicas, with one rule for each queue.

```json
{
  "scale": {
    "minReplicas": 0,
    "maxReplicas": 3,
    "rules": [
      {
        "name": "urgent-exceptions",
        "custom": {
          "type": "azure-servicebus",
          "metadata": {
            "namespace": "<service-bus-namespace>",
            "queueName": "exceptions-urgent",
            "messageCount": "1"
          }
        }
      },
      {
        "name": "bulk-exceptions",
        "custom": {
          "type": "azure-servicebus",
          "metadata": {
            "namespace": "<service-bus-namespace>",
            "queueName": "exceptions-bulk",
            "messageCount": "1"
          }
        }
      }
    ]
  }
}
```

KEDA polls the queue about every 30 seconds and computes the desired replica count from the queue length and messageCount. The maxReplicas value is a fixed cap, so the quota enters the design there. Work out the calls per minute the quota allows, and the calls that must run at once to sustain that rate, then keep replicas multiplied by concurrency per replica at or below that number.

Here is the arithmetic for the lab's chat deployment, which has 30,000 TPM. Each call uses about 2,000 tokens, so the deployment allows 15 calls per minute, or one call every four seconds. If a call takes 10 seconds, 2.5 calls must run at the same time to sustain that rate, so the replica cap is three.

Set the replica count and the per-replica concurrency so that their product stays at or below three in-flight calls. The lab sets MaximumConcurrentActivityWorkItems to 1 in its Program.cs, so each replica holds one call in flight.

## Back Pressure Through the Queue

The queue absorbs the burst. booking-worker keeps publishing DeliveryDelayed at its own pace, and the agent drains the queue at the rate the quota allows. Producers never see the 429, and the backlog shows up as queue depth on the dashboard, where operators can see it.

![booking-worker publishes a burst of DeliveryDelayed events, 30 per minute, into exceptions-urgent and exceptions-bulk faster than they drain; KEDA adds exception-agent replicas up to maxReplicas 3, each holds one call in flight, and the chat deployment stays at about 15 calls per minute below its 429 line; the queues drain while booking-worker keeps publishing and never sees a 429](_images/back-pressure.svg)

Failures keep their own path. The dispatcher completes a message only after its orchestration starts, so a failed start leaves the message on the queue and the broker delivers it again. Model failures stay with the durable activity's retry policy. After the maximum delivery count, the broker moves it to the dead-letter queue, where Module 4 covers the handling.

The gateway option moves the budget check in front of the agent. Azure API Management provides the llm-token-limit policy, which limits tokens per minute for each counter key, such as a subscription, and returns 429 when the limit is reached. The same policy returns 403 when a longer token quota runs out, so the gateway can separate a short burst from an exhausted budget.

## Key Topics

- [Manage Azure OpenAI in Microsoft Foundry Models quota](https://learn.microsoft.com/azure/foundry/openai/how-to/quota)
- [Set scaling rules in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/scale-app)
- [Priority Queue pattern](https://learn.microsoft.com/azure/architecture/patterns/priority-queue)
- [Limit large language model API token usage](https://learn.microsoft.com/azure/api-management/llm-token-limit-policy)
