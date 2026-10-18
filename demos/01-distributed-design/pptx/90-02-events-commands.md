---
slide: "90-02"
topic: 01-distributed-design
title: "Events, commands and aggregates in the lab"
subtitle: "Steps 1 and 2 write docs/events.md"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/01-distributed-design/_images/command-aggregate-event.svg"
media-file: labs/01-distributed-design/_images/command-aggregate-event.svg
visual-prompt: >
  Existing diagram of shipping-api sending BookShipment to the Shipment aggregate, which emits ShipmentBooked, and of ReserveCapacity and RerouteParcel accepted by the Route aggregate, which emits CapacityReserved and RerouteApproved, no generation. Referenced at its repo path.
order: 19
source: labs/01-distributed-design/readme.md
gamma_prompt: >
  Slide titled "Events, commands and aggregates in the lab", subtitle "Steps 1 and 2 write
  docs/events.md". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Events, commands and aggregates in the lab

## Content

- Step 1 lists 13 events, each with a **kind** and a **style**
- ParcelScanned carries the scan types PickedUp, AtHub and OutForDelivery
- Step 2 adds an Emitted by column and 7 **commands**
- Every Accepted by value matches a row in the Aggregates table
- Steps 3 to 5 draw the contexts, pick a broker per flow and add the patterns

## Speaker Notes

Events come first, because each one records a fact that already happened and every later decision refers back to them. The diagram shows how each command reaches the one aggregate that accepts it.

Kind and style: each of the 13 events gets a journey stage, a kind and a style. The kind decides whether a fact stays inside one service, the style whether a consumer must call back for detail.

Scan types: ParcelScanned carries the parcel ID and a scan type with the values PickedUp, AtHub and OutForDelivery.

Commands: BookShipment, ReserveCapacity, CapturePayment, PrintLabel, AssignRoute, RerouteParcel and NotifyCustomer, each with Sent by, Accepted by and Emits.

Aggregates: every command has one aggregate that decides, such as Shipment for BookShipment and Route for ReserveCapacity and RerouteParcel.

The rest of the lab: step 3 groups the aggregates into bounded contexts, step 4 assigns each of seven flows to HTTP or one Azure broker, and step 5 names a reliability pattern per flow and the saga compensations.
