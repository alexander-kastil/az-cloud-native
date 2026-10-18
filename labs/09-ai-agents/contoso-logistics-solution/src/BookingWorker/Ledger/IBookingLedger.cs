namespace BookingWorker.Ledger;

public interface IBookingLedger
{
    Task AppendAsync(string parcelId, string activity);

    Task<IReadOnlyList<string>> ReadActivitiesAsync(string parcelId);
}
