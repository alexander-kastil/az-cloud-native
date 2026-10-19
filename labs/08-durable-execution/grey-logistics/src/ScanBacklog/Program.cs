using System.Text.Json;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

var scanTypes = new[] { "PickedUp", "AtHub", "OutForDelivery" };
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);

await using var client = new ServiceBusClient(args[0], new AzureCliCredential());
await using var sender = client.CreateSender("parcel-events");

var count = int.Parse(args[1]);

for (var i = 0; i < count; i++)
{
    var trackingNumber = $"CL-2026-{i:D6}";

    var body = new
    {
        ParcelId = Guid.NewGuid(),
        ShipmentId = Guid.NewGuid(),
        TrackingNumber = trackingNumber,
        ScanType = scanTypes[i % scanTypes.Length],
        ScanLocation = "Vienna Hub",
        ScannedAt = DateTimeOffset.UtcNow
    };

    var message = new ServiceBusMessage(JsonSerializer.Serialize(body, options))
    {
        Subject = "ParcelScanned",
        SessionId = trackingNumber,
        ContentType = "application/json"
    };

    await sender.SendMessageAsync(message);
}

Console.WriteLine($"Sent {count} ParcelScanned messages to parcel-events");
