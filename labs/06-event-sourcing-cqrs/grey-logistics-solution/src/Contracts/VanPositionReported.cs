namespace Grey.Contracts;

public record VanPositionReported(string VanId, long Sequence, double Latitude, double Longitude, DateTimeOffset ObservedAt);
