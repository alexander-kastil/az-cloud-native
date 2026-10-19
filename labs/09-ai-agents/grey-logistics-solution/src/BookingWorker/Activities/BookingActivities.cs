using BookingWorker.Ledger;
using Grey.Contracts;
using Dapr.Client;
using Microsoft.DurableTask;

namespace BookingWorker.Activities;

public abstract class LedgerActivity(IBookingLedger ledger) : TaskActivity<string, string>
{
    protected abstract bool Delayed { get; }

    protected virtual bool Idempotent => false;

    public override async Task<string> RunAsync(TaskActivityContext context, string parcelId)
    {
        var name = GetType().Name;
        if (Idempotent && (await ledger.ReadActivitiesAsync(parcelId)).Contains(name))
        {
            return name;
        }

        if (parcelId.Contains($"FAIL-{name}", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"{name} failed for {parcelId}");
        }

        if (Delayed && parcelId.Contains("SLOW", StringComparison.Ordinal))
        {
            await Task.Delay(TimeSpan.FromSeconds(3));
        }

        await ledger.AppendAsync(parcelId, name);
        return name;
    }
}

public abstract class ForwardActivity(IBookingLedger ledger) : LedgerActivity(ledger)
{
    protected override bool Delayed => true;
}

public abstract class CompensationActivity(IBookingLedger ledger) : LedgerActivity(ledger)
{
    protected override bool Delayed => false;
}

public sealed class ReserveCapacity(IBookingLedger ledger) : ForwardActivity(ledger);

public sealed class CapturePayment(IBookingLedger ledger) : ForwardActivity(ledger)
{
    protected override bool Idempotent => true;
}

public sealed class PrintLabel(IBookingLedger ledger) : ForwardActivity(ledger)
{
    protected override bool Idempotent => true;
}

public sealed class RerouteParcel(IBookingLedger ledger) : ForwardActivity(ledger);

public sealed class ReleaseCapacity(IBookingLedger ledger) : CompensationActivity(ledger);

public sealed class RefundPayment(IBookingLedger ledger) : CompensationActivity(ledger);

public sealed class VoidLabel(IBookingLedger ledger) : CompensationActivity(ledger);

public sealed class RaiseDeliveryDelayed(IBookingLedger ledger, DaprClient dapr) : TaskActivity<string, string>
{
    public override async Task<string> RunAsync(TaskActivityContext context, string parcelId)
    {
        var delayed = new DeliveryDelayed(Guid.Empty, parcelId, "Delivery window elapsed", DateTimeOffset.UtcNow);
        await dapr.PublishEventAsync("pubsub", "parcel-events", delayed);
        await ledger.AppendAsync(parcelId, nameof(DeliveryDelayed));
        return nameof(DeliveryDelayed);
    }
}
