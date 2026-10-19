using Microsoft.Azure.Cosmos;
using StackExchange.Redis;

public class TrackingProjectionHost(CosmosClient cosmos, TrackingContainerSetup setup, IDatabase cache, IConfiguration configuration) : BackgroundService
{
    private ChangeFeedProcessor? processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var database = cosmos.GetDatabase("logistics");
        var tracking = await setup.EnsureAsync(database, stoppingToken);
        var leases = await database.CreateContainerIfNotExistsAsync("leases", "/id", cancellationToken: stoppingToken);
        var projection = new TrackingProjection(tracking, cache);

        processor = database.GetContainer("shipments")
            .GetChangeFeedProcessorBuilder<ShipmentEventDocument>("tracking-projection", projection.HandleChangesAsync)
            .WithInstanceName(configuration["Tracking:InstanceName"] ?? Environment.MachineName)
            .WithLeaseContainer(leases.Container)
            .WithStartTime(DateTime.MinValue.ToUniversalTime())
            .Build();

        await processor.StartAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        if (processor is not null)
        {
            await processor.StopAsync();
        }
    }
}
