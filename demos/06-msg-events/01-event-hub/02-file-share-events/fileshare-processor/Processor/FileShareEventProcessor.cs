using System;
using Azure.Messaging.EventHubs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FoodApp
{
    public class FileShareEventProcessor
    {
        private readonly ILogger<FileShareEventProcessor> _logger;

        public FileShareEventProcessor(ILogger<FileShareEventProcessor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(FileShareEventProcessor))]
        public void Run([EventHubTrigger("filesharehub-dev", Connection = "EventHubSharedKey")] EventData[] events)
        {           
            foreach (EventData eventData in events)
            {
                LogStream logs = JsonConvert.DeserializeObject<LogStream>(eventData.EventBody.ToString());

                foreach (Record record in logs.records)
                {
                    // Only process for file where write operation is completed
                    if (record.operationName == "PutRange")
                    {
                        string url = record.uri;
                        Uri uri = new Uri(url);
                        _logger.LogInformation("You can now process this file using claim-check-pattern: {scheme}//:{host}{path}", uri.Scheme, uri.Host, uri.AbsolutePath);
                    }
                }                            
            }
        }
    }
}
