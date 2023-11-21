using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;

namespace FoodApp
{
    public class FoodTelemetryInitializer : ITelemetryInitializer{

        IAppConfig config;
        public FoodTelemetryInitializer(IConfiguration cfg)
        {
            config = cfg.Get<IAppConfig>();
        }

        public void Initialize(ITelemetry telemetry)
        {            
            if (string.IsNullOrEmpty(telemetry.Context.Cloud.RoleName))
            {
                telemetry.Context.Cloud.RoleName = config.Title;
            }
        }
    }
}