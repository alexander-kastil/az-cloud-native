namespace Grey.Contracts;

public record ParcelDelivered(Guid ShipmentId, string TrackingNumber, string FinalStatus, string RecipientName, DateTimeOffset DeliveredAt);
