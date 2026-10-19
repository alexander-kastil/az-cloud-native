using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;

namespace BookingFunctions.Activities;

public static class ReserveCapacity
{
    [Function(nameof(ReserveCapacity))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(ReserveCapacity);
}

public static class CapturePayment
{
    [Function(nameof(CapturePayment))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(CapturePayment);
}

public static class PrintLabel
{
    [Function(nameof(PrintLabel))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(PrintLabel);
}

public static class ReleaseCapacity
{
    [Function(nameof(ReleaseCapacity))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(ReleaseCapacity);
}

public static class RefundPayment
{
    [Function(nameof(RefundPayment))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(RefundPayment);
}

public static class VoidLabel
{
    [Function(nameof(VoidLabel))]
    public static string Run([ActivityTrigger] string parcelId) => nameof(VoidLabel);
}
