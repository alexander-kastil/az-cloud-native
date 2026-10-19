namespace Grey.Contracts;

public record DeliveryAttempted(Guid ShipmentId, string TrackingNumber, int AttemptNumber, string Outcome, DateTimeOffset AttemptedAt);
