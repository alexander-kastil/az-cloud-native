namespace FoodApp
{
    public class AppConfig : IAppConfig
    {
        public Logging Logging { get; set; }
        public ApplicationInsights ApplicationInsights { get; set; }
        public GraphCfg GraphCfg { get; set; }
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


       public class GraphCfg
    {
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CacheLocation { get; set; }
        public Endpoints Endpoints { get; set; }
        public string ReturnUrl { get; set; }
        public string MailSender { get; set; }
    }

    public class Endpoints
    {
        public string GraphApiUri { get; set; }
    }
}