using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;

public sealed class PriorityDispatcher(ServiceBusClient client, DurableTaskClient durableClient, ILogger<PriorityDispatcher> logger) : BackgroundService
{
    public const string UrgentQueue = "exceptions-urgent";
    public const string BulkQueue = "exceptions-bulk";

    private static readonly TimeSpan UrgentWait = TimeSpan.FromSeconds(1);
    private static readonly TimeSpan BulkWait = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var urgent = client.CreateReceiver(UrgentQueue);
        await using var bulk = client.CreateReceiver(BulkQueue);

        while (!stoppingToken.IsCancellationRequested)
        {
            var receiver = urgent;
            var message = await urgent.ReceiveMessageAsync(UrgentWait, stoppingToken);
            if (message is null)
            {
                receiver = bulk;
                message = await bulk.ReceiveMessageAsync(BulkWait, stoppingToken);
            }

            if (message is null)
            {
                continue;
            }

            var input = message.Body.ToObjectFromJson<DelayedParcelInput>(JsonSerializerOptions.Web)!;

            logger.LogInformation("Dispatching {Queue} message for event {EventId}", receiver.EntityPath, input.EventId);

            if (await durableClient.GetInstanceAsync(input.EventId, stoppingToken) is null)
            {
                await durableClient.ScheduleNewOrchestrationInstanceAsync(
                    nameof(DelayedParcelOrchestration),
                    input,
                    new StartOrchestrationOptions { InstanceId = input.EventId },
                    stoppingToken);
            }

            await receiver.CompleteMessageAsync(message, stoppingToken);
        }
    }
}
