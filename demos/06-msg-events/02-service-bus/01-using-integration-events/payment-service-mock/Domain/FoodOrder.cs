using Newtonsoft.Json;

namespace FoodApp
{
    public class FoodOrder{
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("customerId")]
        public int CustomerID {get;set;}
        [JsonProperty("itemId")]
        public int FoodItemID {get;set;}
        [JsonProperty("amount")]
        public int Amount {get;set;}
        [JsonProperty("price")]
        public decimal Price {get;set;}
    }
} 