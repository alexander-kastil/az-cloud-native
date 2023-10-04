using System;
using Newtonsoft.Json;

namespace FoodApp.Orders
{
    public class OrderEvent : IEvent
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

    public enum OrderEventType
    {
        OrderCreated,
        PaymentRequested,
        PaymentSuccess,
        PaymentFailed,
        ProductionRequested,
        ProductionStarted,
        ProductionCompleted,
        ProductionNotCompleted,
        DeliveryStarted,
        DeliveryCompleted,
        OrderCompleted,
        OrderCanceled
    }
}