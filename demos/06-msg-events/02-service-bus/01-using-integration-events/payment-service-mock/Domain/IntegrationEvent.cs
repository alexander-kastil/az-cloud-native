using System;
using Newtonsoft.Json;

namespace FoodApp
{
    public class IntegrationEvent<T>
    {
        public IntegrationEvent()
        {
        }

        public IntegrationEvent(T data){
            Data = data;
            CreationDate = DateTime.UtcNow;
            EventId = Guid.NewGuid();
            EventType = this.GetType().Name;
        }

        [JsonProperty("eventId")]
        public Guid EventId { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("eventType")]
        public string EventType {get;set;}

        [JsonProperty("data")]
        public T Data { get; set; }
    }
}
