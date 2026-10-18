---
slide: "00-02"
topic: 09-ai-agents
title: "Overview"
subtitle: "The path through the AI agents module"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/09-ai-agents/pptx/images/00-02-module-path.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif in the module's diagram palette, text #0F172A and #64748B, arrows #334155, minimum 27px type. No
  heading inside the image; the slide title carries it. No arrow crosses a box or a label. Two rows of
  three numbered rounded cards, read left to right, each card a two-digit number, the topic title from
  the module readme split at its colon, and one secondary line. Top row blue #DBEAFE with #1D4ED8,
  caption above it "The model as one step in the flow": 01 Microsoft.Extensions.AI: Foundry Model Client
  "IChatClient, Entra ID", 02 Agent Subscribers: React to Events "CloudEvents, idempotency", 03 LLM Calls:
  Durable Activities "CallActivityAsync, 429 retries". Bottom row green #DCFCE7 with #15803D, caption
  above it "Quota, search and coordination": 04 Rate Limits: Priority Queues & KEDA "Token quotas,
  maxReplicas", 05 Vector Search: Change Feed Embeddings "diskANN, VectorDistance", 06 Agent
  Choreography: Events vs. A2A "Agent cards, A2A 1.0". Short arrows between neighbours, one elbow
  connector from card 03 down between the rows to card 04, routed above the second caption.
order: 2
source: demos/09-ai-agents/readme.md
gamma_prompt: >
  Slide titled "Overview", subtitle "The path through the AI agents module". Layout: Two columns. Left
  column: the bullets. Right column, wider than the left: the provided diagram as an inline content
  image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never used as a
  background or side accent. Use exactly the one image given for this card; do not generate or add any
  other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Overview

## Content

- Call a Foundry model through **IChatClient** with Entra ID and OpenTelemetry
- Subscribe the agent to DeliveryDelayed and keep it **idempotent**
- Run the model call as a **durable activity** that retries on 429
- Hold the agent inside its quota with **priority queues** and a KEDA cap
- Index delivery notes for **vector search**, then choose events or A2A

## Speaker Notes

This slide walks the six topics once, following the three themes of the title slide. Throughout the module the model is one slow, non-deterministic, non-idempotent step in a larger flow.

Model client: IChatClient over AzureOpenAIClient with DefaultAzureCredential, with logging and OpenTelemetry middleware on every call.

Subscriber and activity: the agent consumes DeliveryDelayed like any other service and checks a processed marker, and the model call runs as a durable activity that retries on 429. The lab combines both, with the subscriber starting the orchestration that calls the activity.

Quota: separate priority queues and a KEDA scale rule with maxReplicas hold the agent to the deployment's token and request quotas.

Search and coordination: change feed triggers fill a diskANN vector index for VectorDistance queries, and the module ends with choreography against A2A agent cards. Agent fundamentals, MCP servers and A2A hosting in depth belong to the pro-code-agents class.
