---
minutes: 10
---

# Aspire Testing: Distributed Integration Tests

A parcel booking crosses the shipping API, the message bus and the tracking projection, and a unit test of any one of them misses the handoffs between them.
This topic runs the real AppHost inside a test with `Aspire.Hosting.Testing`, calls its resources over HTTP, waits for health and checks an event flow from end to end.

## Testing the AppHost with DistributedApplicationTestingBuilder

`Aspire.Hosting.Testing` starts the real AppHost inside a test. The test project references the AppHost project, and `DistributedApplicationTestingBuilder.CreateAsync` takes the AppHost type as its generic argument.

```csharp
using System.Net;
using Aspire.Hosting.Testing;
using Xunit;

public class ShippingApiTests
{
    [Fact]
    public async Task ShippingApiAnswersOnItsHttpEndpoint()
    {
        var cancellationToken = new CancellationTokenSource(TimeSpan.FromMinutes(2)).Token;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>(cancellationToken);
        await using var app = await appHost.BuildAsync(cancellationToken);
        await app.StartAsync(cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("shipping-api", cancellationToken);

        using var httpClient = app.CreateHttpClient("shipping-api");
        using var response = await httpClient.GetAsync("/", cancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
```

The test then builds and starts the application. `CreateHttpClient` returns an `HttpClient` whose base address points at the named resource, so the test sends real HTTP to a real process. The same wiring that `aspire run` uses also serves the test.

The `await using` declaration disposes the application at the end of the test, and that disposal stops the containers the test started. The cancellation token caps the whole test, so a stuck resource fails the test and does not hang the build agent. The configure callback of `CreateAsync` can also disable the dashboard for test runs.

## Waiting for Resources to Be Healthy

A started application is not yet a ready application. `WaitForResourceHealthyAsync` waits until the named resource reports healthy, so the test sends its first request only when the service accepts it.

Wait for each resource that a test depends on. A fixed `Task.Delay` guesses a startup time, and the guess passes on a fast laptop and fails on a busy build agent.

## Testing an Event Flow End to End

An event flow crosses several resources, so the test waits for every one of them before it sends a request. The booking flow needs the bus, the shipping API, the tracking service and the routing service, and the test waits for all four. Each waited resource adds startup time, so the test waits only for the resources that the flow uses.

```csharp
await app.ResourceNotifications.WaitForResourceHealthyAsync("messaging", cancellationToken);
await app.ResourceNotifications.WaitForResourceHealthyAsync("shipping-api", cancellationToken);
await app.ResourceNotifications.WaitForResourceHealthyAsync("tracking-service", cancellationToken);
await app.ResourceNotifications.WaitForResourceHealthyAsync("routing-service", cancellationToken);
```

After the waits, the test calls the shipping API through its client, then polls the tracking service through its client until the parcel appears or the timeout expires. The poll proves the whole path: the HTTP call, the event on the topic, the subscription and the read model.

The booking happens asynchronously, so the assertion polls with a deadline and does not expect the parcel on the first read. A single read right after the booking would fail for the reason the design expects: the event has not reached the read model yet.

The deadline is a deliberate value in the test. A timeout that is too short fails on a slow build agent, and one that is too long hides a stuck flow. Keep one assertion per flow stage, so a failure names the stage that broke.

This topic shows the structure of the test. The endpoint paths belong to the service code, and later modules add them.

## When WebApplicationFactory Is the Better Tool

`WebApplicationFactory<TEntryPoint>` hosts one ASP.NET Core app in memory with a `TestServer`. It starts quickly and needs no container runtime, so it fits logic inside a single service.

`DistributedApplicationTestingBuilder` starts every resource of the AppHost, including emulators, so it fits flows that cross services and brokers. It needs a container runtime, and it starts more slowly than a factory.

| Question the test answers | Use |
| --- | --- |
| Does the test exercise one service's handlers and data access? | `WebApplicationFactory` |
| Does the test cross a bus, a database emulator or another service? | `DistributedApplicationTestingBuilder` |
| Does the test need the wiring from the AppHost? | `DistributedApplicationTestingBuilder` |

The two tools combine well. A factory test covers the handler logic of one service, and an AppHost test covers the handoff between that service and the bus.

A factory test runs in the test process, so it can replace one registration with a stub, such as a fake clock or a fake client. A test class that starts the AppHost once through a shared xUnit class fixture pays the startup cost once for every test in the class.

## Key Topics

- [Aspire testing overview](https://aspire.dev/testing/overview/)
- [Write your first Aspire test](https://aspire.dev/testing/write-your-first-test/)
- [Integration tests in ASP.NET Core](https://learn.microsoft.com/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
