---
slide: "90-02"
topic: 05-event-grid-hubs
title: "Checkpointing the van position processor"
subtitle: "A restart replays from the last checkpoint and skips no position"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/05-event-grid-hubs/_images/push-then-checkpoint.svg"
media-file: labs/05-event-grid-hubs/_images/push-then-checkpoint.svg
visual-prompt: >
  Existing diagram of VanPositionProcessor reading partition 1 of van-positions through the ops-console consumer group, first sending each position through VanPositionHub to the ops console map, then writing the checkpoint to the ops-console blob container, so after a restart it reads 07-6 again, no generation. Referenced at its repo path.
order: 15
source: labs/05-event-grid-hubs/readme.md
gamma_prompt: >
  Slide titled "Checkpointing the van position processor", subtitle "A restart replays from the last
  checkpoint and skips no position". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Checkpointing the van position processor

## Content

- `VanPositionProcessor` reads `van-positions` through the `ops-console` consumer group
- Each position goes out with `SendAsync` before `UpdateCheckpointAsync` stores the checkpoint
- Checkpoints live in the `ops-console` blob container in Azurite
- After a restart, the last pushed position can reach the map twice
- Test it: stop and start `telemetry-ingest`, and every van's sequence continues without a gap

## Speaker Notes

Step 2 of the lab puts the checkpoint trade-off from the partitions topic into code. The processor must resume where it stopped, and the map must show each position as it arrives.

The processor: an `EventProcessorClient` on the `ops-console` consumer group of `van-positions`, with a `BlobContainerClient` for the `ops-console` container as its checkpoint store.

Push, then checkpoint: each `VanPositionReported` goes to `VanPositionHub` at `/hubs/van-positions` with `SendAsync("vanPositionReported", ...)`, and only then does the processor call `UpdateCheckpointAsync`. The map in `van-map.ts` listens for `vanPositionReported` through a `HubConnectionBuilder`.

Azurite: the lab runs the storage account as the Azurite emulator, and the processor uses the connection strings that Aspire injects.

Replays: a restart reads from the last stored checkpoint, so a position pushed just before the stop can arrive twice, and none is skipped. The research prompt asks the agent to weigh one blob write per position against those replays, and to push back on any claim of exactly-once delivery.

Expected outcome: stop `telemetry-ingest` in the Aspire dashboard and start it again, and the dots keep moving with no gap in any van's sequence numbers.
