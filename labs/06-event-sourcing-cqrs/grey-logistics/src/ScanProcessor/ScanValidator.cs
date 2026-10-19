using System.Text.Json;
using System.Text.Json.Serialization;
using Grey.Contracts;

namespace ScanProcessor;

public static class ScanValidator
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static (ParcelScanned? Scan, string? Error) Validate(BinaryData body)
    {
        ParcelScanned? scan;

        try
        {
            scan = body.ToObjectFromJson<ParcelScanned>(JsonOptions);
        }
        catch (JsonException ex)
        {
            return (null, $"Body is not a valid ParcelScanned: {ex.Message}");
        }

        if (scan is null)
        {
            return (null, "Body is not a valid ParcelScanned: body is null");
        }

        if (!Enum.IsDefined(scan.ScanType))
        {
            return (null, $"Unknown scan type: {(int)scan.ScanType}");
        }

        return (scan, null);
    }
}
