namespace Contoso.Contracts;

public record CapacityReserved(Guid ShipmentId, string TrackingNumber, Guid ReservationId, DateTimeOffset ReservedAt);
