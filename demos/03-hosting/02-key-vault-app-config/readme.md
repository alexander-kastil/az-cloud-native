# Key Vault & App Configuration

> **_NOTE:_** Duration: about 7 minutes.

Contoso Logistics keeps carrier API keys, database connection details, and business settings that change between releases. Storing them in `appsettings.json` or in plain environment variables exposes the secrets to anyone who can read the configuration, and every settings change then needs a redeploy. The fix is to keep secrets in Key Vault, keep settings in App Configuration, and give each service a managed identity so that no password is stored.

This topic covers managed identity with `DefaultAzureCredential`, secret versions and rotation events, App Configuration keys with Key Vault references, and feature flags. It also shows the Aspire hosting and client integrations for both services, so the AppHost wires each resource and the services read it through the standard configuration and dependency injection model.

## Authenticate with a managed identity

`DefaultAzureCredential` determines at runtime which credential to use. On a developer machine it uses the sign-in from the Azure CLI, and in Azure it uses the managed identity assigned to the app. The same code runs in both places without environment checks.

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var vaultName = Environment.GetEnvironmentVariable("KEY_VAULT_NAME");
var client = new SecretClient(new Uri($"https://{vaultName}.vault.azure.net"), new DefaultAzureCredential());
```

Grant the identity read access with the Key Vault Secrets User role. The scope is the vault's resource ID.

```bash
az role assignment create --role "Key Vault Secrets User" --scope <KEY_VAULT_RESOURCE_ID> --assignee <PRINCIPAL_ID>
```

Aspire can wire the vault for you. The AppHost declares the vault and references it from the service, and Aspire assigns roles on the vault.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var keyVault = builder.AddAzureKeyVault("key-vault");

builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithReference(keyVault);

builder.Build().Run();
```

By default, `AddAzureKeyVault` gives the consuming project Key Vault Administrator. For read-only access, call `WithRoleAssignments` with `KeyVaultBuiltInRole.KeyVaultSecretsUser`, the more restrictive role that the Aspire example uses.

## Secrets, versions, and rotation

Each write to a secret creates a new version. The method below writes a rotated carrier key as a new version of the secret that the shipping API reads.

```csharp
public static async Task RotateCarrierKeyAsync(SecretClient client, string newKeyValue)
{
    await client.SetSecretAsync("carrier-api-key", newKeyValue);
}
```

Key Vault publishes events when a secret version is created or nears expiry. Event Grid delivers those events to subscribers.

| Event type | Raised when |
| --- | --- |
| `Microsoft.KeyVault.SecretNearExpiry` | The current version is about to expire, 30 days before the expiration date |
| `Microsoft.KeyVault.SecretNewVersionCreated` | A new secret or a new secret version is created |
| `Microsoft.KeyVault.SecretExpired` | The current version has expired |

Rotation works as a subscriber to `SecretNearExpiry`. The Learn tutorials route the event through Event Grid to a function that writes a new version and updates the consumer. Container Apps secret values need a revision restart before a container sees them, so plan rotations that restart the affected revision.

Prefer managed identity over shared secrets. Use rotation only where a workload still needs a password or a key.

## App Configuration, Key Vault references, and feature flags

App Configuration stores key-values and feature flags, and the .NET provider loads them into `IConfiguration`. A Key Vault reference is a key in App Configuration that points to a secret in Key Vault. The app reads the reference like any other key, and the secret stays in the vault.

The provider resolves the reference when you call `ConfigureKeyVault`. The Aspire client page doesn't describe this resolution, so the snippet below uses the provider directly.

```csharp
using Azure.Identity;

var appConfigEndpoint = builder.Configuration.GetValue<string>("Endpoints:AppConfiguration");

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(appConfigEndpoint!), new DefaultAzureCredential())
        .ConfigureKeyVault(kv => kv.SetCredential(new DefaultAzureCredential()));
});
```

The AppHost declares the store and runs it on a local emulator during development. The service then references the store and waits for it to start.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddAzureAppConfiguration("config")
    .RunAsEmulator();

builder.AddProject<Projects.TrackingService>("tracking-service")
    .WithReference(appConfig)
    .WaitFor(appConfig);

builder.Build().Run();
```

The service registers the Aspire client, loads the `Tracking` keys, and turns on feature flags. `UseFeatureFlags` loads flags, and `AddFeatureManagement` registers `IFeatureManager`.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration(
    "config",
    configureOptions: options =>
    {
        options.Select("Tracking:*");
        options.UseFeatureFlags();
    });

builder.Services.AddFeatureManagement();
```

Feature flags are stored as key-values with the `.appconfig.featureflag/` prefix. The value is a small JSON document with an identifier and a switch.

```json
{
  "id": "LiveMap",
  "enabled": true
}
```

The service injects `IFeatureManager` and checks the flag at runtime. The provider refreshes feature flags on its own, with a default interval of 30 seconds.

```csharp
public class TrackingPage(IFeatureManager featureManager)
{
    public Task<bool> ShowLiveMapAsync() => featureManager.IsEnabledAsync("LiveMap");
}
```

## Key Topics

- [Azure Key Vault developer's guide](https://learn.microsoft.com/azure/key-vault/general/developers-guide)
- [Automate the rotation of a secret for resources that use one set of authentication credentials](https://learn.microsoft.com/azure/key-vault/secrets/tutorial-rotation)
- [Azure Key Vault as an Event Grid source](https://learn.microsoft.com/azure/event-grid/event-schema-key-vault)
- [.NET configuration provider for Azure App Configuration](https://learn.microsoft.com/azure/azure-app-configuration/reference-dotnet-provider)
- [Quickstart: Add feature flags to an Aspire app](https://learn.microsoft.com/azure/azure-app-configuration/quickstart-feature-flag-aspire)
