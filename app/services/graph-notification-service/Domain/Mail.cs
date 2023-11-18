using Newtonsoft.Json;

namespace FoodApp
{
    public class Mail
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("recipient")]
        public string Recipient { get; set; }
    }
}