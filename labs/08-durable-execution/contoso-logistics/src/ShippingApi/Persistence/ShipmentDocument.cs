using Newtonsoft.Json;

public record ShipmentDocument(
    string id,
    string shipmentId,
    string trackingNumber,
    string documentType,
    string eventType,
    int version,
    DateTimeOffset occurredAt,
    string? scanType,
    Dictionary<string, object?> data,
    [property: JsonProperty("ttl", NullValueHandling = NullValueHandling.Ignore)] int? ttl = null);
