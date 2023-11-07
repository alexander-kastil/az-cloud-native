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

    public class PaymentResponse{
        [JsonProperty("orderId")]
        public string OrderId {get;set;}
        [JsonProperty("status")]
        public string Status {get;set;}
        [JsonProperty("data")]
        public object Data {get;set;}        
    }
}