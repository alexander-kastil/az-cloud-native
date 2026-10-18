---
slide: "02-01"
topic: 02-sync-async
title: "Commands, events and queries"
subtitle: "What the sender expects decides the kind of message"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/02-01-message-kinds.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, text #0F172A, secondary text #64748B, arrows #334155
  3px, minimum 26px type. No heading inside the image. No arrow crosses a box or a label. Three white
  lanes with #94A3B8 borders, each with the kind on the left and its Contoso example on the right, all
  from the topic readme table. Command (blue): "Imperative verb", "The sender expects an action, or a
  failure"; Booking sends "ReserveCapacity" to Routing, caption "the sender names the receiver". Event
  (amber): "Past-tense fact", "The sender expects nothing from the consumers"; a grey Parcel scanner
  emits an amber ParcelScanned note that fans out to Tracking and to a dashed box "any number of services
  can react". Query (green): "Read request", "The sender expects the current data"; the Tracking page
  sends "HTTP GET" to a green Tracking read model, which returns "current data" on a dashed arrow.
order: 5
source: demos/01-distributed-design/02-sync-async/readme.md
gamma_prompt: >
  Slide titled "Commands, events and queries", subtitle "What the sender expects decides the kind of
  message". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Commands, events and queries

## Content

- A **command** asks a named service to act and expects an action or a failure
- An **event** reports a fact, and the publisher does not know who listens
- A **query** reads current data and changes nothing, so HTTP GET fits
- Commands use imperative verbs, events use the past tense

## Speaker Notes

Contoso has a booking flow that must answer the customer now and a scan flow that must never lose a parcel. Before choosing a transport, name what each message is.

Command: BookShipment, ReserveCapacity, CapturePayment and PrintLabel ask a service to act. The sender names the receiver, so it knows what it asked for.

Event: ShipmentBooked and ParcelScanned report facts that already happened, and any number of services can react without the publisher changing. The Microsoft Learn comparison of Azure messaging services makes the same split between an event and a message that carries data under a contract.

Query: the tracking page is a query served from a read model. Repeating a query is harmless, which is why queries suit HTTP GET.

Naming: a command is an imperative verb such as ReserveCapacity, an event a past-tense fact such as ParcelScanned.
