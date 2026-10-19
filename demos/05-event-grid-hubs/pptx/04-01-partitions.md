---
slide: "04-01"
topic: 04-partitions-checkpoints
title: "Partitions and consumer groups"
subtitle: "Order holds within one partition, each group reads the whole stream"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/05-event-grid-hubs/04-partitions-checkpoints/_images/partition-cursors.svg"
media-file: demos/05-event-grid-hubs/04-partitions-checkpoints/_images/partition-cursors.svg
visual-prompt: >
  Existing diagram of van-07, van-12 and van-21 positions routed by PartitionKey into the four partition logs of van-positions, positions without a key going round-robin, and the ops-console and reporting consumer groups reading with their own cursors, no generation. Referenced at its repo path.
order: 10
source: demos/05-event-grid-hubs/04-partitions-checkpoints/readme.md
gamma_prompt: >
  Slide titled "Partitions and consumer groups", subtitle "Order holds within one partition, each group
  reads the whole stream". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Partitions and consumer groups

## Content

- Each **partition** is an ordered log, and order holds only inside one
- The van ID as partition key keeps one van's positions together, in order
- Events without a key go round-robin; a fixed partition ID lowers availability
- A **consumer group** reads the full stream at its own pace
- One active reader per partition and group, one checkpoint container per group

## Speaker Notes

A further consumer such as a reporting job could need every position of one van in order, while the ops console needs the latest position of every van. The keys that producers set and the consumer groups that read decide whether both stay correct.

Ordered logs: an event hub splits its stream into partitions, and Event Hubs guarantees order only within one partition. The design question is which events must stay in order together.

Partition key: the producer sets the key and Event Hubs hashes it to choose the partition, and `CreateBatchOptions` carries one key for a whole batch. With the van ID as key, every position of `van-07` lands in one partition in arrival order.

Without a key: events without a key go to partitions in round-robin order. Sending to a partition ID ties the producer to that partition and lowers availability, so most designs use keys.

Consumer groups: each group is a separate view of the whole event hub, so `ops-console` and an example `reporting` group both read every position. Each group keeps its own cursor in every partition.

One reader each: Event Hubs allows up to five concurrent readers per partition in a group, but two active readers in one group process the same events twice. Give each consuming service its own consumer group and its own checkpoint container.
