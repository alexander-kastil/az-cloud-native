---
slide: "04-02"
topic: 04-read-models
title: "Session tokens for read-your-writes"
subtitle: "The token travels with the next request to another instance"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/06-event-sourcing-cqrs/pptx/images/04-02-session-tokens.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial,
  sans-serif, Consolas for API names, module palette (client grey #F1F5F9 with #64748B, instances blue
  #DBEAFE with #1D4ED8, container green #DCFCE7 with #15803D), the token path in purple #6D28D9,
  minimum 26px type. No heading inside the image. No arrow crosses a box or a label. Box "Browser" (one
  session token per tracking number) on the left, boxes "Instance A" (own SDK client) top centre and
  "Instance B" (own SDK client, no cached token) bottom centre, cylinder "tracking" (Cosmos DB container,
  partition /trackingNumber) on the right. Seven numbered elbow arrows routed in the gaps: 1 request that
  writes a scan, 2 UpsertItemAsync, 3 session token in Headers.Session, 4 response plus the session
  token, 5 next request with the token, 6 ReadItemAsync with SessionToken in ItemRequestOptions, 7 the
  scan instance A wrote. Steps 3 to 6 carry the token and are purple.
order: 11
source: demos/06-event-sourcing-cqrs/04-read-models/readme.md
gamma_prompt: >
  Slide titled "Session tokens for read-your-writes", subtitle "The token travels with the next request to
  another instance". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column,
  never cropped, never used as a background or side accent. Use exactly the one image given for this
  card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Session tokens for read-your-writes

## Content

- **Session** is the default consistency level: a client reads its own writes
- Each write returns a session token in `Headers.Session`
- Several web instances don't share the SDK's cached token
- The browser gets the token and sends it back with the next request
- Tokens are partition bound, so keep one per tracking number

## Speaker Notes

The last setting on the read model is consistency, and it decides whether a user sees their own scan right away. The diagram follows one write on instance A and the next read on instance B.

Session level: session is the default consistency level for Cosmos DB accounts, and within one session a client reads its own writes and sees writes in order. The five levels are strong, bounded staleness, session, consistent prefix and eventual.

Session token: each write returns a token in Headers.Session, and the SDK keeps the most recent token for each client. That works while the same client instance serves the user.

Several instances: a web tier with several instances loses that cache, because instance B has its own SDK client. Learn notes that a client without a token for a partition reads with eventual behavior, so the scan can be missing.

Token round trip: the response carries the token to the browser, and the next request brings it back. Instance B passes it as SessionToken in ItemRequestOptions on ReadItemAsync and reads the scan that instance A wrote.

Partition bound: session tokens are bound to a partition, so the browser keeps one token per tracking number. Pass the token along with the request that reads back the write.
