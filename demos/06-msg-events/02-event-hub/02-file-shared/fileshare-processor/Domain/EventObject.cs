using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodApp
{
    public class Identity
    {
        public string type { get; set; }
        public string tokenHash { get; set; }
    }

    public class Properties
    {
        public string accountName { get; set; }
        public string userAgentHeader { get; set; }
        public string referrerHeader { get; set; }
        public string clientRequestId { get; set; }
        public string etag { get; set; }
        public string serviceType { get; set; }
        public string objectKey { get; set; }
        public string lastModifiedTime { get; set; }
        public string metricResponseType { get; set; }
        public int serverLatencyMs { get; set; }
        public int requestHeaderSize { get; set; }
        public int responseHeaderSize { get; set; }
        public string tlsVersion { get; set; }
        public string accessTier { get; set; }
        public string responseMd5 { get; set; }
        public int? requestBodySize { get; set; }
        public int? contentLengthHeader { get; set; }
    }

    public class Record
    {
        public DateTime time { get; set; }
        public string resourceId { get; set; }
        public string category { get; set; }
        public string operationName { get; set; }
        public string operationVersion { get; set; }
        public string schemaVersion { get; set; }
        public int statusCode { get; set; }
        public string statusText { get; set; }
        public int durationMs { get; set; }
        public string callerIpAddress { get; set; }
        public string correlationId { get; set; }
        public Identity identity { get; set; }
        public string location { get; set; }
        public Properties properties { get; set; }
        public string uri { get; set; }
        public string protocol { get; set; }
        public string resourceType { get; set; }
    }

    public class LogStream
    {
        public List<Record> records { get; set; }
    }
}