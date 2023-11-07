using System;
using Newtonsoft.Json;

namespace FoodApp
{
    public class IntegrationEvent
    {
        public IntegrationEvent()
        {
            EventId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        public IntegrationEvent(object data){
            EventId = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
            Data = data;
            EventType = data.GetType().Name;
        }

        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("eventType")]
        public string EventType {get;set;}

        [JsonProperty("data")]
        public object Data { get; set; }
    }
}
