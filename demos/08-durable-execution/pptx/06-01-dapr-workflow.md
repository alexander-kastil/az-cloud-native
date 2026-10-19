---
slide: "06-01"
topic: 06-dapr-workflow
title: "Workflows and activities in Dapr"
subtitle: "A sidecar next to each service keeps the workflow history"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/06-01-dapr-sidecar.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Top
  row: a blue "booking-worker" box holding white cards "ParcelBookingWorkflow, Workflow<string, string>"
  with an arrow "CallActivityAsync" down to "ReserveCapacityActivity, WorkflowActivity<string, string>",
  beside "DaprWorkflowClient" listing ScheduleNewWorkflowAsync and GetWorkflowStateAsync and
  "AddDaprWorkflow(), registers the classes". A two-way arrow to a purple "Dapr sidecar" box, "app ID
  booking-worker", with a white "history log" chip and "replays the workflow after a crash, restart or
  redeployment". Arrows from the sidecar to two amber boxes: "state store, component, supports
  workflows" and "messaging, component". Below, captioned "hosting", a green "AKS" box (Dapr cluster
  extension installs the runtime; supports the current and previous Dapr version; WorkflowAccessPolicy
  is a Kubernetes custom resource) beside a grey "Local with Aspire" box (WithDaprSidecar();
  CommunityToolkit.Aspire.Hosting.Dapr; dapr init runs on the machine first).
order: 13
source: demos/08-durable-execution/06-dapr-workflow/readme.md
gamma_prompt: >
  Slide titled "Workflows and activities in Dapr", subtitle "A sidecar next to each service keeps the
  workflow history". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Workflows and activities in Dapr

## Content

- A workflow derives from `Workflow<TInput, TOutput>`, an activity from `WorkflowActivity<TInput, TOutput>`
- `DaprWorkflowClient` starts with `ScheduleNewWorkflowAsync` and reads `GetWorkflowStateAsync`
- Dapr replays the code against its history log, so the determinism rules still apply
- Dapr 1.18 can sign the history, and `WorkflowAccessPolicy` controls which apps invoke a workflow
- Run it on **AKS** with the Dapr extension; locally, Aspire attaches the sidecar

## Speaker Notes

Grey runs several services on AKS through Dapr, and the booking workflow should use the same sidecar model as the services it calls. Dapr Workflow gives durable behavior similar to the Durable Task model, with a runtime next to each service.

Workflow and activity: ParcelBookingWorkflow derives from Workflow<string, string> and calls ReserveCapacityActivity by name with CallActivityAsync. AddDaprWorkflow registers the services, and Dapr 1.18 registers the workflow and activity classes itself.

Client: an injected DaprWorkflowClient schedules the workflow and returns the instance ID, and GetWorkflowStateAsync reads the current state of that instance. The state store and messaging components are configuration, so the workflow code does not change when they do.

Replay: Dapr appends each step to the workflow's history log and replays the code against it after a crash, restart or redeployment, so the code must be deterministic. Read the workflow features and concepts page before you port an orchestrator from the Durable Task model.

Dapr 1.18: the release of 10 June 2026 can sign workflow history and verify it on every state load, which needs mTLS. The WorkflowAccessPolicy custom resource controls which app IDs can invoke which workflows and activities.

Hosting: Container Apps managed Dapr is not compatible with the workflow SDK packages, so the code runs on AKS, where the Dapr cluster extension installs the runtime. Locally, CommunityToolkit.Aspire.Hosting.Dapr attaches the sidecar after dapr init, but its tested Dapr version is older than 1.18, so check its compatibility notes.
