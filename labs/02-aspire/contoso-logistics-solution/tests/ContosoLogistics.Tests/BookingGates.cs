using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting.Testing;

namespace ContosoLogistics.Tests;

public class BookingGates
{
    [Fact(Timeout = 600_000)]
    [Trait("Gate", "BookingReachesTracking")]
    public async Task BookingReachesTracking()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        var ct = cts.Token;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>(ct);
        await using var app = await appHost.BuildAsync(ct);
        await app.StartAsync(ct);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("shipping-api", ct);
        await app.ResourceNotifications.WaitForResourceHealthyAsync("tracking-service", ct);

        using var shipping = app.CreateHttpClient("shipping-api");
        using var tracking = app.CreateHttpClient("tracking-service");

        var shipmentId = Guid.NewGuid();
        var trackingNumber = $"TRK-{Guid.NewGuid():N}";

        var booking = await shipping.PostAsJsonAsync("/shipments", new { shipmentId, trackingNumber }, ct);
        booking.EnsureSuccessStatusCode();

        var deadline = DateTime.UtcNow.AddSeconds(60);
        var status = HttpStatusCode.NotFound;
        while (true)
        {
            var response = await tracking.GetAsync($"/tracking/{trackingNumber}", ct);
            status = response.StatusCode;
            if (status == HttpStatusCode.OK || DateTime.UtcNow >= deadline)
            {
                break;
            }
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
        }

        Assert.Equal(HttpStatusCode.OK, status);
    }
}
