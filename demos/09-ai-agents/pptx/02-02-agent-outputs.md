---
slide: "02-02"
topic: 02-agent-subscribers
title: "Output as a command or an event"
subtitle: "A draft for review, a reroute proposal for a person to approve"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/09-ai-agents/pptx/images/02-02-agent-outputs.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif, module palette, minimum 27px type. No heading inside the image. No arrow crosses a box or a
  label. Amber box "DeliveryDelayed" "CloudEvents 1.0 input" with an arrow down into a tall blue box
  "exception-agent" "subscription filtered by event type" "keeps its draft in agent-state". Two lanes
  to the right. Lane one: purple "NotifyCustomer" "command (design)" "draft customer message", blue
  "notification-service" "one named service", green "held for review until an operator approves it".
  Lane two: amber "RerouteRequested" "event" "reroute proposal", blue "routing-service" "ops-console
  (design)", green "a person approves through the reroute-approval endpoint", an arrow down to amber
  "RerouteApproved" "event" and left to purple "orchestration" "from Module 8" "calls RerouteParcel".
  Grey note box bottom left: "Each draft and proposal carries the event id and parcel id" and "Routing
  owns the van assignments; the agent proposes".
order: 6
source: demos/09-ai-agents/02-agent-subscribers/readme.md
gamma_prompt: >
  Slide titled "Output as a command or an event", subtitle "A draft for review, a reroute proposal for
  a person to approve". Layout: Two columns. Left column: the bullets. Right column, wider than the
  left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit
  the column, never cropped, never used as a background or side accent. Use exactly the one image given
  for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Output as a command or an event

## Content

- A **command** asks one named service to act; an **event** states a fact any service can use
- The draft customer message is the `NotifyCustomer` command (design), held until an operator approves it
- The reroute proposal is the `RerouteRequested` event, the one message the lab's agent sends
- A person approves through the reroute-approval endpoint, which raises `RerouteApproved`
- The agent never calls RerouteParcel: the routing service owns the van assignments

## Speaker Notes

After reading the event, the agent writes back, and this slide shows what it sends and who acts on it. In the lab the agent sends one kind of message, an event, and keeps its draft in the agent-state container.

Command or event: a command asks one named service to act, and an event states a fact that any interested service can use. The NotifyCustomer command to notification-service is a design, and the draft is held for review until an operator approves it.

Reroute proposal: RerouteRequested goes to routing-service and the ops-console, both a design with no lab consumer. A person approves through the reroute-approval endpoint, which raises RerouteApproved, and the durable orchestration from Module 8 then calls RerouteParcel.

Traceability: the draft and the proposal are both decisions made by a model, so both carry the event id and parcel id that produced them. Downstream services can then trace each suggestion to the delay that caused it.

Ownership: the agent never calls RerouteParcel directly, because the routing service owns the van assignments. The human approval closes the loop.
