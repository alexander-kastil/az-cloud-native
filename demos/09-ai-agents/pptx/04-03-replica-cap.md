---
slide: "04-03"
topic: 04-rate-limits
title: "Replica cap from the model quota"
subtitle: "KEDA rules on both queues, at most three calls in flight"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/replica-cap-scale-rules.svg"
media-file: labs/09-ai-agents/_images/replica-cap-scale-rules.svg
visual-prompt: >
  Existing diagram of exception-agent-scale.json with the urgent-exceptions and bulk-exceptions rules capping exception-agent at three replicas, each with one model call in flight to grey-chat, no generation. Referenced at its repo path.
order: 12
source: demos/09-ai-agents/04-rate-limits/readme.md
gamma_prompt: >
  Slide titled "Replica cap from the model quota", subtitle "KEDA rules on both queues, at most three
  calls in flight". Layout: Two columns. Left column: the bullets. Right column, wider than the left:
  the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Replica cap from the model quota

## Content

- An `azure-servicebus` rule scales on queue depth; **maxReplicas** is the fixed cap
- 30,000 TPM at about 2,000 tokens per call allows 15 calls per minute
- At 10 s per call, 2.5 calls run at once, so the cap is **three** replicas
- `MaximumConcurrentActivityWorkItems` of 1 keeps one model call in flight per replica
- API Management's **llm-token-limit** policy is the gateway option: 429 per minute, 403 when the quota runs out

## Speaker Notes

Azure Container Apps scales the agent with KEDA rules, and the model quota does not grow with the replicas. The diagram is the lab's step 6: the scale file, its two rules and the three replicas.

Scale rules: one azure-servicebus rule per queue, exceptions-urgent and exceptions-bulk, each with messageCount 1. KEDA polls about every 30 seconds, computes the replica count from queue length and messageCount, and maxReplicas is the fixed cap where the quota enters the design.

The arithmetic: the lab's chat deployment has 30,000 TPM and each call uses about 2,000 tokens, so it allows 15 calls per minute, one every four seconds. A call takes 10 seconds, so 2.5 calls must run at once and the cap is three.

Concurrency: keep replicas times per-replica concurrency at or below three in-flight calls. The lab sets MaximumConcurrentActivityWorkItems to 1 in Program.cs, so each replica holds one call.

Gateway option: the Azure API Management llm-token-limit policy limits tokens per minute for each counter key and returns 429 at the limit. It returns 403 when a longer token quota runs out, so the gateway can tell a short burst from an exhausted budget.
