# Using Managed Identities and Service Connector to access Azure Resources

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
    http://<UrL>.westeurope.azurecontainerapps.io/api/getValue?paramName=CustomConfigValue
    ```