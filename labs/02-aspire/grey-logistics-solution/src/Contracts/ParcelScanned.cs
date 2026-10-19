namespace Grey.Contracts;

public record ParcelScanned(Guid ParcelId, Guid ShipmentId, string TrackingNumber, ScanType ScanType, string ScanLocation, DateTimeOffset ScannedAt);
