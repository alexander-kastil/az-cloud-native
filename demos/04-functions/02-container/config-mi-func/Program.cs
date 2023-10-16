using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var useAppConfig = Environment.GetEnvironmentVariable("UseAppConfig");
        if (useAppConfig != null && Boolean.Parse(useAppConfig))
        {
            Console.WriteLine("Using App Configuration");
            var useManagedIdentity = Environment.GetEnvironmentVariable("UseManagedIdentity");
            var appConfigEP = Environment.GetEnvironmentVariable("AppConfigEndpoint");

            if (!(appConfigEP == null || useManagedIdentity == null || !Boolean.Parse(useManagedIdentity)))
            {
                builder.AddAzureAppConfiguration(options =>
                    options.Connect(
                        new Uri(appConfigEP),
                        new DefaultAzureCredential()));
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

host.Run();
