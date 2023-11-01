using Newtonsoft.Json;

namespace FoodApp
{
    public record PaymentInfo
    {
        [JsonProperty("type")]
        public string  Type { get; set; }
        [JsonProperty("accountNumber")]
        public string AccountNumber { get; set; }
    }
}