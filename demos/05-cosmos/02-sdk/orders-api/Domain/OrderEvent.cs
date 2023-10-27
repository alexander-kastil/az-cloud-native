using System;
using Newtonsoft.Json;

namespace FoodApp
{
    public class OrderEvent 
    {
        public OrderEvent(){
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
        }

        public OrderEvent(string orderId, string eventType, object eventData)
        {
            Id = Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
            OrderId = orderId;
            EventType = eventType;
            EventData = eventData;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("customerId")]
        public string CustomerId { get; set; }

        [JsonProperty("orderId")]
        public string OrderId { get; set; }

        [JsonProperty("eventType")]
        public string EventType { get; set; }

        [JsonProperty("eventData")]
        public object EventData { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; private set; }
    }
}