using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;

namespace ContosoLogistics.Tests;

public class DeployedGates
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(60) };

    private static string Env(string name) =>
        Environment.GetEnvironmentVariable(name) is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException($"Environment variable {name} is not set.");

    private static async Task<JsonElement> GetHealthAsync(string urlVariable)
    {
        var response = await Http.GetAsync($"{Env(urlVariable).TrimEnd('/')}/health");
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == HttpStatusCode.OK, $"{urlVariable} /health returned {(int)response.StatusCode}: {body}");
        return JsonDocument.Parse(body).RootElement;
    }

    private static string EntryStatus(JsonElement health, string entry)
    {
        Assert.True(health.GetProperty("entries").TryGetProperty(entry, out var found), $"Health report has no entry {entry}.");
        return found.GetProperty("status").GetString()!;
    }

    private static async Task<JsonElement> SendAsync(HttpMethod method, string url, string scope, object? body = null)
    {
        var token = await new AzureCliCredential().GetTokenAsync(new TokenRequestContext([scope]));
        using var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        using var response = await Http.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"{method} {url} returned {(int)response.StatusCode}: {text}");
        return JsonDocument.Parse(text).RootElement.Clone();
    }

    [Fact]
    [Trait("Gate", "DeployedSmoke")]
    public async Task DeployedSmoke()
    {
        foreach (var variable in new[] { "SHIPPING_API_URL", "TRACKING_SERVICE_URL" })
        {
            var health = await GetHealthAsync(variable);
            Assert.Equal("Healthy", health.GetProperty("status").GetString());
        }
    }

    [Fact]
    [Trait("Gate", "CarrierKeyFromKeyVault")]
    public async Task CarrierKeyFromKeyVault()
    {
        var health = await GetHealthAsync("SHIPPING_API_URL");
        Assert.Equal("Healthy", EntryStatus(health, "carrier-api-key"));
    }

    [Fact]
    [Trait("Gate", "TrackingPageSizeFromAppConfig")]
    public async Task TrackingPageSizeFromAppConfig()
    {
        var health = await GetHealthAsync("TRACKING_SERVICE_URL");
        Assert.Equal("Healthy", EntryStatus(health, "app-configuration"));
    }

    [Fact]
    [Trait("Gate", "TrackingV2TrafficSplit")]
    public async Task TrackingV2TrafficSplit()
    {
        var app = $"https://management.azure.com/subscriptions/{Env("AZURE_SUBSCRIPTION_ID")}/resourceGroups/{Env("AZURE_RESOURCE_GROUP")}/providers/Microsoft.App/containerApps/tracking-service";
        const string scope = "https://management.azure.com/.default";

        var show = await SendAsync(HttpMethod.Get, $"{app}?api-version=2024-03-01", scope);
        var properties = show.GetProperty("properties");
        var configuration = properties.GetProperty("configuration");

        Assert.Equal("Multiple", configuration.GetProperty("activeRevisionsMode").GetString());

        var traffic = configuration.GetProperty("ingress").GetProperty("traffic").EnumerateArray()
            .Select(t => (Revision: t.GetProperty("revisionName").GetString(), Weight: t.GetProperty("weight").GetInt32()))
            .ToList();

        var revisions = await SendAsync(HttpMethod.Get, $"{app}/revisions?api-version=2024-03-01", scope);
        Assert.True(revisions.GetProperty("value").GetArrayLength() >= 2, "tracking-service has fewer than two revisions.");

        var latest = properties.GetProperty("latestRevisionName").GetString();
        Assert.Equal(2, traffic.Count);
        Assert.Contains((latest, 10), traffic);
        Assert.Contains(traffic, t => t.Weight == 90 && t.Revision != latest);
    }

    [Fact]
    [Trait("Gate", "BookingTraceInAppInsights")]
    public async Task BookingTraceInAppInsights()
    {
        const string query = """
            let bookings = AppRequests | where TimeGenerated > ago(1d) and AppRoleName endswith 'shipping-api' and Name startswith 'POST' | summarize by OperationId;
            union AppRequests, AppDependencies, AppTraces
            | where TimeGenerated > ago(1d) and AppRoleName endswith 'tracking-service'
            | join kind=inner bookings on OperationId
            | summarize Rows = count() by OperationId
            """;

        var result = await SendAsync(
            HttpMethod.Post,
            $"https://api.loganalytics.io/v1/workspaces/{Env("APP_INSIGHTS_WORKSPACE_ID")}/query",
            "https://api.loganalytics.io/.default",
            new { query });

        var rows = result.GetProperty("tables")[0].GetProperty("rows");
        Assert.True(rows.GetArrayLength() >= 1, "No booking operation holds both a shipping-api POST request and a tracking-service row.");
    }
}
