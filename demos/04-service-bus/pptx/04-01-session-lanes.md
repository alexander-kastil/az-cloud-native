---
slide: "04-01"
topic: 04-sessions
title: "Session-enabled subscriptions"
subtitle: "The tracking number is the session ID, so each parcel gets one ordered lane"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/04-service-bus/04-sessions/_images/session-lanes.svg"
media-file: demos/04-service-bus/04-sessions/_images/session-lanes.svg
visual-prompt: >
  Existing diagram of interleaved scans sorted into one lane per SessionId on the tracking subscription, each locked lane draining in order to its own scan-processor receiver while an abandoned AtHub scan holds back only PKG-100482, no generation. Referenced at its repo path.
order: 9
source: demos/04-service-bus/04-sessions/readme.md
gamma_prompt: >
  Slide titled "Session-enabled subscriptions", subtitle "The tracking number is the session ID, so
  each parcel gets one ordered lane". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Session-enabled subscriptions

## Content

- `RequiresSession`: messages that share a `SessionId` form a first-in, first-out sequence
- The tracking number is the **session ID**, so every scan of a parcel shares a session
- Different sessions run in parallel, and one receiver at a time holds a session's lock
- An abandoned scan holds back only the later scans of its own parcel
- A message without a session ID dead-letters with `Session ID is null`

## Speaker Notes

A parcel scanned `PickedUp`, then `AtHub`, then `OutForDelivery` must show that sequence on the tracking page. If two scans of the same parcel run in parallel, the page can show the van leaving before the hub scan lands.

First in, first out: a session-enabled queue or subscription sets `RequiresSession` when you create it. Messages that share a `SessionId` then form a first-in, first-out sequence.

Session ID: Contoso uses the tracking number as the session ID. The scan processor receives these sessions through a Functions trigger with `IsSessionsEnabled = true`.

Parallel lanes: different sessions run in parallel, while the messages inside one session stay in order. Only one receiver can hold a session's lock at a time.

Abandon: the next receive serves the abandoned message again, so a failing scan holds back the later scans of its parcel until it succeeds or moves to the dead-letter queue. The other parcels keep moving.

Missing session ID: a message sent to a session-enabled entity without a session ID dead-letters with the reason `Session ID is null`, so the publisher sets the parcel ID as the session ID on every scan.
