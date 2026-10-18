namespace Contoso.Contracts;

public record DeliveryDelayed(Guid ShipmentId, string TrackingNumber, string DelayReason, DateTimeOffset NewEstimatedArrival);
