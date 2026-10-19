---
slide: "02-01"
topic: 02-functions-triggers
title: "Service Bus trigger and settlement"
subtitle: "The function completes a scan only after its work succeeds"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/04-service-bus/_images/settle-after-work.svg"
media-file: labs/04-service-bus/_images/settle-after-work.svg
visual-prompt: >
  Existing diagram of ProcessScan settling a scan from the tracking subscription: CompleteMessageAsync after the work succeeds, and the scan staying on the subscription for another attempt after a crash, no generation. Referenced at its repo path.
order: 5
source: demos/04-service-bus/02-functions-triggers/readme.md
gamma_prompt: >
  Slide titled "Service Bus trigger and settlement", subtitle "The function completes a scan only
  after its work succeeds". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Service Bus trigger and settlement

## Content

- Isolated worker model with `Microsoft.Azure.Functions.Worker.Extensions.ServiceBus` 5.14.1 or later
- Bind `ServiceBusReceivedMessage` for metadata and `ServiceBusMessageActions` to settle
- Set `AutoCompleteMessages = false` whenever the function binds message actions
- **Settlement**: complete on success, abandon to retry, dead-letter a poison message
- An unhandled exception or an expired lock returns the message with a higher delivery count

## Speaker Notes

Grey Logistics runs the scan processor as an Azure Functions app. Each scan on the `tracking` subscription starts one function invocation that updates the tracking read model.

Isolated worker: the code runs in a process separate from the Functions host, and version 5.14.1 or later of the extension provides the two types on this slide. The in-process model reaches end of support on November 10, 2026, so new C# work uses the isolated model.

Bindings: `ServiceBusReceivedMessage` exposes `MessageId`, `Subject`, `DeliveryCount` and `ApplicationProperties`. A `ServiceBusMessageActions` parameter lets the function settle the message itself.

AutoCompleteMessages: when it is true, the runtime completes the message after a successful run, before your code can decide. The Functions documentation requires false when the function binds message actions.

Settlement: `CompleteMessageAsync` removes the message from the subscription, and `AbandonMessageAsync` returns it with a higher delivery count. `DeadLetterMessageAsync` moves it to the dead-letter subqueue with a reason and a description.

Redelivery: after an unhandled exception the extension abandons the message, so it can be received again immediately. A message still unsettled when its lock expires also returns to the subscription, and topic 05 covers what repeated redeliveries lead to.
