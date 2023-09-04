# Secrets and Configuration

- Using Dapr Secrets Store and Azure Key Vault
- Using Dapr Configuration Store

## Links & Resources

[The Dapr secrets management building block](https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/secrets-management)

[Azure App Configuration configuration store component](https://docs.dapr.io/reference/components-reference/supported-configuration-stores/azure-appconfig-configuration-store/)

## Using Dapr Secrets Store and Azure Key Vault

Dapr’s dedicated secrets building block API makes it easier for developers to consume application secrets from a secret store. 

![Dapr Secrets](_images/secrets.png)

- Execute [create-kv-dapr-app.azcli](create-kv-dapr-app.azcli) to create a Key Vault and a secret in Azure Key Vault.

- Start the Dapr sidecar and the [food-api-dapr](../00-app//food-api-dapr/) application using the following command:

    ```bash
    dapr run --app-id food-api --app-port 5000 --dapr-http-port 5010 --resources-path './components' dotnet run
    ```

- Examine `secretstore.yaml`. 

    ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
    name: azurekeyvault
    spec:
    type: secretstores.azure.keyvault
    version: v1
    metadata:
    - name: vaultName
        value: "aznativekvdev"
    - name: azureTenantId
        value: ""
    - name: azureClientId
        value: ""
    - name: azureClientSecret
        value: ""
    ```

- Examine [KeyVaultController.cs](../00-app/food-api-dapr/Controllers/KeyVaultController.cs) and the `GetSecret()` method:

    ```c#    
    [HttpGet("getSecret")]
    public async Task<string> Get(string secretName)
    {
        HttpClient client = new HttpClient();
        var daprResponse = await client.GetAsync($"http://localhost:5010/v1.0/secrets/azurekeyvault/{secretName}");
        var secretJson = await daprResponse.Content.ReadAsStringAsync();
        return JObject.Parse(secretJson)[secretName].ToString();
    }
    ```

- Test using:

    ```
    GET {{baseUrl}}/keyvault/getsecret?secretName=dapr-secret
    content-type: application/json
    ```
