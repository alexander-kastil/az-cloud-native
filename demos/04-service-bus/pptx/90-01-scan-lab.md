---
slide: "90-01"
topic: 04-service-bus
title: "The scan processing lab"
subtitle: "Seven steps from the parcel-events topic to a consumer that scales on its backlog"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/04-service-bus/_images/scan-pipeline.svg"
media-file: labs/04-service-bus/_images/scan-pipeline.svg
visual-prompt: >
  Existing diagram of the finished scan pipeline, each piece numbered with the lab step that adds it: the parcel-events topic and its three filtered subscriptions, scan-processor on Container Apps, the tracking dead-letter queue, the tracking container in Cosmos DB and the scan-backlog KEDA rule, no generation. Referenced at its repo path.
order: 15
source: labs/04-service-bus/readme.md
gamma_prompt: >
  Slide titled "The scan processing lab", subtitle "Seven steps from the parcel-events topic to a
  consumer that scales on its backlog". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect
  ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The scan processing lab

## Content

- Steps 1 and 2: `parcel-events` with three filtered subscriptions, and a **Functions trigger** that settles after its work
- Step 3: sessions on `tracking` keep each parcel's scans in order
- Step 4: unreadable scans dead-letter with the reason `UnreadableLabel`
- Step 5: duplicate detection and a marker in one Cosmos DB batch
- Steps 6 and 7: deploy once, scale with KEDA, record the patterns in `docs/scan-processing.md`

## Speaker Notes

The lab turns the module into running code in about 55 minutes. You open `labs/04-service-bus/grey-logistics/` with Claude Code or GitHub Copilot agent mode, and Docker Desktop runs the Service Bus and Cosmos DB emulators.

Topic and trigger: step 1 defines the topic and its subscriptions in the AppHost, so the emulator and Azure get the same topology. Step 2 adds the isolated Functions app, whose `ProcessScan` completes a scan only after its work succeeds.

Order: step 3 sets `RequiresSession` on `tracking` and `IsSessionsEnabled` on the trigger. `ParcelScanStore` appends each scan to the `scans` array of the `parcel-{trackingNumber}` document.

Poison scans: step 4 checks each scan with `ScanValidator.Validate` and dead-letters an unreadable one with the reason `UnreadableLabel`, so the session stays open for the parcel's later scans.

Duplicates: step 5 turns on a five-minute duplicate detection window, the largest the emulator accepts, and writes the scan and the `processed-{messageId}` marker in one transactional batch.

Azure: step 6 deploys `scan-processor` to Container Apps and adds the `scan-backlog` KEDA rule, because the emulator has no autoscale. Step 7 writes `docs/scan-processing.md`, which maps each pattern to the file that implements it.
