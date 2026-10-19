namespace Grey.Contracts;

public record LabelPrinted(Guid ShipmentId, string TrackingNumber, Guid LabelId, DateTimeOffset PrintedAt);
