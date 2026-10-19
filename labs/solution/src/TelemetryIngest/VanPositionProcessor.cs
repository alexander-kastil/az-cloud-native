using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;
using Grey.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace TelemetryIngest;

public class VanPositionProcessor(
    IConfiguration configuration,
    IHubContext<VanPositionHub> hub,
    ConnectedClients clients,
    ILogger<VanPositionProcessor> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var checkpoints = new BlobContainerClient(configuration.GetConnectionString("blobs"), "ops-console");
        await checkpoints.CreateIfNotExistsAsync(cancellationToken: stoppingToken);

        var processor = new EventProcessorClient(
            checkpoints,
            "ops-console",
            configuration.GetConnectionString("event-hubs"),
            "van-positions");
        processor.PartitionInitializingAsync += OnPartitionInitializingAsync;
        processor.ProcessEventAsync += OnEventAsync;
        processor.ProcessErrorAsync += OnErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);
        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
        }

        await processor.StopProcessingAsync();
    }

    private static Task OnPartitionInitializingAsync(PartitionInitializingEventArgs args)
    {
        args.DefaultStartingPosition = EventPosition.Earliest;
        return Task.CompletedTask;
    }

    private async Task OnEventAsync(ProcessEventArgs args)
    {
        if (!args.HasEvent)
        {
            return;
        }

        await clients.WaitForAnyAsync(args.CancellationToken);
        var position = JsonSerializer.Deserialize<VanPositionReported>(args.Data.EventBody.ToMemory().Span, JsonSerializerOptions.Web);
        await hub.Clients.All.SendAsync("vanPositionReported", position, args.CancellationToken);
        await args.UpdateCheckpointAsync(args.CancellationToken);
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception, "Processor error on partition {PartitionId} during {Operation}", args.PartitionId, args.Operation);
        return Task.CompletedTask;
    }
}
