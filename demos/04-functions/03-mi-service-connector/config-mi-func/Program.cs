using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var appConfigEP = Environment.GetEnvironmentVariable("UseAppConfig");
        if (appConfigEP != null)
        {
            Console.WriteLine("Using App Configuration");

            builder.AddAzureAppConfiguration(options =>
                options.Connect(
                    new Uri(appConfigEP),
                    new DefaultAzureCredential()));
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
