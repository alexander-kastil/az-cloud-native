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

host.Run();
