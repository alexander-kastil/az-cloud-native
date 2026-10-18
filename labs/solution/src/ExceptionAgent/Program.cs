using Contoso.Contracts;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.AzureManaged;
using Microsoft.DurableTask.Worker;
using Microsoft.DurableTask.Worker.AzureManaged;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();
builder.AddAzureServiceBusClient("messaging");
builder.AddAzureCosmosClient("cosmos", configureClientOptions: options =>
    options.UseSystemTextJsonSerializerWithOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web));
builder.AddExceptionAgentChatClient();

var connectionString = builder.Configuration["DURABLE_TASK_SCHEDULER_CONNECTION_STRING"]
    ?? throw new InvalidOperationException("DURABLE_TASK_SCHEDULER_CONNECTION_STRING is not set");
var taskHubName = builder.Configuration["TASKHUB_NAME"]
    ?? throw new InvalidOperationException("TASKHUB_NAME is not set");
var schedulerConnectionString = $"{connectionString};TaskHub={taskHubName}";

builder.Services.AddDurableTaskWorker(worker =>
{
    worker.AddTasks(tasks =>
    {
        tasks.AddOrchestrator<DelayedParcelOrchestration>();
        tasks.AddActivity<ReadProcessedDraft>();
        tasks.AddActivity<DraftCustomerMessage>();
        tasks.AddActivity<SaveRerouteProposal>();
        tasks.AddActivity<PublishRerouteRequested>();
    });
    worker.UseDurableTaskScheduler(schedulerConnectionString);
    worker.Configure(options => options.Concurrency.MaximumConcurrentActivityWorkItems = 1);
});
builder.Services.AddDurableTaskClient(client => client.UseDurableTaskScheduler(schedulerConnectionString));
builder.Services.AddHostedService<DelayedParcelWorker>();
builder.Services.AddHostedService<PriorityDispatcher>();

builder.Build().Run();

public sealed class DelayedParcelWorker(ServiceBusClient client, ILogger<DelayedParcelWorker> logger) : BackgroundService
{
    private const string Topic = "parcel-events";
    private const string Subscription = "exception-agent";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var processor = client.CreateProcessor(Topic, Subscription, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10)
        });

        processor.ProcessMessageAsync += HandleAsync;
        processor.ProcessErrorAsync += args =>
        {
            logger.LogError(args.Exception, "Service Bus error in {Source}", args.ErrorSource);
            return Task.CompletedTask;
        };

        await processor.StartProcessingAsync(stoppingToken);
        await Task.Delay(Timeout.Infinite, stoppingToken).ContinueWith(_ => { });
        await processor.StopProcessingAsync();
    }

    private async Task HandleAsync(ProcessMessageEventArgs args)
    {
        using var envelope = JsonDocument.Parse(args.Message.Body);
        var eventId = envelope.RootElement.GetProperty("id").GetString()!;
        var delayed = envelope.RootElement.GetProperty("data").Deserialize<DeliveryDelayed>(JsonSerializerOptions.Web)!;

        logger.LogInformation("DeliveryDelayed event {EventId} for parcel {ParcelId}", eventId, delayed.TrackingNumber);

        var urgent = args.Message.ApplicationProperties.TryGetValue("Priority", out var priority)
            && string.Equals(priority as string, "urgent", StringComparison.Ordinal);
        var queue = urgent ? PriorityDispatcher.UrgentQueue : PriorityDispatcher.BulkQueue;

        var input = new DelayedParcelInput(eventId, delayed.ShipmentId, delayed.TrackingNumber);
        await using var sender = client.CreateSender(queue);
        await sender.SendMessageAsync(new ServiceBusMessage(BinaryData.FromObjectAsJson(input, JsonSerializerOptions.Web)), args.CancellationToken);

        await args.CompleteMessageAsync(args.Message, args.CancellationToken);
    }
}
