using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Contoso.Contracts;
using Microsoft.Azure.Cosmos;

public class ShipmentBookedProcessor(ServiceBusClient serviceBus, CosmosClient cosmos) : BackgroundService
{
    private static readonly ActivitySource ActivitySource = new("Contoso.Tracking");

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var container = cosmos.GetContainer("logistics", "tracking");
        await using var processor = serviceBus.CreateProcessor("shipment-events", "tracking", new ServiceBusProcessorOptions { AutoCompleteMessages = false });

        processor.ProcessMessageAsync += async args =>
        {
            var booked = JsonSerializer.Deserialize<ShipmentBooked>(args.Message.Body.ToString())!;

            using var activity = ActivitySource.StartActivity("ProjectShipmentBooked");

            await container.UpsertItemAsync(
                new { id = booked.TrackingNumber, trackingNumber = booked.TrackingNumber, shipmentId = booked.ShipmentId },
                new PartitionKey(booked.TrackingNumber),
                cancellationToken: args.CancellationToken);

            await args.CompleteMessageAsync(args.Message, args.CancellationToken);
        };

        processor.ProcessErrorAsync += args => Task.CompletedTask;

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
}
