# Working with Secrets & Revisions

>Note: This demo used a pre-installed SQL Server Developer Edition running in a VM. The VM is not part of this repository.

Open project [config-api](/demos/00-app/config-api/) and ensure that support for SQL Server is configured:

- dotnet add package Microsoft.EntityFrameworkCore --version 6.0.21
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.21
- dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.21

- Add EF Core tools:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

- Set `UseSQLite` to `false` and update `SQLServerConnection` in `appsettings.json` and update the connection string:

    ```json
    "UseSQLite": false
    "ConnectionStrings": {
        ...
        "SQLServerConnection": "Data Source=<server>.westeurope.cloudapp.azure.com;Initial Catalog=demo-db;Persist Security Info=True;User ID=demo-login;Password='<pwd>'"
    },
    ```

    >Note: You might have to replace the server name with the IP address of the VM and the password with the password of the `demo-login` user if you want to follow along.


- Test if the database connection works on your local machine

Execute [deploy-app.azcli](/demos/04-azure-container-apps/02-secrets-revisions/deploy-app.azcli) to deploy the application to Azure Container Apps.

- Check the revision history:

    ```bash
    az containerapp revision list -n $appApi -g $grp -o table
    ```

- Update environment variables:

    ```bash
    az containerapp secret set  -n $appApi -g $grp --secrets "sqlcon=$conSQL"

    az containerapp update -n $appApi -g $grp --image $apiImg \
        --replace-env-vars \App__UseSQLite=false App__ConnectionStrings__SQLServerConnection=secretref:sqlcon
    ```

- Check the `/settings` endpoint:

    ```bash
    apiUrl=$(az containerapp show -n $appApi -g $grp --query properties.configuration.ingress.fqdn -o tsv)
    az rest --method get --uri https://$apiUrl/settings    

- Add secret and secret reference:

    ```bash
    cs="Data Source=<server>.westeurope.cloudapp.azure.com;Initial Catalog=demo-db;Persist Security Info=True;User ID=demo-login;Password='P@ssw0rd4dem0'"
    az containerapp secret set  -n $appApi -g $grp --secrets "dbcon=$cs"

    az containerapp update -n $appApi -g $grp --image $apiImg --replace-env-vars App__UseSQLite=false App__ConnectionStrings__SQLServerConnection=secretref:dbcon
    ```