using Microsoft.Azure.Cosmos;

public class OutboxRelayHost(CosmosClient cosmos, OutboxRelay relay, IConfiguration configuration) : BackgroundService
{
    private ChangeFeedProcessor? processor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (configuration["Outbox:RelayEnabled"] != "true")
        {
            return;
        }

        var database = cosmos.GetDatabase("logistics");
        var leases = await database.CreateContainerIfNotExistsAsync("leases", "/id", cancellationToken: stoppingToken);

        processor = database.GetContainer("shipments")
            .GetChangeFeedProcessorBuilder<ShipmentDocument>("outbox-relay", relay.HandleChangesAsync)
            .WithInstanceName(configuration["Outbox:InstanceName"] ?? Environment.MachineName)
            .WithLeaseContainer(leases.Container)
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
