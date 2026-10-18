namespace Contoso.Contracts;

public record VanPositionReported(string VanId, double Latitude, double Longitude, double Speed, DateTimeOffset ReportedAt);
