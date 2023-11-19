using Newtonsoft.Json;

namespace FoodApp
{
    public class PaymentTransaction
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("customerId")]
        public string CustomerId { get; set; }
        [JsonProperty("orderId")]
        public string OrderId { get; set; }
        [JsonProperty("paymentInfo")]
        public PaymentInfo PaymentInfo { get; set; }
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }
}