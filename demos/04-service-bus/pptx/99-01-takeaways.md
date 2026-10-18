---
slide: "99-01"
topic: 04-service-bus
title: "Takeaways"
subtitle: "What to carry into the Event Grid and Event Hubs module"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/04-service-bus/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Four software engineers in their late twenties to mid thirties, two women and two men in
  modern work-casual clothes, gathered at a large whiteboard in a bright team room after a workshop
  day: one woman finishes a hand-drawn sketch of one box with arrows fanning out to three boxes, the
  others stand around her smiling, one man holding a coffee cup and another giving a relaxed
  thumbs-up. The group in the centre of the frame. The whiteboard carries only boxes and arrows: no
  words, no letters, no numbers. Modern, light, bright and friendly: high-key natural daylight, white
  and light-oak contemporary interior, soft pastel accents in light blue, mint and lavender, green
  plants, 2020s design. Screens show only soft blurred shapes and colours, never readable UI. 16:9
  landscape 1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks,
  extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 16
source: demos/04-service-bus/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into the Event Grid and Event Hubs module".
  Side-accent layout: title, subtitle and five bullets on the left half on plain white, the provided
  photograph of a team at a whiteboard sketching boxes and arrows filling the right half, no text over
  it. Use exactly the one image given for this card; do not generate or add any other images, icons or
  logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Give each consumer a filtered subscription and delete the default rule
- Settle after the work: `AutoCompleteMessages = false` with message actions
- Scale on the backlog with a KEDA rule and a managed identity
- Use the parcel ID as the session ID to keep each parcel's scans in order
- Dead-letter unreadable scans with a reason, and write each scan once with a marker batch

## Speaker Notes

Close with the habits this module built, each one a decision in the topology or in the consumer.

Filtered subscriptions: a correlation filter on `Subject` gives each consumer only its own events. On Azure the default rule matches every message until you delete it.

Settle after the work: with `ServiceBusMessageActions` bound and `AutoCompleteMessages` set to false, a crash leaves the scan on the subscription for another attempt.

Scale on the backlog: the `azure-servicebus` KEDA rule adds replicas as `tracking` fills and scales the app to zero when it empties. It reads the backlog with a managed identity, so no connection string is involved.

Order per parcel: the tracking number as session ID gives each parcel one ordered lane, while different parcels run in parallel.

Poison messages and duplicates: dead-letter a scan that can't succeed with a stable reason, and write the scan and its marker in one batch so a redelivery stores nothing twice. Module 5 adds the broadcast side with Event Grid, Event Hubs and SignalR.
