public record ShipmentEventDocument(
    string id,
    string shipmentId,
    string trackingNumber,
    string documentType,
    string eventType,
    int version,
    DateTimeOffset occurredAt,
    string? scanType,
    Dictionary<string, object?> data);
