using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var useAppConfig = Environment.GetEnvironmentVariable("UseAppConfig");
        if(useAppConfig!=null && Boolean.Parse(useAppConfig)){
            Console.WriteLine("Using App Configuration");
            var cs = Environment.GetEnvironmentVariable("AppConfigConnection");
            if(cs!=null){                              
                builder.AddAzureAppConfiguration(cs);
            }
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
