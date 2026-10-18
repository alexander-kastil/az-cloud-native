---
slide: "05-01"
topic: 05-durable-task-sdk
title: "Worker and client packages"
subtitle: "Orchestrations and activities on a generic host"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/05-01-worker-host.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Left
  half, a vertical stack of four cards joined by short down arrows: grey
  "Host.CreateApplicationBuilder(args)" with "console app, worker service or ASP.NET Core project"; blue
  "AddDurableTaskWorker()" with "runs as a background service"; blue ".AddTasks(registry => ...)" with
  the chips "AddOrchestrator<RouteOrchestrator>()" and "AddActivity<AssignVanActivity>()" and the note
  "work item filters, the worker receives only these tasks"; amber
  ".UseDurableTaskScheduler(schedulerConnectionString)" with "connection string + ;TaskHub= +
  TASKHUB_NAME". Right side: a green "RouteClient" box, "client package only", listing
  AddDurableTaskClient, ScheduleNewOrchestrationInstanceAsync and WaitForInstanceCompletionAsync, with an
  arrow "schedules an instance" down to an amber "Durable Task Scheduler" box, "task hub from
  TASKHUB_NAME", holding "work items as they arrive" with the chips RouteOrchestrator and
  AssignVanActivity. Arrow "pushes work items" from the scheduler to the AddTasks card, arrow "sends
  results" from the UseDurableTaskScheduler card to the scheduler.
order: 11
source: demos/08-durable-execution/05-durable-task-sdk/readme.md
gamma_prompt: >
  Slide titled "Worker and client packages", subtitle "Orchestrations and activities on a generic host".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Worker and client packages

## Content

- The **AzureManaged** worker and client packages add `UseDurableTaskScheduler` to the builder
- The worker registers `RouteOrchestrator` and `AssignVanActivity` during host setup
- It runs as a background service in a console app, worker service or ASP.NET Core project
- The scheduler pushes work items, and filters send each worker only the tasks it hosts
- A service that starts instances installs only the client package

## Speaker Notes

Contoso's routing workers assign vans to parcels and do not need the Functions host, so they run as plain .NET services. The Durable Task SDK provides the worker and client libraries for them.

AzureManaged packages: Microsoft.DurableTask.Worker.AzureManaged and Microsoft.DurableTask.Client.AzureManaged connect to the Durable Task Scheduler and add UseDurableTaskScheduler to the builder. The worker project installs both.

Registration: AddDurableTaskWorker registers RouteOrchestrator and AssignVanActivity in AddTasks, and the worker appends the task hub from TASKHUB_NAME to the connection string. The orchestrator calls the activity with nameof, so renaming the class fails the build.

Generic host: the worker runs as a background service, so the same code works in a console app, a worker service or an ASP.NET Core project.

Push and filters: the scheduler pushes work items as they arrive, so the worker does not poll, and each replica processes several work items in parallel. The .NET SDK generates work item filters from the registered tasks, so a worker receives only the orchestrations and activities it hosts.

Client: a service that starts instances registers AddDurableTaskClient with the same connection string. RouteClient schedules the orchestration, waits with WaitForInstanceCompletionAsync and reads the output.
