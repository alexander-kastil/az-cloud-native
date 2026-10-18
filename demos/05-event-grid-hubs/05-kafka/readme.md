---
minutes: 10
---

# Event Hubs: Kafka-Compatible Streaming

Several Contoso Logistics telematics vendors already publish to Kafka, and the telemetry-ingest service has to accept those streams without a rewrite. Event Hubs exposes a Kafka endpoint on the same namespace, so a Kafka client keeps its code and changes its connection settings. The work is to map the Kafka concepts onto Event Hubs and to choose the right tier and authentication.

This topic covers the endpoint and the concept mapping, the tiers that support Kafka, the SASL settings for PLAIN and OAUTHBEARER, a Confluent.Kafka producer for van GPS positions, and the Kafka features that remain in preview.

## The Kafka Endpoint and Concept Mapping

The Kafka endpoint uses the namespace host name on port 9093 and accepts Kafka clients from version 1.0 onward. Event Hubs serves the Kafka protocol next to AMQP and HTTPS, so a Kafka producer and an AMQP consumer can share the same event hub. Event Hubs is managed, so no brokers exist to configure.

A Kafka topic maps to an event hub, and the namespace takes the place of the cluster. Partitions and consumer groups keep their Kafka meaning, and the consumer group is identified by the `group.id` setting on the client.

| Apache Kafka | Azure Event Hubs |
| --- | --- |
| Cluster of brokers | Namespace with one endpoint on port 9093 |
| Topic | Event hub |
| Partition | Partition |
| Consumer group, set by `group.id` | Consumer group |
| Broker and topic configuration | Managed by Event Hubs, set in the portal, the Azure CLI or ARM templates |
| Kafka AdminClient topic management | Azure portal, Azure CLI or ARM templates |

The `group.id` value can be up to 256 characters long.

## Tier Support

Event Hubs supports the Kafka endpoint on the Standard, Premium and Dedicated tiers. The Basic tier doesn't support Kafka, and a TopicAuthorizationException on a Basic namespace is one of the first things to check. Learn recommends the Premium or Dedicated tier for full Kafka protocol compatibility, because the Standard tier has limitations that can affect some Kafka workloads.

| Tier | Kafka endpoint | Typical use |
| --- | --- | --- |
| Basic | Not supported | Not suitable for Kafka clients |
| Standard | Supported, with limitations | Development and low-throughput tests |
| Premium | Supported | Production Kafka workloads |
| Dedicated | Supported | High throughput and large messages |

Event Hubs accepts only gzip compression on Kafka producers, so set `compression.type` to `gzip` and avoid snappy, lz4 and zstd.

## SASL Configuration with PLAIN and OAUTHBEARER

Every Kafka client connects over TLS with SASL. PLAIN sends a connection string as the password, and OAUTHBEARER sends a Microsoft Entra ID token. Both use the `SASL_SSL` security protocol, and the PLAIN form works with a connection string from the namespace.

```properties
bootstrap.servers=contoso-telemetry.servicebus.windows.net:9093
security.protocol=SASL_SSL
sasl.mechanism=PLAIN
sasl.jaas.config=org.apache.kafka.common.security.plain.PlainLoginModule required username="$ConnectionString" password="<your-connection-string>";
```

The OAUTHBEARER form uses the same endpoint and protocol, with a different mechanism and a callback class that you write. The callback class obtains the Entra ID token for the client identity.

```properties
bootstrap.servers=contoso-telemetry.servicebus.windows.net:9093
security.protocol=SASL_SSL
sasl.mechanism=OAUTHBEARER
sasl.jaas.config=org.apache.kafka.common.security.oauthbearer.OAuthBearerLoginModule required;
sasl.login.callback.handler.class=CustomAuthenticateCallbackHandler
```

The Learn configuration for OAUTHBEARER is written for Java clients, and the callback class is your own code.

The identity needs the Azure Event Hubs Data Sender role to produce and the Azure Event Hubs Data Receiver role to consume. The Kafka endpoint rejects generated shared access signature tokens, so use a connection string for PLAIN or an Entra ID token for OAUTHBEARER. Prefer OAUTHBEARER for production, because it removes the connection string from the client configuration.

## Producing Van GPS Positions with Confluent.Kafka

Confluent.Kafka accepts the same librdkafka configuration keys as the properties files above. The producer builder takes the keys as a dictionary, and the message key decides the partition, so every position of one van stays in order.

```csharp
using System.Text.Json;
using Confluent.Kafka;

var config = new Dictionary<string, string>
{
    ["bootstrap.servers"] = "contoso-telemetry.servicebus.windows.net:9093",
    ["security.protocol"] = "SASL_SSL",
    ["sasl.mechanism"] = "PLAIN",
    ["sasl.username"] = "$ConnectionString",
    ["sasl.password"] = Environment.GetEnvironmentVariable("EVENTHUBS_CONNECTION_STRING"),
    ["compression.type"] = "gzip"
};

using var producer = new ProducerBuilder<string, string>(config).Build();

var position = new VanPosition("van-07", 48.2082, 16.3738, DateTimeOffset.UtcNow);

var message = new Message<string, string>
{
    Key = position.VanId,
    Value = JsonSerializer.Serialize(position)
};

await producer.ProduceAsync("van-positions", message);

public record VanPosition(string VanId, double Latitude, double Longitude, DateTimeOffset ObservedAt);
```

The Kafka topic name in `ProduceAsync` is the event hub name. For OAUTHBEARER in .NET, Confluent.Kafka exposes a token refresh hook on its builders, and the Confluent.Kafka documentation describes that hook for the consumer builder. Use the same pattern for the producer after you check the producer overload in the version you install.

## Preview Features: Kafka Streams and Transactions

Kafka Streams is a Kafka client library for stream processing, and Event Hubs supports it on the Premium and Dedicated tiers as a public preview. Learn notes that Kafka Streams has fewer built-in capabilities than services such as Azure Stream Analytics, and it lists Stream Analytics as an alternative for stream processing.

Learn lists Kafka transactions as a public preview on the Premium and Dedicated tiers. The client configuration article still lists transactions and exactly-once semantics as unsupported, so the status of both features conflicts across Learn pages. Check the current status before the class depends on either feature, and treat both as preview until the documentation agrees.

## Key Topics

- [What is Azure Event Hubs for Apache Kafka?](https://learn.microsoft.com/azure/event-hubs/azure-event-hubs-apache-kafka-overview)
- [Apache Kafka client configurations for Azure Event Hubs](https://learn.microsoft.com/azure/event-hubs/apache-kafka-configurations)
- [Apache Kafka troubleshooting guide for Event Hubs](https://learn.microsoft.com/azure/event-hubs/apache-kafka-troubleshooting-guide)
- [Migrate to Azure Event Hubs for Apache Kafka](https://learn.microsoft.com/azure/event-hubs/apache-kafka-migration-guide)
- [Quickstart: Data streaming with Event Hubs using the Kafka protocol](https://learn.microsoft.com/azure/event-hubs/event-hubs-quickstart-kafka-enabled-event-hubs)
