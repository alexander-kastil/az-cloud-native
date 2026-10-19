---
slide: "02-02"
topic: 02-durable-task-scheduler
title: "Emulator and connection strings"
subtitle: "Local runs on the emulator, Azure apps use managed identity"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/scheduler-wiring.svg"
media-file: labs/08-durable-execution/_images/scheduler-wiring.svg
visual-prompt: >
  Existing diagram of the AppHost starting the scheduler emulator with the bookings task hub and passing the connection string and TASKHUB_NAME settings to booking-worker, no generation. Referenced at its repo path.
order: 6
source: demos/08-durable-execution/02-durable-task-scheduler/readme.md
gamma_prompt: >
  Slide titled "Emulator and connection strings", subtitle "Local runs on the emulator, Azure apps use
  managed identity". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Emulator and connection strings

## Content

- The **emulator** runs the same protocol in Docker: port 8080 for the endpoint, 8082 for the dashboard
- Under Aspire, `RunAsEmulator()` starts it and passes `ConnectionStringExpression` to the app
- Local: `Endpoint=http://localhost:8080;Authentication=None`, with the task hub from `TASKHUB_NAME`
- Azure: `Authentication=ManagedIdentity` with the client ID of a user-assigned identity
- Grant Durable Task Data Contributor at the task hub scope

## Speaker Notes

This slide moves from the Azure resource to the connection your app opens to it, locally and in Azure. The diagram comes from lab step 1, where the AppHost wires the emulator to the booking worker.

Emulator: it runs the same protocol in a Docker container, so you can develop without an Azure subscription. Bind ports 8080 and 8082 to loopback, because the emulator accepts unauthenticated requests.

Aspire: the AppHost starts the emulator with RunAsEmulator() and passes the scheduler's ConnectionStringExpression to the app, so no port is hard-coded. The dashboard endpoint of the scheduler resource opens the emulator dashboard.

Local connection string: it names the endpoint and Authentication=None, and the worker appends ;TaskHub= with the value of the TASKHUB_NAME setting.

Managed identity in Azure: the connection string carries the scheduler endpoint and the ClientID of a user-assigned identity, which Learn recommends because it outlives the app. The scheduler accepts managed identity only, never connection strings with storage keys.

Role at the task hub scope: Durable Task Data Contributor includes every other data role, and assigning it at the task hub follows least privilege. The dashboard also needs a role, and Learn names two different ones, so check the current dashboard page before you grant access to a team.
