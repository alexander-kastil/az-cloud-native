using System;
using Newtonsoft.Json;

namespace food_invoices_dapr
{
    public class FoodItem
    {
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("instock")]
        public int InStock { get; set; } 
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}