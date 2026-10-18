---
slide: "04-02"
topic: 04-event-types
title: "Notification and state transfer events"
subtitle: "What the payload holds decides whether the consumer asks back"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/01-distributed-design/04-event-types/_images/notification-vs-state-transfer.svg"
media-file: demos/01-distributed-design/04-event-types/_images/notification-vs-state-transfer.svg
visual-prompt: >
  Existing diagram of two lanes, DeliveryDelayed as a notification that sends the exception agent back to Delivery for the delivery attempts, and ParcelDelivered as a state transfer that Billing and Notifications close their records with, no generation. Referenced at its repo path.
order: 11
source: demos/01-distributed-design/04-event-types/readme.md
gamma_prompt: >
  Slide titled "Notification and state transfer events", subtitle "What the payload holds decides
  whether the consumer asks back". Layout: Two columns. Left column: the bullets. Right column, wider
  than the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio,
  scaled to fit the column, never cropped, never used as a background or side accent. Use exactly the
  one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Notification and state transfer events

## Content

- A **notification** carries the fact, the identifiers and the time
- A consumer that needs more detail calls the owner in a second request
- A **state transfer** event carries the new state, and the consumer upserts its copy
- DeliveryDelayed is a notification, ParcelDelivered a state transfer
- Name events after business facts, such as **ParcelDelivered**

## Speaker Notes

Once an event crosses a boundary, its payload decides how often the consumer must ask back. The two lanes show one Contoso example of each style.

Notification: ParcelScanned carries the parcel ID and the scan type, and Tracking appends it to its own read model without calling Booking.

Second request: DeliveryDelayed carries the shipment, the delay reason and the new arrival time, but a reroute decision also needs the delivery attempts that Delivery owns. In the design the exception agent calls Delivery for them; the lab's agent only logs the event.

State transfer: the Azure Architecture Center describes a message that carries the resulting absolute state, which the consumer applies as an upsert to its own copy.

ParcelDelivered: it carries the final status, the recipient name and the delivery time, so Billing and Notifications close their own records without a follow-up request.

Business names: publishing database rows leaks internal structure, and a generic name such as RecordChanged makes every consumer inspect the payload. A name such as ParcelDelivered tells the consumer what happened.
