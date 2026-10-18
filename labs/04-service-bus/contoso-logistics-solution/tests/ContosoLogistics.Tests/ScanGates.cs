using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Aspire.Hosting;
using Aspire.Hosting.Testing;
using Azure.Messaging.ServiceBus;
using Contoso.Contracts;
using Microsoft.Azure.Cosmos;

namespace ContosoLogistics.Tests;

internal static class ScanKit
{
    public const string Topic = "parcel-events";
    public static readonly TimeSpan Startup = TimeSpan.FromMinutes(10);
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };

    public static ServiceBusMessage Scan(string trackingNumber, ScanType scanType)
    {
        var scan = new ParcelScanned(Guid.NewGuid(), Guid.NewGuid(), trackingNumber, scanType, "Vienna Hub", DateTimeOffset.UtcNow);
        return new ServiceBusMessage(JsonSerializer.Serialize(scan, Json))
        {
            Subject = "ParcelScanned",
            SessionId = trackingNumber,
            MessageId = Guid.NewGuid().ToString(),
            ContentType = "application/json"
        };
    }

    public static async Task<ServiceBusClient> BusAsync(DistributedApplication app) =>
        new(await app.GetConnectionStringAsync("messaging"));

    public static async Task<Container> TrackingAsync(DistributedApplication app)
    {
        var client = new CosmosClient(await app.GetConnectionStringAsync("cosmos"), new CosmosClientOptions
        {
            ConnectionMode = ConnectionMode.Gateway,
            LimitToEndpoint = true,
            HttpClientFactory = () => new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            })
        });
        return client.GetContainer("logistics", "tracking");
    }

    public static async Task<List<string>> ScansAsync(Container tracking, string trackingNumber)
    {
        using var response = await tracking.ReadItemStreamAsync($"parcel-{trackingNumber}", new PartitionKey(trackingNumber));
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return [];
        }

        response.EnsureSuccessStatusCode();
        using var document = await JsonDocument.ParseAsync(response.Content);
        if (!document.RootElement.TryGetProperty("scans", out var scans))
        {
            return [];
        }

        return scans.EnumerateArray()
            .Select(s => s.ValueKind == JsonValueKind.String
                ? s.GetString()!
                : s.EnumerateObject().First(p => p.Name.Equals("scanType", StringComparison.OrdinalIgnoreCase)).Value.ToString())
            .ToList();
    }

    public static async Task<List<string>> WaitForScansAsync(Container tracking, string trackingNumber, int count)
    {
        var deadline = DateTime.UtcNow.AddSeconds(120);
        var scans = new List<string>();
        while (DateTime.UtcNow < deadline)
        {
            scans = await ScansAsync(tracking, trackingNumber);
            if (scans.Count >= count)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));
                return await ScansAsync(tracking, trackingNumber);
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        return scans;
    }
}

public sealed class BrokerOnlyFixture : IAsyncLifetime
{
    public DistributedApplication App { get; private set; } = default!;

