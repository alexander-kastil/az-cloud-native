# Secrets and Configuration

- Using Dapr Secrets Store
- Using Dapr Configuration Store

## Links & Resources

[The Dapr secrets management building block](https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/secrets-management)

## Using Dapr Secrets Store

Dapr’s dedicated secrets building block API makes it easier for developers to consume application secrets from a secret store. 

![Dapr Secrets](_images/secrets.png)

- Execute `create-kv-dapr-app.azcli` to create a Key Vault and a secret in Azure Key Vault.

- Start the Dapr sidecar and the application using the following command:

```bash
cd config-api-dapr
dapr run --app-id dapr-kv --app-port 3500 --components-path ../components dotnet run
```