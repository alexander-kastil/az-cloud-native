using System;
using Newtonsoft.Json;

namespace FoodApp
{
    public class OrderEventResponse{
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("eventType")]
        public string EventType { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }     
        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }   
    }
}