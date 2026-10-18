---
slide: "90-02"
topic: 08-durable-execution
title: "Safe retries and a worker restart"
subtitle: "One payment line per parcel, even after a restart"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/08-durable-execution/_images/idempotent-activities.svg"
media-file: labs/08-durable-execution/_images/idempotent-activities.svg
visual-prompt: >
  Existing diagram of two bookings with the same parcel ID, where CapturePayment and PrintLabel find their ledger line and return without writing, so the ledger holds one CapturePayment line, no generation. Referenced at its repo path.
order: 19
source: labs/08-durable-execution/readme.md
gamma_prompt: >
  Slide titled "Safe retries and a worker restart", subtitle "One payment line per parcel, even after a
  restart". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Safe retries and a worker restart

## Content

- Only `PrintLabel` gets a retry policy: three attempts, the first retry after 5 seconds
- `CapturePayment` and `PrintLabel` look up the parcel ID plus the activity name in the ledger
- Two bookings with the same parcel ID leave one `CapturePayment` line
- A worker stopped mid-saga hands the instance to the restarted worker, which replays the history
- Parcel IDs with `FAIL-` or `SLOW` make failures and the restart visible

## Speaker Notes

The runtime runs each activity at least once, so a retry or a restart can repeat a payment. Lab steps 3 and 4 make the saga safe against both, and the diagram shows the ledger after two bookings of one parcel.

Retry policy: only the PrintLabel call gets TaskOptions with a RetryPolicy of three attempts and a first retry after 5 seconds. The other forward activities and the compensations run without retry options.

Idempotency key: before CapturePayment or PrintLabel does its work, it looks in booking-ledger.jsonl for a line keyed with the parcel ID, a colon and the activity name. If that line exists, the activity returns without writing a second one.

The proof: two bookings with the same parcel ID leave one CapturePayment line in GET /ledger/{parcelId}. The key uses the parcel ID and the activity name only, never a timestamp or a GUID.

Worker restart: stop booking-worker in the Aspire dashboard while a SLOW booking runs, then start it again. The scheduler hands the abandoned work item to the new worker, which replays the history, and the instance completes with the output booked.

Test markers: a parcel ID with FAIL- followed by an activity name makes that activity throw before it writes its line. SLOW makes each forward activity wait 3 seconds, long enough to stop the worker in the middle.
