using Newtonsoft.Json;

public class TestMessage{

    [JsonProperty("id")]
    int Id { get; set; }

    [JsonProperty("subject")]
    string Subject { get; set; }
}