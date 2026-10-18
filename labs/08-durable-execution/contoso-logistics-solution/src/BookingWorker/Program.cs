using BookingWorker.Activities;
using BookingWorker.Ledger;
using BookingWorker.Sagas;
using Microsoft.DurableTask.Client;
using Microsoft.DurableTask.Client.AzureManaged;
using Microsoft.DurableTask.Worker;
using Microsoft.DurableTask.Worker.AzureManaged;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var connectionString = builder.Configuration["DURABLE_TASK_SCHEDULER_CONNECTION_STRING"]
    ?? throw new InvalidOperationException("DURABLE_TASK_SCHEDULER_CONNECTION_STRING is not set");
var taskHubName = builder.Configuration["TASKHUB_NAME"]
    ?? throw new InvalidOperationException("TASKHUB_NAME is not set");
var schedulerConnectionString = $"{connectionString};TaskHub={taskHubName}";

builder.Services.AddSingleton<IBookingLedger>(
    new FileBookingLedger(Path.Combine(builder.Environment.ContentRootPath, "data", "booking-ledger.jsonl")));

builder.Services.AddDaprClient();

builder.Services.AddDurableTaskWorker(worker =>
{
    worker.AddTasks(tasks =>
    {
        tasks.AddOrchestrator<BookingSagaOrchestrator>();
        tasks.AddOrchestrator<DeliveryDeadlineOrchestrator>();
        tasks.AddActivity<ReserveCapacity>();
        tasks.AddActivity<CapturePayment>();
        tasks.AddActivity<PrintLabel>();
        tasks.AddActivity<ReleaseCapacity>();
        tasks.AddActivity<RefundPayment>();
        tasks.AddActivity<VoidLabel>();
        tasks.AddActivity<RaiseDeliveryDelayed>();
        tasks.AddActivity<RerouteParcel>();
    });
    worker.UseDurableTaskScheduler(schedulerConnectionString);
    worker.Configure(options => options.Concurrency.MaximumConcurrentActivityWorkItems = 1);
});
builder.Services.AddDurableTaskClient(client => client.UseDurableTaskScheduler(schedulerConnectionString));

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapPost("/bookings", async (BookingRequest request, DurableTaskClient client) =>
{
    var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(BookingSagaOrchestrator), request.ParcelId);
    return Results.Ok(instanceId);
});

app.MapGet("/bookings/{instanceId}", async (string instanceId, DurableTaskClient client) =>
{
    var instance = await client.GetInstanceAsync(instanceId, getInputsAndOutputs: true);
    return instance is null
        ? Results.NotFound()
        : Results.Ok(new { status = instance.RuntimeStatus.ToString(), output = instance.ReadOutputAs<string>() });
});

app.MapPost("/deliveries", async (DeliveryRequest request, DurableTaskClient client) =>
{
    var input = new DeliveryWatchInput(request.ParcelId, request.WindowSeconds);
    var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(DeliveryDeadlineOrchestrator), input);
    return Results.Ok(instanceId);
});

app.MapGet("/deliveries/{instanceId}", async (string instanceId, DurableTaskClient client) =>
{
    var instance = await client.GetInstanceAsync(instanceId, getInputsAndOutputs: true);
    return instance is null
        ? Results.NotFound()
        : Results.Ok(new { status = instance.RuntimeStatus.ToString(), output = instance.ReadOutputAs<string>() });
});

app.MapPost("/deliveries/{instanceId}/reroute-approval", async (string instanceId, DurableTaskClient client) =>
{
    var instance = await client.GetInstanceAsync(instanceId);
    if (instance is null)
    {
        return Results.NotFound();
    }

    await client.RaiseEventAsync(instanceId, "RerouteApproved", true);
    return Results.Accepted();
});

app.MapGet("/ledger/{parcelId}", async (string parcelId, IBookingLedger ledger) =>
    Results.Ok(await ledger.ReadActivitiesAsync(parcelId)));

app.Run();

public sealed record BookingRequest(string ParcelId);

public sealed record DeliveryRequest(string ParcelId, int WindowSeconds);
