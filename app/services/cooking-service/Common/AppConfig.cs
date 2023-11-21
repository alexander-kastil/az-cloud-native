namespace FoodApp
{
    public class AppConfig : IAppConfig
    {
        public Logging Logging { get; set; }
        public ApplicationInsights ApplicationInsights { get; set; }
        public string AllowedHosts { get; set; }
        public string Title { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string Microsoft { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
    }

    public class ApplicationInsights
    {
        public string ConnectionString { get; set; }
    }
}