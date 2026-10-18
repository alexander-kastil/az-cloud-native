---
slide: "05-02"
topic: 05-dead-letter
title: "Dead-lettering and resubmitting unreadable scans"
subtitle: "The reason travels with the message while the parcel's other scans keep moving"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/04-service-bus/_images/unreadable-scan.svg"
media-file: labs/04-service-bus/_images/unreadable-scan.svg
visual-prompt: >
  Existing diagram of three scans of one parcel in a tracking session: ScanValidator.Validate lets PickedUp and AtHub through to the parcel document and sends the plain-text scan to the tracking dead-letter queue with the reason UnreadableLabel while the session stays open, no generation. Referenced at its repo path.
order: 12
source: demos/04-service-bus/05-dead-letter/readme.md
gamma_prompt: >
  Slide titled "Dead-lettering and resubmitting unreadable scans", subtitle "The reason travels with the
  message while the parcel's other scans keep moving". Layout: Two columns. Left column: the bullets.
  Right column, wider than the left: the provided diagram as an inline content image at its natural
  16:9 aspect ratio, scaled to fit the column, never cropped, never used as a background or side
  accent. Use exactly the one image given for this card; do not generate or add any other images, icons
  or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Dead-lettering and resubmitting unreadable scans

## Content

- `DeadLetterMessageAsync` takes a reason and a description, each up to 4,096 characters
- Use a short, stable **reason code** such as `UnreadableLabel`
- Only that message moves, so the parcel's session stays open
- Read dead letters with `SubQueue.DeadLetter` and their `DeadLetterReason`
- Send a repaired copy first, then complete the original

## Speaker Notes

The Contoso Logistics scan processor dead-letters unreadable scans from its Functions trigger, through `ServiceBusMessageActions`. The diagram shows the lab case: a plain-text body between two readable scans of one parcel.

Reason and description: both values are limited to 4,096 characters, and the call works only in `PeekLock` mode. PeekLock is the default receive mode, so the receiver holds a lock on each message until it settles it.

Reason code: a short, stable code lets dashboards and operators group failures by cause. `ScanValidator.Validate` rejects a body that fails to parse or has an unknown `ScanType`.

One message only: dead-lettering moves just that message to the dead-letter subqueue. The session stays open, and the parcel's readable scans still apply in order around it.

Reading dead letters: a receiver with `SubQueue.DeadLetter` in its `ServiceBusReceiverOptions` reads them. `DeadLetterReason` and `DeadLetterErrorDescription` travel with each message, so an operator sees the cause next to the payload.

Resubmitting: fix the cause, send a repaired copy, then complete the original in the dead-letter queue. Sending first means a failed send leaves the original in place.
