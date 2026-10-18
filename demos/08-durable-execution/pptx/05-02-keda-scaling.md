---
slide: "05-02"
topic: 05-durable-task-sdk
title: "KEDA scale rule for workers"
subtitle: "Container Apps adds replicas as the activity backlog grows"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/05-02-keda-scale-rule.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. No arrow crosses a box or a label. Top
  left a grey "RouteClient" box, "starts instances", with an arrow labelled
  "ScheduleNewOrchestrationInstanceAsync" down to an amber "Durable Task Scheduler" box holding a white
  "bookings, task hub" row and a white "Activity backlog, pending work items" row with six small
  work-item chips. Arrow "pushes work items" to a blue "Container Apps" box with three stacked
  "route-worker, replica" cards, "1 to 10 replicas" and "--min-replicas 1 --max-replicas 10". Along the
  bottom a purple scale rule band "dts-activities, type azure-durabletask-scheduler, scale rule" with
  chips "endpoint", "taskhubName=bookings", "workItemType=Activity", "maxConcurrentWorkItemsCount=1",
  "scale rule identity". Dashed purple arrows up from the band: "polls" into the scheduler, "sets
  replica count" into Container Apps.
order: 12
source: demos/08-durable-execution/05-durable-task-sdk/readme.md
gamma_prompt: >
  Slide titled "KEDA scale rule for workers", subtitle "Container Apps adds replicas as the activity
  backlog grows". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# KEDA scale rule for workers

## Content

- A custom rule of type `azure-durabletask-scheduler` polls the scheduler for pending work items
- It sets `endpoint`, `taskhubName`, `workItemType` and `maxConcurrentWorkItemsCount`, plus a managed identity
- A lower `maxConcurrentWorkItemsCount` adds replicas sooner; start at 1 for CPU-bound work
- `minReplicas` 0 scales to zero, at the cost of a cold start
- A CLI update replaces the rule, so add the orchestration rule with YAML or Bicep

## Speaker Notes

Worker load rises and falls with the number of waiting activities, so the replica count should follow the backlog. This slide shows the KEDA scale rule that Container Apps uses for Durable Task SDK workers.

Rule type: the worker container app scales with a custom rule of type azure-durabletask-scheduler. The rule polls the scheduler for pending work items of one work item type.

Metadata: endpoint names the scheduler, taskhubName the task hub, workItemType is Orchestration, Activity or Entity, and maxConcurrentWorkItemsCount sets how many work items one replica runs at once. The scale rule identity is a managed identity linked to the scheduler and task hub.

Tuning: a lower maxConcurrentWorkItemsCount makes the scaler add replicas sooner, so start with 1 for CPU-intensive work. Raise the value for I/O-bound work such as HTTP calls, which spends most of its time waiting on the network.

Scale to zero: minReplicas 0 removes every replica when no work is waiting, and the first work item after an idle period waits for a cold start. Keep one replica for latency-sensitive routes.

Revisions and the second rule: adding or changing a scale rule creates a new revision, so confirm that each revision holds the activity rule. Learn documents a second rule for orchestrations, which needs YAML or Bicep because a CLI update replaces the existing rule.
