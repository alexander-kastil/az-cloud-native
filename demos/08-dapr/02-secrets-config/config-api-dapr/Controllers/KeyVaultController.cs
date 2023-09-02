using Azure.Core;
using Azure.Identity;
using Dapr.Client;

// using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

namespace ConfigApi;

[ApiController]
[Route("[controller]")]
public class KeyVaultController : ControllerBase
{
    private readonly ILogger<KeyVaultController> logger;
    private IConfiguration cfg;
    private readonly DaprClient client;

    public KeyVaultController(ILogger<KeyVaultController> lgr, IConfiguration configuration, DaprClient daprClient)
    {
        logger = lgr;
        cfg = configuration;
        client = daprClient;
    }

    [HttpGet("GetValue")]
    public async Task<string> Get()
    {
        var metadata = new Dictionary<string, string> { ["version_id"] = "3" };
        var store = cfg.GetValue<string>("DAPR_SECRET_STORE");
        Dictionary<string, string> secrets = await client.GetSecretAsync(store, "daprsecret", metadata);
        var secretValue = string.Join(", ", secret);
        return secretValue;
    }
}
