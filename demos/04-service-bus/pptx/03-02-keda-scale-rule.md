---
slide: "03-02"
topic: 03-competing-consumers
title: "KEDA scale rule in Container Apps"
subtitle: "Replicas follow the backlog of the tracking subscription"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/04-service-bus/_images/scan-backlog-scaling.svg"
media-file: labs/04-service-bus/_images/scan-backlog-scaling.svg
visual-prompt: >
  Existing diagram of the scan-backlog KEDA azure-servicebus rule with messageCount 10 scaling scan-processor to 6 of its 10 replicas for 60 active scans on tracking, and to none when tracking is empty, no generation. Referenced at its repo path.
order: 8
source: demos/04-service-bus/03-competing-consumers/readme.md
gamma_prompt: >
  Slide titled "KEDA scale rule in Container Apps", subtitle "Replicas follow the backlog of the
  tracking subscription". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# KEDA scale rule in Container Apps

## Content

- A custom `azure-servicebus` rule reads the depth of the `tracking` subscription
- **messageCount** sets the messages per replica: 5 by default, 10 in the lab
- The rule's `identity` reads the backlog with a managed identity instead of a connection string
- `minReplicas` 0 scales to zero, and `maxReplicas` caps the cost
- 60 waiting scans with `messageCount` 10 give 6 of the 10 replicas

## Speaker Notes

Concurrency inside one process has a ceiling, so Container Apps adds replicas with KEDA. The number of replicas follows the backlog, and the app scales to zero when no scans wait.

The rule: a custom rule of type `azure-servicebus` reads a queue through `queueName`, or a topic subscription through `topicName` and `subscriptionName`. The lab's `scan-backlog` rule counts `tracking` on `parcel-events`.

messageCount: each replica handles that many messages, and the default is 5. The lab sets 10, so the rule adds one replica for every ten active scans.

Identity: the `identity` property sits inside the `custom` rule, set to `system` or to the resource ID of a user-assigned identity. That identity needs Azure Service Bus Data Owner, or a custom role that can read the subscription's runtime properties.

Replica limits: with `minReplicas` 0 the app scales to zero, and the 300-second cooldown applies only to that last step. Every added replica adds cost, so `maxReplicas` caps the scale-out.

The lab run: `ScanBacklog` sends 60 scans, and the replica count rises toward six over several 30-second polling intervals. Functions on Container Apps rejects custom rules until `allowScalingRuleOverride` is true, so `infra/scan-consumer.azcli` sets that flag.
