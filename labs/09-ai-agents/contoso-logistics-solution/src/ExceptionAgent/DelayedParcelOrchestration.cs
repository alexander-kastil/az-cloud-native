using Microsoft.DurableTask;

public sealed record DelayedParcelInput(string EventId, Guid ShipmentId, string ParcelId);

public sealed class DelayedParcelOrchestration : TaskOrchestrator<DelayedParcelInput, string>
{
    public override async Task<string> RunAsync(TaskOrchestrationContext context, DelayedParcelInput input)
    {
        var options = TaskOptions.FromRetryPolicy(new RetryPolicy(
            maxNumberOfAttempts: 5,
            firstRetryInterval: TimeSpan.FromSeconds(2),
            backoffCoefficient: 2,
            maxRetryInterval: TimeSpan.FromSeconds(60)));

        var processed = await context.CallActivityAsync<string?>(nameof(ReadProcessedDraft), input, options);
        if (processed is not null)
        {
            return processed;
        }

        var draft = await context.CallActivityAsync<string>(nameof(DraftCustomerMessage), input, options);
        await context.CallActivityAsync<bool>(nameof(SaveRerouteProposal), new SaveRerouteProposalInput(input, draft), options);
        await context.CallActivityAsync<bool>(nameof(PublishRerouteRequested), input, options);

        return draft;
    }
}
