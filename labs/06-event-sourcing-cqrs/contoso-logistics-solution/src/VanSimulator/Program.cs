using System.Text.Json;
using System.Text.RegularExpressions;
using Confluent.Kafka;
using Contoso.Contracts;

var bootstrapServers = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS") ?? "localhost:9092";
var connectionString = Regex.Replace(
    Environment.GetEnvironmentVariable("ConnectionStrings__event-hubs")!,
    "Endpoint=sb://[^;]+",
    $"Endpoint=sb://{bootstrapServers}");

var config = new ProducerConfig
{
    BootstrapServers = bootstrapServers,
    SecurityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism = SaslMechanism.Plain,
    SaslUsername = "$ConnectionString",
    SaslPassword = connectionString
};

string[] vans = ["van-07", "van-12", "van-21"];
var sequences = vans.ToDictionary(van => van, _ => 0L);

using var producer = new ProducerBuilder<string, string>(config).Build();
using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

while (await timer.WaitForNextTickAsync())
{
    foreach (var van in vans)
    {
        var position = new VanPositionReported(van, ++sequences[van], 48.2082, 16.3738, DateTimeOffset.UtcNow);
        await producer.ProduceAsync("van-positions", new Message<string, string>
        {
            Key = van,
            Value = JsonSerializer.Serialize(position)
        });
    }
}
