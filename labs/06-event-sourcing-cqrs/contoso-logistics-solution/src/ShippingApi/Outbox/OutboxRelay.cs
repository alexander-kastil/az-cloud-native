using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public class OutboxRelay(ServiceBusSender sender)
{
    public async Task HandleChangesAsync(
        ChangeFeedProcessorContext context,
        IReadOnlyCollection<ShipmentDocument> changes,
        CancellationToken cancellationToken)
    {
        foreach (var change in changes.Where(change => change.documentType == "outbox"))
        {
            var message = new ServiceBusMessage(BinaryData.FromString(JsonConvert.SerializeObject(change.data)))
            {
                MessageId = change.id,
                SessionId = change.shipmentId,
                Subject = change.eventType,
                ContentType = "application/json"
            };

            await sender.SendMessageAsync(message, cancellationToken);
        }
    }
}
