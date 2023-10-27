using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FoodApp
{
    public class Order : IOrderEvent
    {
        public Order()
        {
            Id = Guid.NewGuid().ToString();
            Items = new List<OrderItem>();
            Events = new List<OrderEventMetadata>();
            Timestamp = DateTime.UtcNow;
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("total")]
        public decimal Total { get; set; }
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
        [JsonProperty("shippingAddress")]
        public Address ShippingAddress { get; set; }
        [JsonProperty("payment")]
        public PaymentInfo Payment { get; set; }
        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; }
        public string Status { get; set; }
        public List<OrderEventMetadata> Events { get; set; }
        public DateTime Timestamp { get; set; }
    }
}