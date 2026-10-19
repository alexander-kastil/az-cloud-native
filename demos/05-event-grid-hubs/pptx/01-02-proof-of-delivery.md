---
slide: "01-02"
topic: 01-custom-topics
title: "Blob Storage system topic for proof of delivery"
subtitle: "A finished photo upload becomes ParcelDelivered"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/05-event-grid-hubs/_images/photo-to-parcel-delivered.svg"
media-file: labs/05-event-grid-hubs/_images/photo-to-parcel-delivered.svg
visual-prompt: >
  Existing diagram of the photo CL-10482.jpg in proof-of-delivery raising BlobCreated on the system topic egst-pod-labs05, the pod-events queue, and ProofOfDeliveryWorker publishing ParcelDelivered and deleting the message only after the publish succeeds, no generation. Referenced at its repo path.
order: 4
source: demos/05-event-grid-hubs/01-custom-topics/readme.md
gamma_prompt: >
  Slide titled "Blob Storage system topic for proof of delivery", subtitle "A finished photo upload
  becomes ParcelDelivered". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Blob Storage system topic for proof of delivery

## Content

- Blob Storage raises `Microsoft.Storage.BlobCreated` once a block blob is fully committed
- A **system topic** turns those storage events into Event Grid events
- `subjectBeginsWith` limits the subscription to the proof-of-delivery container
- The worker reads the tracking number from the blob name and publishes `ParcelDelivered`
- Create the account as general purpose v2, because v1 can't publish to Event Grid

## Speaker Notes

A delivery app uploads each proof-of-delivery photo to Blob Storage, and that upload is the moment the parcel counts as delivered. This slide follows one photo until a partner receives `ParcelDelivered`.

BlobCreated: the storage account publishes `Microsoft.Storage.BlobCreated` when a block blob is completely committed, which means the upload has finished.

System topic: it represents the blob events of one storage account and turns them into Event Grid events that subscriptions can route. Event Grid recommends creating the system topic first, so it stays in place after its last subscription is deleted.

Subject filter: the event carries the blob path in its subject, and `subjectBeginsWith` on the proof-of-delivery path keeps other blobs away from the handler.

The worker: it reads each event from the `pod-events` storage queue, takes the tracking number from the file name without downloading the photo, and publishes `ParcelDelivered` to the custom topic. In the lab it deletes the queue message only after the publish succeeds, so a failed publish shows the message again after one minute.

General purpose v2: a general purpose v1 account can't publish to Event Grid, so create the storage account as v2.
