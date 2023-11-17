using Newtonsoft.Json;

namespace FoodApp
{
    public class Mail
    {
        [JsonProperty("id")]
        public string subject { get; set; }
        [JsonProperty("id")]
        public string text { get; set; }
        [JsonProperty("id")]
        public string recipient { get; set; }
    }
}