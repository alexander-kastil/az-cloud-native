---
slide: "06-02"
topic: 06-dapr-workflow
title: "Dapr Workflow and Durable Task compared"
subtitle: "Choose by hosting, portability and operations"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/06-02-dapr-vs-durable-task.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. A comparison table with row labels on
  the left and two columns of white cells: blue header "Durable Task, Functions and SDKs" and purple
  header "Dapr Workflow". Rows: .NET package "Microsoft.DurableTask.Worker.AzureManaged, or the Functions
  extension" / "Dapr.Workflow"; State backend "Durable Task Scheduler" / "a state store component that
  supports workflows"; Start and read state "ScheduleNewOrchestrationInstanceAsync, status URLs" /
  "ScheduleNewWorkflowAsync, GetWorkflowStateAsync"; External events "RaiseEventAsync" / "raise event
  through the HTTP or gRPC management API"; Azure hosting "Functions, Container Apps, AKS, App Service
  and VMs" / "AKS through the Dapr extension"; Upgrade model "Azure-managed backend" / "Dapr runtime
  version that the team schedules". A grey band across both columns, "After a failure: both replay the
  code against its history under determinism rules". A last row "Choose when" with tinted cells: "the
  service runs on Functions or Container Apps, and the backend is a managed Azure service" / "the
  service already runs on AKS with Dapr, one component model for state, messaging and workflows". No red.
order: 14
source: demos/08-durable-execution/06-dapr-workflow/readme.md
gamma_prompt: >
  Slide titled "Dapr Workflow and Durable Task compared", subtitle "Choose by hosting, portability and
  operations". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dapr Workflow and Durable Task compared

## Content

- Both persist progress and replay the code under determinism rules after a failure
- Durable Task keeps its state in the managed **Durable Task Scheduler**
- Dapr Workflow keeps it in a state store component and runs on AKS
- Choose Durable Task on Functions or Container Apps with a managed backend
- Choose Dapr Workflow when the service already runs on AKS with Dapr

## Speaker Notes

Both models persist progress and resume after a failure, so the choice depends on hosting, portability and operations. This slide compares them on the properties that matter for a service in this class.

Shared model: both replay the code against its history under determinism rules, which is why topics 1, 4 and 7 apply to either one.

Durable Task: it uses Microsoft.DurableTask.Worker.AzureManaged or the Functions extension, keeps state in the Durable Task Scheduler and starts instances with ScheduleNewOrchestrationInstanceAsync. It runs on Functions, Container Apps, AKS, App Service and VMs with an Azure-managed backend.

Dapr Workflow: it uses Dapr.Workflow, keeps state in a state store component that supports workflows and starts with ScheduleNewWorkflowAsync. It runs on AKS through the Dapr extension, on a Dapr runtime version the team schedules.

Choosing Durable Task: pick it when the service runs on Functions or Container Apps and the backend should be a managed Azure service.

Choosing Dapr Workflow: pick it when the service already runs on AKS with Dapr and the team wants the same component model for state, messaging and workflows.
