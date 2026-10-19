namespace Grey.Contracts;

public record RerouteApproved(Guid ShipmentId, string TrackingNumber, Address NewAddress, string NewRoute, DateTimeOffset NewEstimatedArrival);
