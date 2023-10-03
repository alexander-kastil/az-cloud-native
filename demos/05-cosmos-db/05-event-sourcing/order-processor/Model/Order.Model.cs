using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FoodApp.Orders
{
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid().ToString();
            Items = new List<OrderItem>();
            Events = new List<OrderEvent>();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("total")]
        public decimal Total { get; set; }
        [JsonProperty("customer")]
        public Customer Customer { get; set; }
        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; }
        public List<OrderEvent> Events { get; set; }
        public bool CanceledByUser { get; set; }
    }

    public class Customer
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string EMail { get; set; }
        [JsonProperty("address")]
        public string Address { get; set; }
    }

    public class OrderItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("quantity")]
        public int Quantity { get; set; }
    }
}