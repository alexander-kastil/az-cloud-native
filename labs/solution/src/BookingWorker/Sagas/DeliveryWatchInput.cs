namespace BookingWorker.Sagas;

public sealed record DeliveryWatchInput(string ParcelId, int WindowSeconds);
