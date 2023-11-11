# Environment Variables, Key Vault and App Configuration

## Links & Resources

[Azure Functions on Azure Container Apps](https://github.com/Azure/azure-functions-on-container-apps)

## Environment Variables and Containerized Functions

In this demo you will learn how to access and override environment variables in Azure Functions that are hosted in containers

- Execute [deploy-config-func.azcli](deploy-config-func.azcli) to create an Azure App Configuration Service instance.

- For the ease of the demo local.settings.json is checked in to GitHub:

    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            "AppConfigConnection": "Endpoint=https://appconfigdemo.azconfig.io;Id=xxxxxx;Secret=xxxxxx",
            "CustomConfigValue": "Default Value",
            "Environment": "development"
        }
    }
    ```

    >Note: Update `AppConfigConnection` to point to your Azure App Configuration Service instance.

- Build the container:

    ```bash
    docker build --rm -f dockerfile -t config-mi-func:v1 --no-cache .
    ```

- Run the container:

    ```bash
    docker run -d --rm -p 5053:80 -e "CustomConfigValue='Overridden Config Value'" config-mi-func:v1
    ```

- Browse to the following URL:

    ```bash
    CTRL+ Click http://localhost:5053/api/getConfigValue?paramName=CustomConfigValue
    ```

## Use App Configuration Service in Azure Functions

In this demo you will learn how to access Azure App Configuration Service from Azure Functions using a ConnectionString.

- Examine local.settings.json:

    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            "UseAppConfig": "false",
            "AppConfigConnection": "<App Config Connection String>",
            "CustomConfigValue": "Default Value",
            "Environment": "development"
        }
    }
    ```

- Set the value of `UseAppConfig` to `true` to access App Configuration Service using a ConnectionString.

- Examine the current state of Program.cs:

    ```c#
    var host = new HostBuilder()
        .ConfigureAppConfiguration(builder =>
        {
            var useAppConfig = Environment.GetEnvironmentVariable("UseAppConfig");        
            if (useAppConfig != null && Boolean.Parse(useAppConfig))
            {            
                var appConfigCS = Environment.GetEnvironmentVariable("AppConfigConnection");
                if (appConfigCS != null)
                {
                    builder.AddAzureAppConfiguration(appConfigCS);
                }
            }
        })
        .ConfigureFunctionsWorkerDefaults()
        .Build();
    ```

- Run the container and override the `AppConfigConnection` environment variable with the connection string from Azure App Configuration Service:

    ```bash
    docker run -d -p 5053:80 -e "AppConfigConnection=$appConfigCS" -e "UseAppConfig=true"  config-mi-func:v1
    ```

- Test the function using:

    ```bash
    CTRL+ Click http://localhost:5053/api/getConfigValue?paramName=AppConfigValue
    ```