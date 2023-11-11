using System.Text.Json;
using Newtonsoft.Json;

namespace FoodApp
{
    public class PaymentResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("paymentInfo")]
        public string Status { get; set; }
        [JsonProperty("data")]
        public object Data { get; set; }
    }
}