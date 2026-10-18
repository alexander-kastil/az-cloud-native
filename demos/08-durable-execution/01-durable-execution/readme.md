# Durable Execution: Replay & Determinism

> **_NOTE:_** Duration: about 8 minutes.

A parcel booking touches capacity, payment, and label services. If the process that drives the booking crashes after the payment step, a plain function call loses that progress, and a retry may charge the customer twice. Durable execution stores the result of each completed step, so the code resumes after the last step it finished.

This topic explains how that works in the Durable Task programming model. You learn how the runtime rebuilds state from an event history, which code rules keep replay safe, and where side effects belong. The same rules apply to Durable Functions and to the Durable Task SDKs.

## How Replay Rebuilds State

Each orchestration instance has an event history. The runtime appends an event for every scheduled activity, completed activity, timer, and external event. The Durable Task Scheduler dashboard shows these events under names such as `TaskScheduled`, `TaskCompleted`, `TimerCreated`, and `EventRaised`.

When a worker picks up an instance, it runs the orchestrator code from the first line. Each `await` on a durable task receives its recorded result, so the code moves forward without repeating finished work. The first `await` without a recorded result is the point where the instance waits.

```csharp
public class ParcelBookingOrchestrator : TaskOrchestrator<string, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, string parcelId)
    {
        string reservation = await context.CallActivityAsync<string>("ReserveCapacity", parcelId);
        string payment = await context.CallActivityAsync<string>("CapturePayment", parcelId);
        return await context.CallActivityAsync<string>("PrintLabel", parcelId);
    }
}
```

Suppose the worker crashes after `CapturePayment` completes. The next worker replays the method, receives the stored results for both earlier calls, and schedules `PrintLabel` because no result exists yet. The customer pays once, and the label prints on the second attempt.

The approach is event sourcing, as the Durable Functions docs describe it. Local variables survive a restart because the runtime rebuilds them from the history, and the code never writes them to a database.

Between checkpoints, the runtime removes the orchestrator from memory, so a long wait for a customer decision holds no worker memory. The history also explains a stuck instance. The last completed event shows which step is blocked, and the dashboard displays that sequence for each instance.

## Determinism Rules

Replay runs the orchestrator code again, so every run must make the same decisions. Code that reads the clock, generates random values, or calls the network produces a different result on each replay. Static variables and environment variables can change between runs, so they carry the same risk.

The test for any call in an orchestrator is simple: does it return the same value on every replay? Arithmetic and string formatting pass that test. The table lists common calls that fail it, along with the replacement for each one.

| Do not call in an orchestrator | Replacement |
| --- | --- |
| `DateTime.UtcNow`, `DateTime.Now` | `context.CurrentUtcDateTime` |
| `Guid.NewGuid()` | `context.NewGuid()` |
| `Task.Delay`, `Thread.Sleep` | `context.CreateTimer` |
| `HttpClient`, storage SDK clients | An activity |
| `Random` | An activity that returns the value |
| `Environment.GetEnvironmentVariable` | An input value or an activity result |
| `Task.Run`, `Thread.Start` | Activities |
| `ILogger` | `context.CreateReplaySafeLogger` |

The Durable Task Roslyn Analyzer reports most of these cases as warnings, with rule IDs DURABLE0001 through DURABLE0011. The same rule set flags an unbounded `while (true)` loop that never calls `ContinueAsNew`, because the history would grow without limit.

Run the analyzer on every orchestrator project before you merge a change to a workflow.

## Activities Hold the Side Effects

Activities do the work that changes the outside world. They call services, write to databases, and print labels. The runtime guarantees that each called activity runs at least once, so a retry after a crash can call the same activity twice.

Each activity must be idempotent, which means a repeated call with the same input produces the same outcome. The orchestrator can start several activities at once and await them together with `Task.WhenAll`. The runtime runs them in parallel on any available worker, and the orchestrator resumes when all of them complete.

Activity inputs and outputs are serialized, and each one is limited to 1 MB. Keep large content out of the history by passing a blob name, and let the activity load the photo itself.

The same model fits long-running processes and AI agents. An orchestration can wait for days for a customer decision. Each model call in an agent can run as an activity whose result the history keeps.

## Key Topics

- [What is Durable Task?](https://learn.microsoft.com/azure/durable-task/common/what-is-durable-task)
- [Durable orchestrations](https://learn.microsoft.com/azure/durable-task/common/durable-task-orchestrations)
- [Orchestrator function code constraints](https://learn.microsoft.com/azure/durable-task/common/durable-task-code-constraints)
- [Durable Task Roslyn Analyzer for C# orchestrations](https://learn.microsoft.com/azure/durable-task/durable-functions/durable-functions-roslyn-analyzer)
