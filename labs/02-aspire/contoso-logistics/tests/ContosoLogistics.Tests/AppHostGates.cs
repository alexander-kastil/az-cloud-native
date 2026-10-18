using Aspire.Hosting;
using Aspire.Hosting.Testing;

namespace ContosoLogistics.Tests;

public class AppHostGates
{
    [Fact]
    [Trait("Gate", "AppHostListsResources")]
    public async Task AppHostListsResources()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>();
        await using var _ = builder;

        var names = builder.Resources.Select(r => r.Name).ToHashSet();

        Assert.Contains("shipping-api", names);
        Assert.Contains("tracking-service", names);
        Assert.Contains("ops-console", names);
        Assert.Contains("exception-agent", names);
        Assert.Contains("messaging", names);
        Assert.Contains("cosmos", names);
    }
}
