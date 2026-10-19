using Azure.Core;
using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Grey.Contracts;

namespace LabelService;

public class ParcelDeliveredPublisher
{
    private readonly EventGridPublisherClient client;

    public ParcelDeliveredPublisher(Uri topicEndpoint, TokenCredential credential)
    {
        client = new EventGridPublisherClient(topicEndpoint, credential);
    }

    public Task PublishAsync(ParcelDelivered delivered, CancellationToken cancellationToken)
    {
        var cloudEvent = new CloudEvent("/label-service", "ParcelDelivered", delivered);
        return client.SendEventAsync(cloudEvent, cancellationToken);
    }
}
