using Newtonsoft.Json;

namespace FoodApp
{
    public class PaymentRequest{
        [JsonProperty("orderId")]
        public string OrderId {get;set;}
        [JsonProperty("amount")]
        public decimal Amount {get;set;}
        [JsonProperty("paymentInfo")]
        public PaymentInfo PaymentInfo {get;set;}
    }
}