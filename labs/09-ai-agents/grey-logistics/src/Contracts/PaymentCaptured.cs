namespace Grey.Contracts;

public record PaymentCaptured(Guid ShipmentId, string TrackingNumber, Guid PaymentId, decimal Amount, string Currency, DateTimeOffset CapturedAt);
