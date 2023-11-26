using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FoodApp
{
    public class AILogger
    {
        private TelemetryClient ai;
        private AppConfig config;

        public AILogger(TelemetryClient tc, IConfiguration cfg)
        {
            ai = tc;
            config = cfg.Get<AppConfig>();            
        }
        
        public void LogEvent(string text, object item, bool logToConsole = false)
        {
            string value = JsonConvert.SerializeObject(item);
            ai.TrackEvent($"{config.Title} - {text}", new Dictionary<string, string> { { text, value } });
            if (logToConsole) Console.WriteLine($"{config.Title} - {text} - {value}");
        }

        public void LogEvent(string text, string param)
        {
            var props = new Dictionary<string, string> { { text, param } };
            ai.TrackEvent(text, props);
        }

        public void LogEvent(string text, Exception ex)
        {
            ai.TrackEvent(text, new Dictionary<string, string> { { "Error", ex.Message } });
        }

        public void LogEvent(string text, Dictionary<string, string> arr)
        {
            ai.TrackEvent(text, arr);
        }

    }
}