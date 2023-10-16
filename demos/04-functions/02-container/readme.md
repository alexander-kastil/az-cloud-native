# Hosting and Scaling Function Apps in Containers

## Environment Variables and Containerized Functions

In this demo you will learn how to access and override environment variables in Azure Functions that are hosted in containers

- Execute [deploy-func.azcli](deploy-func.azcli) to create an Azure App Configuration Service instance.

- For the ease of the demo local.settings.json is checked in to GitHub:

    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
            "AppConfigConnection": "Endpoint=https://appconfigdemo.azconfig.io;Id=xxxxxx;Secret=xxxxxx",
            "Func:Title": "Default Title",
            "Environment": "development"
        }
    }
    ```

    >Note: Update `AppConfigConnection` to point to your Azure App Configuration Service instance.

- Build the container:

    ```bash
    docker build --rm -f dockerfile -t config-func .
    ```

- Run the container:

    ```bash
    docker run -d --rm -p 5053:80 -e "Func:Title='Overridden Title'" config-func
    ```

- Browse to the following URL:

    ```bash
    CTRL+ Click http://localhost:5053/api/getValue?paramName=Func:Title
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
            "UseManagedIdentity": "false",
            "AppConfigEndpoint": "<App Config Endpoint>",
            "AppConfigConnection": "<App Config Connection String>",
            "Func:Title": "Default Title",
            "Environment": "development"
        }
    }
    ```

- Set the value of `UseAppConfig` to `true` and `UseManagedIdentity` to `false` to access App Configuration Service using a ConnectionString.

- Examine the current state of Program.cs:

    ```c#
    var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var useAppConfig = Environment.GetEnvironmentVariable("UseAppConfig");
        if(useAppConfig!=null && Boolean.Parse(useAppConfig)){
            var cs = Environment.GetEnvironmentVariable("AppConfigConnection");
            if(cs!=null){                              
                builder.AddAzureAppConfiguration(cs);
            }
        }
    })
    ```

- Start debug mode and use the following Url:

    ```bash
    CTRL+ Click http://localhost:7071/api/getValue?paramName=FuncappTitle
    ```

- Run the container and override the `AppConfigConnection` environment variable with the connection string from Azure App Configuration Service:

    ```bash
    docker run -d -p 5053:80 -e "AppConfigConnection=$configCon" -e "UseAppConfig=true"  config-func:v1
    ```

- Test the function using:

    ```bash
    http://localhost:5053/api/getValue?paramName=FuncappTitle
    ```

## Use Managed Identity in Azure Functions    

In this demo you will learn how to access Azure App Configuration Service from Azure Functions using Managed Identity.

- Update Program.cs to use Managed Identity:

    ```c#
    var host = new HostBuilder()
        .ConfigureAppConfiguration(builder =>
        {
            var useAppConfig = Environment.GetEnvironmentVariable("UseAppConfig");
            if (useAppConfig != null && Boolean.Parse(useAppConfig))
            {
                Console.WriteLine("Using App Configuration");
                var useMi = Environment.GetEnvironmentVariable("UseManagedIdentity");
                var ep = Environment.GetEnvironmentVariable("AppConfigEndpoint");

                if (ep != null && useMi != null && Boolean.Parse(useMi))
                {
                    builder.AddAzureAppConfiguration(options =>
                        options.Connect(
                            new Uri(ep),
                            new ManagedIdentityCredential()));
                }
                else
                {
                    var cs = Environment.GetEnvironmentVariable("AppConfigConnection");
                    if (cs != null)
                    {
                        builder.AddAzureAppConfiguration(cs);
                    }
                }
            }
        })
        .ConfigureFunctionsWorkerDefaults()
        .Build();
    ```

- Rebuild the container:

    ```bash
    az acr build --image $funcapp:v2 --registry $acr --file dockerfile .
    ```

- Crete the container app and test

    ```
    http://<UrL>.westeurope.azurecontainerapps.io/api/getValue?paramName=FuncappTitle
    ```