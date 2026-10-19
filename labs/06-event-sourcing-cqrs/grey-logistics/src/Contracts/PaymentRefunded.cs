namespace Grey.Contracts;

public record PaymentRefunded(Guid ShipmentId, string TrackingNumber, Guid PaymentId, decimal RefundedAmount, string RefundReason);
