namespace Contoso.Contracts;

public record RerouteRequested(Guid ShipmentId, string TrackingNumber, Address RequestedNewAddress, string Reason, string Requester);
