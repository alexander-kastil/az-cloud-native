---
slide: "03-01"
topic: 03-durable-functions
title: "Orchestrations in the isolated worker"
subtitle: "Orchestrator, activities and client on the azureManaged provider"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/booking-functions.svg"
media-file: labs/08-durable-execution/_images/booking-functions.svg
visual-prompt: >
  Existing diagram of the BookingFunctions app, where StartBooking schedules BookingSagaOrchestrator, which calls six activities, and host.json connects through the azureManaged provider to the scheduler emulator, no generation. Referenced at its repo path.
order: 7
source: demos/08-durable-execution/03-durable-functions/readme.md
gamma_prompt: >
  Slide titled "Orchestrations in the isolated worker", subtitle "Orchestrator, activities and client on
  the azureManaged provider". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Orchestrations in the isolated worker

## Content

- The in-process model ends support on 10 November 2026, so use the **.NET isolated worker**
- `[OrchestrationTrigger]` runs the orchestrator, `[ActivityTrigger]` the activities that do the I/O
- The starter schedules through `[DurableClient]` and returns `CreateCheckStatusResponseAsync`
- `host.json` selects the `azureManaged` provider and names the connection string setting
- `hubName` must match the task hub that holds the instances

## Speaker Notes

Durable Functions adds orchestrations and entities to Azure Functions, and their state persists on the Durable Task Scheduler without a database you manage. The diagram shows the lab's BookingFunctions app, the booking saga hosted in Functions.

Isolated worker: the in-process model reaches end of support on 10 November 2026. The scheduler backend needs the Microsoft.Azure.Functions.Worker.Extensions.DurableTask.AzureManaged package, which the lab pins at version 1.10.4.

Orchestrator and activities: the orchestrator takes a TaskOrchestrationContext and calls activities by name, and an activity is an ordinary function that can perform I/O. Only orchestrators must stay deterministic, so route every service call through an activity.

Client start: the starter schedules the instance through the DurableClient binding and returns CreateCheckStatusResponseAsync. A GET on statusQueryGetUri returns the runtimeStatus and, once the instance completes, its output.

host.json: the azureManaged storage provider points the extension at the scheduler, and connectionStringName names the app setting that holds the connection string. The connection string itself belongs in an app setting, never in host.json.

Task hub name: hubName must match the task hub that holds the instances, because a mismatch makes the worker poll a different hub.
