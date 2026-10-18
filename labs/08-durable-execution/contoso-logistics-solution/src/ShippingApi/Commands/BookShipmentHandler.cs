using System.Security.Cryptography;
using System.Text;
using Contoso.Contracts;
using Newtonsoft.Json.Linq;

public class BookShipmentHandler(ShipmentOutboxWriter writer)
{
    public static string TrackingNumberFor(string shipmentId) =>
        "TRK-" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(shipmentId)))[..10];

    public async Task<bool> HandleAsync(BookShipment command, CancellationToken cancellationToken)
    {
        var trackingNumber = TrackingNumberFor(command.ShipmentId);
        var booked = new ShipmentBooked(
            Guid.Parse(command.ShipmentId),
            trackingNumber,
            command.CustomerId,
            new Address(command.DeliveryAddress, string.Empty, string.Empty, string.Empty),
            "Medium",
            1m,
            "Standard",
            DateTimeOffset.UtcNow);

        var data = JObject.FromObject(booked).ToObject<Dictionary<string, object?>>()!;
        data["PickupAddress"] = command.PickupAddress;
        data["PromisedBy"] = command.PromisedBy;

        var bookedDocument = new ShipmentDocument(
            $"{command.ShipmentId}-v000001",
            command.ShipmentId,
            trackingNumber,
            "event",
            nameof(ShipmentBooked),
            1,
            booked.BookedAt,
            null,
            data);
        var outboxDocument = new ShipmentDocument(
            Guid.NewGuid().ToString(),
            command.ShipmentId,
            trackingNumber,
            "outbox",
            nameof(ShipmentBooked),
            1,
            booked.BookedAt,
            null,
            data,
            864000);

        return await writer.SaveBookingAsync(bookedDocument, outboxDocument, cancellationToken);
    }
}
