---
slide: "04-03"
topic: 04-event-types
title: "The CloudEvents 1.0 envelope"
subtitle: "One envelope that consumers read without the producer's format"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/01-distributed-design/pptx/images/04-03-cloudevents-envelope.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, the module's own diagram
  language: 'Segoe UI', Inter, Arial, sans-serif, Consolas for attribute names and values, text
  #0F172A, notes #334155, minimum 26px type. No heading inside the image. No line crosses a box or a
  label. On the left a white panel labelled "ParcelScanned notification" holds the example from the
  topic readme in JSON order: a blue "required" group with specversion "1.0", type
  "com.grey.logistics.ParcelScanned", source "/grey/logistics/scanners/hub-ams-01" and id
  "5f0c2a7e-3b1d-4c8a-9e62-7d41b8a0c913"; a purple "optional" group with time "2026-10-18T09:14:22Z",
  subject "parcels/CL-2026-000142" and datacontenttype "application/json"; an amber "data" box with
  parcelId, shipmentId, trackingNumber "CL-2026-000142", scanType "AtHub", scanLocation "Vienna Hub" and
  scannedAt. On the right, short connector lines lead from rows to notes: type "names the business
  fact", source "identifies the producer", id "unique per event; a consumer uses it to detect a
  duplicate", subject "names the parcel, so routing and filtering work without parsing data", data
  "carries the payload".
order: 12
source: demos/01-distributed-design/04-event-types/readme.md
gamma_prompt: >
  Slide titled "The CloudEvents 1.0 envelope", subtitle "One envelope that consumers read without the
  producer's format". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# The CloudEvents 1.0 envelope

## Content

- Four required attributes: **id**, source, specversion and type
- Optional attributes are datacontenttype, subject and time; **data** carries the payload
- The id is unique per event, so consumers use it to detect duplicates
- The **subject** names the parcel, so brokers filter without parsing data
- Event Grid namespace topics and Dapr carry it; Service Bus carries plain JSON

## Speaker Notes

Brokers and routing services need one envelope, so a consumer can read the type, source and ID without knowing the producer's internal format. CloudEvents is a CNCF graduated project, and the diagram shows a ParcelScanned notification inside its envelope.

Required attributes: specification version 1.0.2 defines four, id, source, specversion and type. The source identifies the producer and the type names the business fact.

Optional attributes: datacontenttype, subject and time are optional. Data carries the payload, here the parcel and shipment IDs, the tracking number, the scan type, the location and the scan time.

Duplicates: the id must be unique per event, and it is the attribute a consumer uses to detect a duplicate.

Filtering: brokers that route by attribute can match on type and subject, so a subscriber selects ParcelScanned events for one parcel without reading the payload.

Where it applies: Event Grid namespace topics accept CloudEvents 1.0 over HTTP in JSON, and Dapr wraps every pub/sub message in the envelope. The Service Bus messages in the labs carry plain JSON with a subject.
