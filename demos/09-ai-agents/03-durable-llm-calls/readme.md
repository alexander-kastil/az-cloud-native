# LLM Calls: Durable Activities

> **_NOTE:_** Duration: about 8 minutes.

The delayed-parcel workflow at Contoso Logistics runs for hours. It waits for a deadline, asks the exception-agent for a draft, publishes a reroute proposal, and then waits for a person to approve it.

A model call takes seconds, can be throttled, and can fail after it has already been billed. The workflow must survive all of that, so the model call needs a durable home.

This topic explains why the model call belongs in an activity, outside the orchestrator, how replay stays safe, how retries handle throttling, and how the reroute approval arrives as an external event. The orchestration mechanics come from Module 8, so this topic links to those topics for the details.

## Why the Model Call Belongs in an Activity

An orchestrator replays its history each time it resumes. Every call it makes must return the same result on replay, or the orchestration no longer matches its recorded history. A model returns different text for the same prompt, so a model call in the orchestrator breaks determinism on the first replay.

An activity runs once for each attempt, and the runtime records its output in the history. On replay, the orchestrator reads the recorded draft and never calls the model again. The activity is the only place where the non-deterministic work happens, and the history keeps the result.

Call the activity with a retry policy and the input that identifies the delayed event. The input carries the parcel id and the event id, so the activity can key its work and its stored result to the event.

```csharp
var retry = new TaskOptions(new TaskRetryOptions(new RetryPolicy(
    maxNumberOfAttempts: 5,
    firstRetryInterval: TimeSpan.FromSeconds(2),
    backoffCoefficient: 2,
    maxRetryInterval: TimeSpan.FromSeconds(60))));

var draft = await context.CallActivityAsync<string>(
    "DraftCustomerMessage",
    delayed,
    retry);
```

The maxNumberOfAttempts value counts the first call too, so 5 allows four retries. Set the limit deliberately, because an unbounded retry keeps the orchestration in the running state with no clear failure.

## Retries on Throttling and Replay Safety

A 429 from the Foundry deployment means the tokens-per-minute or requests-per-minute budget is spent. The response carries retry-after-ms and retry-after headers that say how long to wait. The retry policy supplies the backoff schedule, and the next attempt should not start before the header value has passed.

Not every failure deserves a retry. The table shows how the activity should treat each response, following the throttling guidance in the Azure Well-Architected Framework.

| Response | Retry in the activity | Reason |
| --- | --- | --- |
| 429 with retry-after-ms | Yes | Capacity returns after the indicated delay |
| 408 from a provisioned deployment | Yes | The request timed out in the deployment queue |
| 5xx | Yes | Transient service failure |
| 400 with code content_filter | No | The same content fails again, so the activity records the failure and stops |

Retries also raise a replay question. An activity can call the model, receive the answer, and fail before it reports success. The runtime then runs the activity again, and the model answers again.

The activity therefore writes its result under the event id. It returns the stored result when that key already exists.

## Waiting for the Reroute Approval

After the draft succeeds, the orchestrator publishes the RerouteRequested event and waits for the answer. The answer arrives as the external event RerouteApproved, or the wait ends when the deadline timer fires. A waiting orchestration has no model call in flight, so a parcel can wait a day for a person without tying up a call.

The external event pattern, the timer race, and the approval handling are taught in [Durable Timers: External Events & Approvals](../../08-durable-execution/04-timers-events/readme.md). This module only supplies the model step that comes before that wait.

Durable agents are the deeper option when you want the agent to own its workflow state. The Microsoft Agent Framework supports them, and the pro-code-agents class covers their design and hosting. This module keeps the orchestration in code, so the model stays one replaceable step.

## Key Topics

- [Handle errors and retries in orchestrations](https://learn.microsoft.com/azure/durable-task/common/durable-task-error-handling)
- [Migrate Durable Functions from in-process to the isolated worker model](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-migrate)
- [Throttling design guide, respect backpressure](https://learn.microsoft.com/azure/well-architected/design-guides/throttling)
- [Manage Azure OpenAI in Microsoft Foundry Models quota](https://learn.microsoft.com/azure/foundry/openai/how-to/quota)
