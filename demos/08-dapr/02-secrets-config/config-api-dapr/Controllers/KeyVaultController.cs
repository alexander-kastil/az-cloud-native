using Azure.Core;
using Azure.Identity;
using Dapr.Client;

// using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

namespace configapidapr;

[ApiController]
[Route("[controller]")]
public class KeyVaultController : ControllerBase
{
    private readonly ILogger<KeyVaultController> logger;
    private IConfiguration cfg;

    const string DAPR_SECRET_STORE = "localsecretstore";  
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
        var store = cfg.GetValue<string>("DAPR_SECRET_STORE");
        var secret = await client.GetSecretAsync(store, "dapr-secret");
        var secretValue = string.Join(", ", secret);
        return secretValue;
    }
}
