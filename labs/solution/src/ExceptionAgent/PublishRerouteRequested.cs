using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.DurableTask;

public sealed record RerouteRequestedMessage(string EventId, string ParcelId, Guid ShipmentId);

public sealed class PublishRerouteRequested(ServiceBusClient client) : TaskActivity<DelayedParcelInput, bool>
{
    public override async Task<bool> RunAsync(TaskActivityContext context, DelayedParcelInput input)
    {
        await using var sender = client.CreateSender("parcel-events");

        var body = new RerouteRequestedMessage(input.EventId, input.ParcelId, input.ShipmentId);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(body, JsonSerializerOptions.Web))
        {
            MessageId = $"reroute-requested-{input.EventId}",
            Subject = "RerouteRequested",
            ContentType = "application/json"
        };

        await sender.SendMessageAsync(message);
        return true;
    }
}
