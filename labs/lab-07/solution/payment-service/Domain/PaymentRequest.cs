using System.Text.Json;
using Newtonsoft.Json;

namespace FoodApp
{
    public class PaymentRequest
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("paymentInfo")]
        public PaymentInfo PaymentInfo { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}