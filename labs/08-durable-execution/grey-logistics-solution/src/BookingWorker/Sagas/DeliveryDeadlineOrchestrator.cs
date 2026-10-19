using BookingWorker.Activities;
using Grey.Contracts;
using Microsoft.DurableTask;

namespace BookingWorker.Sagas;

public sealed class DeliveryDeadlineOrchestrator : TaskOrchestrator<DeliveryWatchInput, string>
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
