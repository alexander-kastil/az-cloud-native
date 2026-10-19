---
slide: "01-01"
topic: 01-custom-topics
title: "Publishing CloudEvents to a custom topic"
subtitle: "The label-service publishes once, each partner gets a subscription"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/05-event-grid-hubs/_images/publish-to-partner-queue.svg"
media-file: labs/05-event-grid-hubs/_images/publish-to-partner-queue.svg
visual-prompt: >
  Existing diagram of ParcelDeliveredPublisher sending ParcelDelivered as a CloudEvent to the custom topic egt-grey-labs05 with DefaultAzureCredential, and the partner-delivered subscription putting it in the partner's storage queue, no generation. Referenced at its repo path.
order: 3
source: demos/05-event-grid-hubs/01-custom-topics/readme.md
gamma_prompt: >
  Slide titled "Publishing CloudEvents to a custom topic", subtitle "The label-service publishes once,
  each partner gets a subscription". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Publishing CloudEvents to a custom topic

## Content

- A **custom topic** has an endpoint and keys; a system topic belongs to an Azure service
- `EventGridPublisherClient` sends a `CloudEvent` with a source, a type and data
- `DefaultAzureCredential` needs the **EventGrid Data Sender** role on the topic
- Handlers: HTTPS webhooks, Azure Functions, Service Bus, Event Hubs and Storage queues
- The event type name is the contract with every subscriber

## Speaker Notes

Grey Logistics needs to tell partners when a parcel reaches its destination, and today no partner can see that moment. The diagram shows the lab's version: the label-service publishes `ParcelDelivered` to a custom topic, and a subscription delivers it to the partner's storage queue.

Custom and system topics: your application publishes to a custom topic, which has an endpoint, access keys and a schema you choose. Only Azure services publish to a system topic, so it has no endpoint or keys.

EventGridPublisherClient: `SendEventAsync` sends one event and `SendEventsAsync` a batch, both with `CloudEvent` objects when the topic uses the CloudEvents schema. The constructor takes the source, the event type and the data, which the SDK serializes as JSON.

Entra ID instead of keys: the label-service authenticates with `DefaultAzureCredential`, and its identity holds the EventGrid Data Sender role on the topic. An access key works for a quick test, but a key doesn't tell you which service sent an event.

Handlers: partners with their own HTTPS endpoint use a webhook, and internal consumers use the Azure Functions Event Grid trigger. A Service Bus queue fits a partner that must keep messages while it is slow or offline.

A stable contract: event type names are the contract between the publisher and every subscriber, so the label-service keeps them stable.
