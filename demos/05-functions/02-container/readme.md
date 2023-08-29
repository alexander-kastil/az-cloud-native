# Hosting and Scaling Function Apps in Containers

## Environment Variables and Containerized Functions

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

- Update `AppConfigConnection` to point to your Azure App Configuration Service instance.

- Build the container:

    ```bash
    docker build --rm -f dockerfile -t config-func .
    ```

- Run the container:

    ```bash
    docker run -d --rm -p 5053:80 -e "Func:Title='Local Title'" config-func
    ```

- Browse to the following URL:

    ```bash
    CTRL+ Click http://localhost:5053/api/getEnvVariable?paramName=Func:Title
    ```

## Using App Configuration Service in Azure Functions

- Add the following code to `Program.cs` in the Azure Function project in the current folder:

    ```csharp
    .ConfigureAppConfiguration(builder =>
    {
        string cs = Environment.GetEnvironmentVariable("AppConfigConnection");
        builder.AddAzureAppConfiguration(cs);
    })
    ```

- The result should look like this:

    ```csharp
    var host = new HostBuilder()
        .ConfigureAppConfiguration(builder =>
        {
            string cs = Environment.GetEnvironmentVariable("AppConfigConnection");
            builder.AddAzureAppConfiguration(cs);
        })
        .ConfigureFunctionsWorkerDefaults()
        .Build();

    host.Run();
    ```

- Notice that local.settings.json contains a setting `App:Funcapp` which will be overriden by the value in Azure App Configuration Service. Start debug mode and use the following Url:

    ```bash
    TRL+ Click http://localhost:7071/api/getEnvVariable?paramName=FuncappTitle
    ```

- Re-build the container and assign it the `appcfg` tag:

    ```bash
    docker build --rm -f dockerfile -t config-func:v2 .
    ```

- Run the container and override the `AppConfigConnection` environment variable with the connection string from Azure App Configuration Service:

    ```bash
    docker run -d -p 5053:80 -e "AppConfigConnection=$configCon" config-func:v2
    ```

- Test the function using:

    ```bash
    http://localhost:5053/api/getEnvVariable?paramName=FuncappTitle
    ```