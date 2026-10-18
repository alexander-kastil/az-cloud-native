namespace Contoso.Contracts;

public record ShipmentBooked(Guid ShipmentId, string TrackingNumber, string Sender, Address RecipientAddress, string ParcelSize, decimal ParcelWeight, string ServiceLevel, DateTimeOffset BookedAt);
