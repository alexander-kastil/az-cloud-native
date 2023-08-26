# Working with Secrets & Revisions

Execute `create-sql-server.azcli` to create a SQL Server instance in Azure.

Open project [config-api](../../03-containers/01-build-publish/config-api/) and ensure that support for Azure SQL ist configured:

- dotnet add package Microsoft.Data.SqlClient --version 5.1.1
- dotnet add package Microsoft.EntityFrameworkCore --version 6.0.21
- dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 6.0.21
- dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.21

- Add EF Core tools:

    ```bash
    dotnet tool install --global dotnet-ef
    ```

- Set `UseSQLite` to `false` and update `SQLServerConnection` in `appsettings.json`

- Create Initial Migration:

    ```bash
    dotnet ef migrations add InitialCreate
    ```

- Apply Initial Migration:

    ```bash
    dotnet ef database update
    ```    

- Test if the database connection works on your local machine

Execute `deploy-app.azcli` to deploy the application to Azure Container Apps.

- Update environment variables:

    ```bash
    az containerapp update -n $appApi -g $grp --image $apiImg \
        --replace-env-vars App__UseSQLite=false 
    ```

- Add secret reference:

    ```bash
    az containerapp secret set  -n $appApi -g $grp --secrets "dbcon=$cs"

    az containerapp update -n $appApi -g $grp --image $apiImg \
        --replace-env-vars \App__UseSQLite=false App__ConnectionStrings__SQLServerConnection=secretref:dbcon
    ```