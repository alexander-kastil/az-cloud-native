---
slide: "03-01"
topic: 03-event-storming
title: "The sticky-note grammar"
subtitle: "Business events on a wall, with the domain experts in the room"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/01-distributed-design/pptx/images/03-01-sticky-note-grammar.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. An event storming workshop in a bright modern meeting room: four people in their late
  twenties to mid thirties in modern work-casual clothes, three software engineers and a dispatcher, two
  women and two men, stand close together at one stretch of a long roll of white paper fixed to a white
  wall. A row of orange square sticky notes runs along the paper as a timeline, with a few blue sticky
  notes placed just before some of the orange ones. The dispatcher, a woman, presses a fresh orange note
  onto the paper while an engineer beside her points along the row and the other two watch, one holding
  a marker and a small pad of orange notes. The sticky notes show only their flat colour and faint grey
  lines, with no letters, words or numbers anywhere. Modern, light, bright and friendly: high-key
  natural daylight, white and light-oak contemporary interior, soft pastel accents in light blue, mint
  and lavender, green plants, 2020s design. Screens show only soft blurred shapes and colours, never
  readable UI. 16:9 landscape 1920x1080, contemporary editorial photography. Avoid: readable text,
  logos, watermarks, extra people, dark or moody lighting, vintage or retro decor, grime, clutter.
order: 8
source: demos/01-distributed-design/03-event-storming/readme.md
gamma_prompt: >
  Slide titled "The sticky-note grammar", subtitle "Business events on a wall, with the domain experts in
  the room". Side-accent layout: title, subtitle and five bullets on the left half on plain white, the
  provided photograph of a team placing orange and blue sticky notes along a paper wall filling the
  right half, no text over it. Use exactly the one image given for this card; do not generate or add
  any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The sticky-note grammar

## Content

- **Events** are orange notes in the past tense, such as ShipmentBooked
- **Commands** are blue notes written as imperatives, such as BookShipment
- Actors issue commands, policies react to events, and **aggregates** decide
- **Hotspots** mark open questions, each with an owner and a date
- The big picture comes first, then the process level for one flow

## Speaker Notes

Booking calls a parcel a shipment, Tracking calls it a tracking number and Billing a charge, and a service boundary belongs where the words change meaning. Event storming finds those boundaries in a workshop with the domain experts in the room.

Events: orange notes written in the past tense, such as ShipmentBooked or ParcelScanned, placed along one long timeline.

Commands: blue notes written as imperatives, such as BookShipment or PrintLabel. Published colour conventions differ for the other notes, so the workshop picks those colours.

Actors, policies and aggregates: an actor such as a customer, a dispatcher or a parcel scanner issues a command, and a policy reads "whenever ParcelScanned reports AtHub, notify the customer". The aggregate is the consistency unit that decides, such as the Shipment that accepts BookShipment.

Hotspots: a hotspot marks an open question, such as who approves a refund when a van is late. Each one gets an owner and a date, so the question does not turn into a hidden dependency.

Two levels: the big-picture session maps every event on one timeline and postpones arguments about services. The process level zooms into one flow, such as booking through delivery, and adds commands, actors, policies and aggregates.
