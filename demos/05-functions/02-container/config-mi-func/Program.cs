using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(builder =>
    {
        var cs = Environment.GetEnvironmentVariable("AppConfigConnection");
        if(cs!=null){
            builder.AddAzureAppConfiguration(cs);
        }
    })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
