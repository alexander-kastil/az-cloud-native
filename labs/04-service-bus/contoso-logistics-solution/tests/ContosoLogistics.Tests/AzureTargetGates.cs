using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;

namespace ContosoLogistics.Tests;

public class AzureTargetGates
{
    [Fact]
    [Trait("Gate", "AppHostDeclaresAzureTargets")]
    public async Task AppHostDeclaresAzureTargets()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>(["--operation", "publish", "--output-path", Path.GetTempPath()]);
        await using var _ = builder;

        var names = builder.Resources.Select(r => r.Name).ToHashSet();

        Assert.Contains("aca-env", names);
        Assert.Contains("key-vault", names);
        Assert.Contains("config", names);
        Assert.Contains("app-insights", names);

        foreach (var service in new[] { "shipping-api", "tracking-service" })
        {
            var resource = builder.Resources.Single(r => r.Name == service);
            Assert.Contains(resource.Annotations.OfType<EndpointAnnotation>(), endpoint => endpoint.IsExternal);
        }
    }
}
