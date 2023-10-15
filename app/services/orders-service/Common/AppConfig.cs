namespace FoodApp.Orders {
     public class CosmosDB
    {
        public bool InitCosmosClient { get; set; }
        public string ConnectionString { get; set; }
        public string DBName { get; set; }
        public string Container { get; set; }
    }

    public class ApplicationInsights{
        public string ConnectionString {get;set;}
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    public class AppConfig
    {
        public Logging Logging { get; set; }
        public CosmosDB CosmosDB { get; set; }
        public string AllowedHosts { get; set; }
        public ApplicationInsights ApplicationInsights {get;set;}
    }

}