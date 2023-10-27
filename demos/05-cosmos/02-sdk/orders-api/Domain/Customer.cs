using Newtonsoft.Json;

namespace FoodApp
{
    public class Customer
    {
        public Customer()
        {
            Address = new Address();
        }

        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("email")]
        public string EMail { get; set; }
        [JsonProperty("address")]
        public Address Address { get; set; }
    }
}