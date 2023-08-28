# Using Managed Identities & Service Connector to access services

## Using a managed Identity to access a Key Vault

- Execute [create-kv-app.azcli](/demos/04-azure-container-apps/03-managed-identity/create-kv-app.azcli) to create a Key Vault in Azure. The kv-api will be configured to access this Key Vault when running as a container in Azure Container Apps.

    >Note: Setting the AZURE_CLIENT_ID is essential for the managed identity auth to work:

    ```bash
    az containerapp create -n $app -g $grp --image $img \
        --environment $acaenv \
        --target-port 80 --ingress external \
        --min-replicas 0 --max-replicas 1 \
        --user-assigned $miId \
        --env-vars KEY_VAULT_NAME=$kv AZURE_CLIENT_ID=$miClientId
    ```

## Using a managed Identity and Microsoft.Data.SqlClient to access a SQL Server using a password-less connection string

- Execute [create-sql-server-app.azcli](/demos/04-azure-container-apps/03-managed-identity/create-sql-server-app.azcli) to create a SQL Server instance in Azure. It creates the SQL server, enables AzureAD auth and sets the default AzureAD admin. It also sets the firewall rules to allow access from the Azure Container App by setting the "Allow Azure"-rule.

- Open project [config-api](/demos/00-app/config-api/) and ensure that support for `password-less` Azure SQL auth is configured:

    ```bash
    dotnet add package Microsoft.Data.SqlClient --version 5.1.1
    ```

- Update `SQLServerConnection` to the following value. Replace `<AZURE-SQL-SERVERNAME>` with the name of the SQL Server instance you created in the previous step and `<DATABASE>` with the name of the database you want to connect to:

    ```
    "Data Source=<AZURE-SQL-SERVERNAME>; Initial Catalog=<DATABASE>; Authentication=Active Directory Managed Identity; Encrypt=True";
    ```

- Build and push the image to ACR:

    ```bash
    env=dev
    acr=aznative$env
    imgApi=config-api:v3
    az acr build --image $imgApi --registry $acr --file dockerfile .
    ```

