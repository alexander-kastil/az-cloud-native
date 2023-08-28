using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Mvc;

namespace kv_api;

[ApiController]
[Route("[controller]")]
public class KeyVaultController : ControllerBase
{
    private readonly ILogger<KeyVaultController> _logger;
    private IConfiguration cfg;

    public KeyVaultController(ILogger<KeyVaultController> logger, IConfiguration configuration)
    {
        _logger = logger;
        cfg = configuration;
    }

    [HttpGet(Name = "GetValue")]
    public string Get()
    {
        SecretClientOptions options = new SecretClientOptions()
        {
            Retry =
                    {
                        Delay= TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                     }
        };
        var keyValueName = cfg.GetValue<string>("KEY_VAULT_NAME");
        var client = new SecretClient(new Uri($"https://{keyValueName}.vault.azure.net/"), new DefaultAzureCredential(), options);
        var secret = client.GetSecret("demo-secret");
        return secret.Value.Value;
    }
}
