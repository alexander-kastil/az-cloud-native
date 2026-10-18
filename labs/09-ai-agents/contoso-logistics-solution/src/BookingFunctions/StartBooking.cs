using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;

namespace BookingFunctions;

public static class StartBooking
{
    [Function(nameof(StartBooking))]
    public static async Task<HttpResponseData> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "bookings/{parcelId}")] HttpRequestData request,
        string parcelId,
        [DurableClient] DurableTaskClient client)
    {
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Sagas.BookingSagaOrchestrator), parcelId);
        return await client.CreateCheckStatusResponseAsync(request, instanceId);
    }
}
