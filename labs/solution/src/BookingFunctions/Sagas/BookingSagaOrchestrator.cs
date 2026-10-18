using BookingFunctions.Activities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace BookingFunctions.Sagas;

public static class BookingSagaOrchestrator
{
    private static readonly TaskOptions PrintLabelOptions = new TaskOptions(
        new TaskRetryOptions(new RetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(5))));

    [Function(nameof(BookingSagaOrchestrator))]
    public static async Task<string> RunAsync([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var parcelId = context.GetInput<string>()!;
        var compensations = new Stack<Func<Task>>();
        try
        {
            await context.CallActivityAsync<string>(nameof(ReserveCapacity), parcelId);
            compensations.Push(() => context.CallActivityAsync<string>(nameof(ReleaseCapacity), parcelId));

            await context.CallActivityAsync<string>(nameof(CapturePayment), parcelId);
            compensations.Push(() => context.CallActivityAsync<string>(nameof(RefundPayment), parcelId));

            await context.CallActivityAsync<string>(nameof(PrintLabel), parcelId, PrintLabelOptions);
            compensations.Push(() => context.CallActivityAsync<string>(nameof(VoidLabel), parcelId));

            return "booked";
        }
        catch (TaskFailedException)
        {
            while (compensations.Count > 0)
            {
                await compensations.Pop()();
            }

            return "compensated";
        }
    }
}