    public async Task InitializeAsync()
    {
        var builder = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ContosoLogistics_AppHost>();
        foreach (var consumer in builder.Resources.Where(r => r.Name.Contains("scan", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            builder.Resources.Remove(consumer);
        }

        App = await builder.BuildAsync();
        await App.StartAsync().WaitAsync(ScanKit.Startup);
    }

    public async Task DisposeAsync() => await App.DisposeAsync();
}

public class ScanRoutingGates(BrokerOnlyFixture fixture) : IClassFixture<BrokerOnlyFixture>
{
    [Fact]
    [Trait("Gate", "FilteredSubscriptionReceivesOnlyItsEvents")]
    public async Task FilteredSubscriptionReceivesOnlyItsEvents()
    {
        await fixture.App.ResourceNotifications.WaitForResourceHealthyAsync("messaging").WaitAsync(ScanKit.Startup);
        var run = Guid.NewGuid().ToString("N");
        await using var bus = await ScanKit.BusAsync(fixture.App);
        await using var sender = bus.CreateSender(ScanKit.Topic);
        foreach (var subject in new[] { "ParcelScanned", "ParcelDelivered", "DeliveryDelayed" })
        {
            var message = new ServiceBusMessage("{}") { Subject = subject, SessionId = $"route-{run}", MessageId = $"{subject}-{run}" };
            message.ApplicationProperties["run"] = run;
            await sender.SendMessageAsync(message);
        }

        Assert.Equal(["ParcelScanned"], await DrainAsync(bus, "tracking", run));
        Assert.Equal(["DeliveryDelayed", "ParcelDelivered"], (await DrainAsync(bus, "customer-messages", run)).Order().ToList());
        Assert.Equal(["DeliveryDelayed"], await DrainAsync(bus, "exceptions", run));
    }

    private static async Task<List<string>> DrainAsync(ServiceBusClient bus, string subscription, string run)
    {
        ServiceBusReceiver receiver;
        try
        {
            receiver = await bus.AcceptSessionAsync(ScanKit.Topic, subscription, $"route-{run}");
        }
        catch (Exception)
        {
            receiver = bus.CreateReceiver(ScanKit.Topic, subscription);
        }

        var subjects = new List<string>();
        await using (receiver)
        {
            var empty = 0;
            while (empty < 2)
            {
                var batch = await receiver.ReceiveMessagesAsync(10, TimeSpan.FromSeconds(5));
                empty = batch.Count == 0 ? empty + 1 : 0;
                foreach (var message in batch)
                {
                    if (message.ApplicationProperties.TryGetValue("run", out var value) && Equals(value, run))
                    {
                        subjects.Add(message.Subject);
                    }

                    await receiver.CompleteMessageAsync(message);
                }
            }
        }

        return subjects;
    }
}

public class ScanProcessingGates(AppHostFixture fixture) : IClassFixture<AppHostFixture>
{
    private DistributedApplication App => fixture.App;

    [Fact]
    [Trait("Gate", "ScansOfOneParcelStayInOrder")]
    public async Task ScansOfOneParcelStayInOrder()
    {
        await App.ResourceNotifications.WaitForResourceHealthyAsync("messaging").WaitAsync(ScanKit.Startup);
        var a = $"CL-ORD-A-{Random.Shared.Next(100000, 999999)}";
        var b = $"CL-ORD-B-{Random.Shared.Next(100000, 999999)}";
        await using var bus = await ScanKit.BusAsync(App);
        await using var sender = bus.CreateSender(ScanKit.Topic);
        foreach (var type in new[] { ScanType.PickedUp, ScanType.AtHub, ScanType.OutForDelivery })
        {
            await sender.SendMessageAsync(ScanKit.Scan(a, type));
            await sender.SendMessageAsync(ScanKit.Scan(b, type));
        }

        var tracking = await ScanKit.TrackingAsync(App);
        Assert.Equal(["PickedUp", "AtHub", "OutForDelivery"], await ScanKit.WaitForScansAsync(tracking, a, 3));
        Assert.Equal(["PickedUp", "AtHub", "OutForDelivery"], await ScanKit.WaitForScansAsync(tracking, b, 3));
    }

    [Fact]
    [Trait("Gate", "UnreadableScanDeadLettered")]
    public async Task UnreadableScanDeadLettered()
    {
        await App.ResourceNotifications.WaitForResourceHealthyAsync("messaging").WaitAsync(ScanKit.Startup);
        var c = $"CL-DLQ-{Random.Shared.Next(100000, 999999)}";
        await using var bus = await ScanKit.BusAsync(App);
        await using var sender = bus.CreateSender(ScanKit.Topic);
        var unreadable = new ServiceBusMessage("label unreadable") { Subject = "ParcelScanned", SessionId = c, MessageId = $"bad-{c}" };
        await sender.SendMessageAsync(ScanKit.Scan(c, ScanType.PickedUp));
        await sender.SendMessageAsync(unreadable);
        await sender.SendMessageAsync(ScanKit.Scan(c, ScanType.AtHub));

        var tracking = await ScanKit.TrackingAsync(App);
        Assert.Equal(["PickedUp", "AtHub"], await ScanKit.WaitForScansAsync(tracking, c, 2));

        await using var deadLetters = bus.CreateReceiver(ScanKit.Topic, "tracking", new ServiceBusReceiverOptions { SubQueue = SubQueue.DeadLetter });
        ServiceBusReceivedMessage? found = null;
        var deadline = DateTime.UtcNow.AddSeconds(60);
        while (found is null && DateTime.UtcNow < deadline)
        {
            foreach (var message in await deadLetters.PeekMessagesAsync(100))
            {
                if (message.MessageId == $"bad-{c}")
                {
                    found = message;
                }
            }

            if (found is null)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        Assert.NotNull(found);
        Assert.Equal("UnreadableLabel", found!.DeadLetterReason);
    }

    [Fact]
    [Trait("Gate", "DuplicateScanStoredOnce")]
    public async Task DuplicateScanStoredOnce()
    {
        await App.ResourceNotifications.WaitForResourceHealthyAsync("cosmos").WaitAsync(ScanKit.Startup);
        var tracking = await ScanKit.TrackingAsync(App);
        var store = CreateStore(tracking);
        var append = store.GetType().GetMethod("TryAppendAsync", [typeof(string), typeof(string), typeof(string)])!;
        var trackingNumber = $"CL-DUP-{Random.Shared.Next(100000, 999999)}";
        var messageId = Guid.NewGuid().ToString();

        var first = await (Task<bool>)append.Invoke(store, [trackingNumber, "PickedUp", messageId])!;
        var second = await (Task<bool>)append.Invoke(store, [trackingNumber, "PickedUp", messageId])!;

        Assert.True(first);
        Assert.False(second);
        Assert.Equal(["PickedUp"], await ScanKit.ScansAsync(tracking, trackingNumber));

        var markers = tracking.GetItemQueryIterator<int>(
            new QueryDefinition("SELECT VALUE COUNT(1) FROM c WHERE c.id = @id").WithParameter("@id", $"processed-{messageId}"),
            requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(trackingNumber) });
        var count = 0;
        while (markers.HasMoreResults)
        {
            count += (await markers.ReadNextAsync()).Sum();
        }

        Assert.Equal(1, count);
    }

    private static object CreateStore(Container tracking)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "ContosoLogistics.sln")))
        {
            root = root.Parent;
        }

        var assemblyPath = Directory.GetFiles(Path.Combine(root!.FullName, "src", "ScanProcessor", "bin"), "ScanProcessor.dll", SearchOption.AllDirectories)
            .OrderByDescending(File.GetLastWriteTimeUtc)
            .First();
        var assembly = Assembly.LoadFrom(assemblyPath);
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            types = e.Types.Where(t => t is not null).ToArray()!;
        }

        var storeType = types.First(t => t.Name == "ParcelScanStore");
        return Activator.CreateInstance(storeType, tracking)!;
    }
}
