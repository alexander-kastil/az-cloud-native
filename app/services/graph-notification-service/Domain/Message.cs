using System.Text.Json.Serialization;

public record TestMessage{

    [JsonPropertyName("id")]
    int Id { get; init; }

    [JsonPropertyName("subject")]
    string Subject { get; init; }
}