# Using Managed Identities & Service Connector to access services

## Using a managed Identity to access a Key Vault

- Execute `create-kv-app` to create a Key Vault in Azure. The kv-api will be configured to access this Key Vault when running as a container in Azure Container Apps.

## Using a managed Identity and Microsoft.Data.SqlClient to access a SQL Server using a password-less connection string

Execute [create-sql-server.azcli](/demos/04-azure-container-apps/02-secrets-revisions/create-sql-server.azcli) to create a SQL Server instance in Azure.

Open project [config-api](/demos/00-app/config-api/) and ensure that support for Azure SQL is configured:

- dotnet add package Microsoft.Data.SqlClient --version 5.1.1