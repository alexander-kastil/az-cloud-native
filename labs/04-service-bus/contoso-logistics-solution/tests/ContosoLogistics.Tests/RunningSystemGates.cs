using System.Net;
using System.Net.Http.Json;
using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace ContosoLogistics.Tests;

public sealed class AppHostFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>();
        App = await builder.BuildAsync();
        await App.StartAsync().WaitAsync(TimeSpan.FromMinutes(10));
    }

    public async Task DisposeAsync()
    {
        await App.DisposeAsync();
    }
}

public class RunningSystemGates(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
    private static readonly TimeSpan StartupTimeout = TimeSpan.FromMinutes(10);

    private DistributedApplication App => fixture.App;

    [Fact]
    [Trait("Gate", "ServicesReportHealthy")]
    public async Task ServicesReportHealthy()
    {
        foreach (var name in new[] { "shipping-api", "tracking-service", "exception-agent" })
        {
            await App.ResourceNotifications.WaitForResourceHealthyAsync(name).WaitAsync(StartupTimeout);

            using var client = App.CreateHttpClient(name);
            var response = await client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    [Trait("Gate", "BookingReachesTracking")]
    public async Task BookingReachesTracking()
    {
        await App.ResourceNotifications.WaitForResourceHealthyAsync("shipping-api").WaitAsync(StartupTimeout);
        await App.ResourceNotifications.WaitForResourceHealthyAsync("tracking-service").WaitAsync(StartupTimeout);

        var trackingNumber = $"CL-2026-{Random.Shared.Next(100000, 999999)}";
        using var shipping = App.CreateHttpClient("shipping-api");
        var booking = await shipping.PostAsJsonAsync("/shipments", new { shipmentId = Guid.NewGuid(), trackingNumber });

        Assert.True(booking.StatusCode == HttpStatusCode.Created, await booking.Content.ReadAsStringAsync());

        using var tracking = App.CreateHttpClient("tracking-service");
        var deadline = DateTime.UtcNow.AddSeconds(60);
        var status = HttpStatusCode.NotFound;
        while (DateTime.UtcNow < deadline)
        {
            var response = await tracking.GetAsync($"/tracking/{trackingNumber}");
            status = response.StatusCode;
            if (status == HttpStatusCode.OK)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        Assert.Equal(HttpStatusCode.OK, status);
    }

    [Fact]
    [Trait("Gate", "OpsConsoleServes")]
    public async Task OpsConsoleServes()
    {
        await App.ResourceNotifications.WaitForResourceAsync("ops-console", KnownResourceStates.Running).WaitAsync(StartupTimeout);

        using var client = App.CreateHttpClient("ops-console");
        var deadline = DateTime.UtcNow.AddMinutes(2);
        var status = HttpStatusCode.ServiceUnavailable;
        while (DateTime.UtcNow < deadline)
        {
            try
            {
                var response = await client.GetAsync("/");
                status = response.StatusCode;
            }
            catch (HttpRequestException)
            {
            }

            if (status == HttpStatusCode.OK)
            {
                break;
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        Assert.Equal(HttpStatusCode.OK, status);
    }
}
