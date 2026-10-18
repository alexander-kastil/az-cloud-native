---
slide: "05-01"
topic: 05-kafka
title: "The Kafka endpoint of Event Hubs"
subtitle: "Kafka clients keep their code and change their connection settings"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/05-event-grid-hubs/_images/kafka-keyed-partitions.svg"
media-file: labs/05-event-grid-hubs/_images/kafka-keyed-partitions.svg
visual-prompt: >
  Existing diagram of VanSimulator, a Confluent.Kafka producer, sending the positions of van-07, van-12 and van-21 keyed by VanId to port 9092 of the event-hubs emulator, where the hashed key puts each van's positions in order into one of the four partitions of van-positions, no generation. Referenced at its repo path.
order: 12
source: demos/05-event-grid-hubs/05-kafka/readme.md
gamma_prompt: >
  Slide titled "The Kafka endpoint of Event Hubs", subtitle "Kafka clients keep their code and change
  their connection settings". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The Kafka endpoint of Event Hubs

## Content

- Port 9093 on the namespace (9092 in the emulator); a Kafka topic is an **event hub**
- Standard, Premium and Dedicated tiers; Basic has no Kafka endpoint
- `SASL_SSL` with **PLAIN** and a connection string, or **OAUTHBEARER** and an Entra ID token
- The message key picks the partition, so each van's positions stay in order
- Only gzip compression; treat Kafka Streams and transactions as preview

## Speaker Notes

Several Contoso Logistics telematics vendors already publish to Kafka, and telemetry-ingest has to accept those streams without a rewrite. The diagram shows the lab's van simulator producing keyed positions into the four partitions of `van-positions`.

Endpoint and mapping: the namespace takes the place of the cluster, a Kafka topic maps to an event hub, and partitions and consumer groups keep their Kafka meaning. Event Hubs serves Kafka next to AMQP and HTTPS, so a Kafka producer and an AMQP consumer can share one event hub.

Tiers: Basic doesn't support Kafka, and a TopicAuthorizationException on a Basic namespace is one of the first things to check. Learn recommends Premium or Dedicated for full Kafka protocol compatibility.

Authentication: PLAIN sends `$ConnectionString` as the user name and the connection string as the password, OAUTHBEARER sends an Entra ID token, and both use `SASL_SSL`. Prefer OAUTHBEARER in production; the lab's emulator listens on port 9092 with `SASL_PLAINTEXT` and no TLS.

Keyed producer: Confluent.Kafka takes the librdkafka keys as a dictionary, and the message key `VanId` decides the partition. The Kafka topic name in `ProduceAsync` is the event hub name.

Compression and preview: Event Hubs accepts only gzip on Kafka producers, so avoid snappy, lz4 and zstd. Learn pages disagree on the status of Kafka Streams and transactions, so treat both as preview until the documentation agrees.
