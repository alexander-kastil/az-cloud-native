using Microsoft.Extensions.Diagnostics.HealthChecks;

public class AppConfigHealthCheck(IConfiguration configuration) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = configuration["Tracking:PageSize"] == "50"
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();
        return Task.FromResult(result);
    }
}
