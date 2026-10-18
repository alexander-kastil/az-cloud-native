---
slide: "03-02"
topic: 03-state-stores
title: "Write modes and transactions"
subtitle: "Choosing a concurrency mode and changing several keys at once"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/07-dapr/pptx/images/03-02-write-modes.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-serif,
  the module's palette, minimum 26px type. No heading inside the image. No arrow crosses a box or a
  label. Three tall cards side by side. Blue card "Last-write-wins" and green card "First-write-wins",
  each with four rows separated by thin rules: ETag ("no ETag" / "the ETag from the read"), write rule
  ("the newest write replaces the value" / "succeeds only when the ETag matches"), mode
  (ConcurrencyMode.LastWrite / ConcurrencyMode.FirstWrite), Contoso Logistics example ("driver note on a
  parcel" / "booking status moves from Booked to PickedUp"). Purple card "Transaction" with
  ExecuteStateTransactionAsync, "one request", two white rows SHP-1001 Delete and SHP-1002 Delete, an
  arrow down to a white result box with green border "succeeds for every key or fails for every key".
order: 8
source: demos/07-dapr/03-state-stores/readme.md
gamma_prompt: >
  Slide titled "Write modes and transactions", subtitle "Choosing a concurrency mode and changing
  several keys at once". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Write modes and transactions

## Content

- Without an ETag the newest write replaces the value: **last-write-wins**
- Use first-write-wins for a transition that must happen once
- `StateOptions.Consistency`: eventual by default, strong waits for replicas or a quorum
- `ExecuteStateTransactionAsync` succeeds for every key or fails for every key
- `state.azure.cosmosdb` is Tier 1 and needs the partition key path `/partitionKey`

## Speaker Notes

The ETag check is one of two write modes, and the right one depends on the data. This slide also covers changes that touch several keys and the Cosmos DB store behind the booking data.

Last-write-wins: without an ETag the newest write replaces the stored value. That is fine where only the latest value matters, such as a driver's note on a parcel.

First-write-wins: use it when two writers race for a transition that must happen once, such as a booking status moving from Booked to PickedUp.

Consistency is a separate choice: eventual consistency, the default, acknowledges as soon as the store accepts the write. Strong consistency waits for all replicas or a designated quorum.

Transactions: `ExecuteStateTransactionAsync` takes a list of `StateTransactionRequest` entries, and the store applies them atomically. A transaction covers one request and does not replace the ETag check for a read-modify-write across separate requests.

Cosmos DB store: the `state.azure.cosmosdb` component is Tier 1 on Container Apps and needs URL, key, database and collection. With a managed identity, grant Cosmos DB Built-in Data Contributor and drop `masterKey`.
