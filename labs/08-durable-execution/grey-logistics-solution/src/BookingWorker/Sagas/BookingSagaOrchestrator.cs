using BookingWorker.Activities;
using Microsoft.DurableTask;

namespace BookingWorker.Sagas;

public sealed class BookingSagaOrchestrator : TaskOrchestrator<string, string>
{
    private static readonly TaskOptions PrintLabelOptions = new TaskOptions(
        new TaskRetryOptions(new RetryPolicy(maxNumberOfAttempts: 3, firstRetryInterval: TimeSpan.FromSeconds(5))));

    public override async Task<string> RunAsync(TaskOrchestrationContext context, string parcelId)
    {
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
