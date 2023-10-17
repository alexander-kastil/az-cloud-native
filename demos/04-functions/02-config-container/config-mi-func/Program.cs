using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

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

host.Run();
