using Azure.Core;
using Azure.Messaging;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Grey.Contracts;

namespace LabelService;

public class ProofOfDeliveryWorker : BackgroundService
{
    private const string BlobCreated = "Microsoft.Storage.BlobCreated";
    private const string ContainerMarker = "/containers/proof-of-delivery/";
    private const string BlobMarker = "/blobs/";

    private readonly QueueClient queue;
    private readonly ParcelDeliveredPublisher publisher;
    private readonly ILogger<ProofOfDeliveryWorker> logger;

    public ProofOfDeliveryWorker(IConfiguration configuration, TokenCredential credential, ParcelDeliveredPublisher publisher, ILogger<ProofOfDeliveryWorker> logger)
    {
        var account = configuration["Azure:Storage:AccountName"]!;
        queue = new QueueClient(new Uri($"https://{account}.queue.core.windows.net/pod-events"), credential, new QueueClientOptions { MessageEncoding = QueueMessageEncoding.Base64 });
        this.publisher = publisher;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var messages = await queue.ReceiveMessagesAsync(16, TimeSpan.FromMinutes(1), stoppingToken);
                foreach (var message in messages.Value)
                {
                    await HandleAsync(message, stoppingToken);
                }

                if (messages.Value.Length == 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Reading pod-events failed");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleAsync(QueueMessage message, CancellationToken cancellationToken)
    {
        var cloudEvent = CloudEvent.Parse(message.Body);
        var trackingNumber = TrackingNumberOf(cloudEvent);
        if (trackingNumber is not null)
        {
            var delivered = new ParcelDelivered(Guid.Empty, trackingNumber, "Delivered", string.Empty, cloudEvent!.Time ?? DateTimeOffset.UtcNow);
            await publisher.PublishAsync(delivered, cancellationToken);
            logger.LogInformation("Published ParcelDelivered for {TrackingNumber}", trackingNumber);
        }

        await queue.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
    }

    private static string? TrackingNumberOf(CloudEvent? cloudEvent)
    {
        var subject = cloudEvent?.Subject;
        if (cloudEvent!.Type != BlobCreated || subject is null || !subject.Contains(ContainerMarker))
        {
            return null;
        }

        var blobIndex = subject.IndexOf(BlobMarker, StringComparison.Ordinal);
        if (blobIndex < 0)
        {
            return null;
        }

        var name = Path.GetFileNameWithoutExtension(subject[(blobIndex + BlobMarker.Length)..]);
        return name.Length == 0 ? null : name;
    }
}
