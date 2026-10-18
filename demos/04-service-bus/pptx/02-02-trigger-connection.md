---
slide: "02-02"
topic: 02-functions-triggers
title: "Concurrency, connections and hosting"
subtitle: "The trigger connects through the app's managed identity"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/04-service-bus/pptx/images/02-02-trigger-connection.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, in the module's diagram palette: background
  #F8FAFC, 'Segoe UI', Inter, Arial, sans-serif, Consolas for settings, text #0F172A, secondary #64748B,
  arrows #334155 3px, minimum 26px type. No heading inside the image; the slide title carries it. No
  arrow crosses a box or a label. Left, a blue #DBEAFE box with #1D4ED8 border "scan-processor",
  secondary "Functions app in the aca-env Container Apps environment", holding two white boxes:
  "host.json extensions.serviceBus" with maxConcurrentCalls: 8, maxConcurrentSessions: 8,
  prefetchCount: 0, maxAutoLockRenewalDuration: 00:05:00 and the note "per instance; default 16,
  multiplied by the cores"; and "application settings" with ServiceBusConnection__fullyQualifiedNamespace
  <NAMESPACE>.servicebus.windows.net and ServiceBusConnection__credential managedidentity. An arrow from
  the settings to a purple #EDE9FE box "managed identity of scan-processor", and an arrow on to the
  right column: an amber #FEF3C7 box "parcel-events topic" with an arrow down to a white box with
  #B45309 border "tracking subscription" that holds two green #DCFCE7 role boxes, "Azure Service Bus
  Data Receiver: receives messages from the subscription" and "Azure Service Bus Data Owner: scale
  decisions, runtime properties read through the Administration API", and the green line "both roles
  are assigned on the subscription itself".
order: 6
source: demos/04-service-bus/02-functions-triggers/readme.md
gamma_prompt: >
  Slide titled "Concurrency, connections and hosting", subtitle "The trigger connects through the app's
  managed identity". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Concurrency, connections and hosting

## Content

- `maxConcurrentCalls` limits callbacks per instance: 16 by default, multiplied by the cores
- `maxConcurrentSessions` 8 caps the parcel sessions one instance works on
- `ServiceBusConnection__fullyQualifiedNamespace` and `__credential` replace the connection string
- **Roles on the subscription**: Data Receiver to receive, Data Owner for scale decisions
- `AddAzureFunctionsProject` places `scan-processor` in the `aca-env` Container Apps environment

## Speaker Notes

With settlement in place, the next settings decide how much one instance processes at once, how the trigger signs in, and where the app runs.

maxConcurrentCalls: the `extensions.serviceBus` section of `host.json` limits the callbacks that run at once per instance, and the default is 16. The runtime multiplies the value by the core count, so lower it when handlers call a database with a fixed connection limit.

Sessions and locks: the `tracking` trigger uses sessions, so the Contoso Logistics `host.json` sets `maxConcurrentSessions` to 8. `prefetchCount` stays at its default of 0, and `maxAutoLockRenewalDuration` renews a message lock for up to five minutes.

Identity-based connection: settings that share the `ServiceBusConnection` prefix name the namespace and set the credential to `managedidentity`. The function app's managed identity then needs a role on the subscription.

Roles: Azure Service Bus Data Receiver receives the messages, and Data Owner lets the extension read runtime properties for scale decisions. The assignment must reach the subscription itself, because a scope on the topic alone fails when the trigger starts.

Hosting: `AddAzureFunctionsProject` in `src/AppHost/Program.cs` registers the app in the `aca-env` Container Apps environment, and `aspire deploy` publishes it there. Custom scale rules need `allowScalingRuleOverride` set to true, which the next topic uses.
