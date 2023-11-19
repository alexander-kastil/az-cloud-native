using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json;

namespace FoodApp
{
    public class AILogger
    {
        private TelemetryClient ai;

        public AILogger(TelemetryClient tc)
        {
            ai = tc;
        }

        public void LogEvent(string text, object item)
        {
            string param = JsonConvert.SerializeObject(item);
            ai.TrackEvent(text, new Dictionary<string, string> { { text, param } });
        }

        public void LogEvent(string text, string param)
        {
            try
            {
                ai.TrackEvent(text, new Dictionary<string, string> { { text, param } });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
        }

        public void LogEvent(string text, Exception exception)
        {
            try
            {
                ai.TrackEvent(text, new Dictionary<string, string> { { "Error", exception.Message } });
            }
            catch (System.Exception ex)
            {                
                Console.WriteLine(ex.Message);           
            }
        }

        public void LogEvent(string text, Dictionary<string, string> arr)
        {
            try
            {
                ai.TrackEvent(text, arr);
            }
            catch (System.Exception ex)
            {                
                Console.WriteLine(ex.Message);           
            }
        }
    }
}