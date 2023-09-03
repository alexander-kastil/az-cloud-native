using System;
using System.Text.Json.Serialization;

namespace FoodDapr
{
    public class FoodItem
    {

        [JsonPropertyName("id")]        
        public int ID { get; set; }
        [JsonPropertyName("name")]        
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; } 
        public string PictureUrl { get; set; }
        public string Code { get; set; }
    }
}