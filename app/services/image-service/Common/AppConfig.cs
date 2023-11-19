namespace FoodApp {
    public class CosmosDB
    {
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
        public string Title {get;set;}
        public string IMG_FOLDER_NAME {get;set;}
    }
}