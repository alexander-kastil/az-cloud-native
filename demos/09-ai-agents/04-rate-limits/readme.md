---
minutes: 10
---

# Rate Limits: Priority Queues & KEDA

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

| Layout | How it orders work | Starvation risk |
| --- | --- | --- |
| One queue with a priority value | Consumers take the highest priority message first | Depends on the broker and the consumer logic |
| Several queues with one consumer pool | Consumers drain the high queue before the low queue | High: low-priority messages can wait indefinitely |
| Several queues with one consumer pool each | Each pool drains its own queue at its own pace | Low: each pool has its own capacity |

Give each priority its own queue and its own scaling rule. The urgent pool keeps capacity even when the bulk queue holds a million messages.

## KEDA Scaling Bounded by Quota

Azure Container Apps scales the agent with KEDA rules. A Service Bus rule scales on the queue depth, and maxReplicas is the ceiling. The following scale block caps the urgent pool at 10 replicas.

```json
{
  "minReplicas": 0,
  "maxReplicas": 10,
  "rules": [
    {
      "name": "urgent-exceptions",
      "custom": {
        "type": "azure-servicebus",
        "metadata": {
          "queueName": "exceptions-urgent",
          "namespace": "<service-bus-namespace>",
          "messageCount": "5"
        }
      }
    }
  ]
}
```

KEDA polls the queue about every 30 seconds and computes the desired replica count from the queue length and messageCount. The maxReplicas value is a fixed cap, so the quota enters the design there. Work out the calls per minute the quota allows, and the calls that must run at once to sustain that rate, then keep replicas multiplied by concurrency per replica at or below that number.

Here is the arithmetic for a 120,000 TPM deployment. Each call uses about 2,000 tokens, so the deployment allows 60 calls per minute, or one call per second. If a call takes 10 seconds, ten calls must run at the same time to keep one call per second flowing.

Set the replica count and the per-replica concurrency so that their product stays at or below ten in-flight calls.

## Back Pressure Through the Queue

The queue absorbs the burst. Scanners and telemetry keep publishing exceptions at their own pace, and the agent drains the queue at the rate the quota allows. Producers never see the 429, and the backlog shows up as queue depth on the dashboard, where operators can see it.

Failures keep their own path. When the agent's own retries run out, it abandons the message, and the broker delivers the message again. After the maximum delivery count, the broker moves it to the dead-letter queue, where Module 4 covers the handling.

The gateway option moves the budget check in front of the agent. Azure API Management provides the llm-token-limit policy, which limits tokens per minute for each counter key, such as a subscription, and returns 429 when the limit is reached. The same policy returns 403 when a longer token quota runs out, so the gateway can separate a short burst from an exhausted budget.

## Key Topics

- [Manage Azure OpenAI in Microsoft Foundry Models quota](https://learn.microsoft.com/azure/foundry/openai/how-to/quota)
- [Set scaling rules in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/scale-app)
- [Priority Queue pattern](https://learn.microsoft.com/azure/architecture/patterns/priority-queue)
- [Limit large language model API token usage](https://learn.microsoft.com/azure/api-management/llm-token-limit-policy)
