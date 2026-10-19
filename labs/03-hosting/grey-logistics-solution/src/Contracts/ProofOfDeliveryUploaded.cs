namespace Grey.Contracts;

public record ProofOfDeliveryUploaded(Guid ShipmentId, string TrackingNumber, string ProofDocumentReference, DateTimeOffset UploadedAt);
