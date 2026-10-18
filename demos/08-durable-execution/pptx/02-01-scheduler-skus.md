---
slide: "02-01"
topic: 02-durable-task-scheduler
title: "Schedulers, task hubs and SKUs"
subtitle: "Dedicated reserves capacity, Consumption bills per action"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/08-durable-execution/pptx/images/02-01-scheduler-skus.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif as in the module's existing diagrams, headings #0F172A, secondary text #64748B, minimum 26px
  type. No heading inside the image; the slide title carries it. Two equal cards side by side. Left card
  blue #DBEAFE with #1D4ED8 border, heading "Dedicated", secondary "reserves capacity units (CUs)". Right
  card green #DCFCE7 with #15803D border, heading "Consumption", secondary "an action is a message the
  scheduler dispatches". Each card holds seven white rows, a grey label above a dark value, in the same
  order. Dedicated: Billing "fixed monthly cost per capacity unit", Throughput "2,000 actions per second
  per unit", Data retention "up to 90 days", High availability "three capacity units", Best fit
  "production with predictable volume", Schedulers per region per subscription "25", Task hubs "25 per
  region per subscription". Consumption: "pay per action dispatched", "500 actions per second", "up to
  30 days", "not available", "development and variable workloads", "10", "5 per scheduler". No red.
order: 5
source: demos/08-durable-execution/02-durable-task-scheduler/readme.md
gamma_prompt: >
  Slide titled "Schedulers, task hubs and SKUs", subtitle "Dedicated reserves capacity, Consumption
  bills per action". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Schedulers, task hubs and SKUs

## Content

- A **scheduler** is the Azure resource; a task hub inside it holds the state of a set of orchestrations
- Each app connects to one task hub, created with the `durabletask` CLI extension
- Dedicated reserves capacity units at a fixed monthly cost, 2,000 actions per second each
- Consumption bills per dispatched action, at 500 actions per second
- Inputs and outputs are limited to 1 MB, instance IDs to 100 printable ASCII characters

## Speaker Notes

Orchestration state has to survive restarts and grow with the number of workers, and Contoso does not want to run and tune a state store for every booking service. The Durable Task Scheduler is the managed backend that keeps that state for Durable Functions and the Durable Task SDKs.

Scheduler and task hub: the scheduler is the Azure resource, and a task hub is a container inside it that holds the state of a set of orchestrations. The class creates the scheduler contoso-dts and the task hub bookings.

One task hub per app: each app connects to one task hub, and both are created after adding the durabletask extension to the Azure CLI. The example allow list 0.0.0.0/0 permits any IP address, so restrict it to your network ranges before production traffic.

Dedicated SKU: you reserve capacity units at a fixed monthly cost, each with 2,000 actions per second and up to 90 days of data retention. A single unit gives no redundancy, so a high-availability deployment needs three units.

Consumption SKU: you pay per action, a message the scheduler dispatches to an orchestrator, activity or entity, at 500 actions per second and up to 30 days of retention. It fits development and variable workloads and reached general availability in April 2026.

Limits on every SKU: an orchestrator, activity or entity input or output is limited to 1 MB, and a work item completion request to 3.9 MB. Instance IDs stay within 100 printable ASCII characters, and IDs that start with @ are reserved for entities.
