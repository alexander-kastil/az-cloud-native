---
slide: "04-01"
topic: 04-rate-limits
title: "Model quotas and back pressure"
subtitle: "The queues absorb a burst below the 429 line"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/09-ai-agents/04-rate-limits/_images/back-pressure.svg"
media-file: demos/09-ai-agents/04-rate-limits/_images/back-pressure.svg
visual-prompt: >
  Existing diagram of a DeliveryDelayed burst from booking-worker filling exceptions-urgent and exceptions-bulk while KEDA adds up to three exception-agent replicas and the chat deployment stays at about 15 calls per minute below its 429 line, no generation. Referenced at its repo path.
order: 10
source: demos/09-ai-agents/04-rate-limits/readme.md
gamma_prompt: >
  Slide titled "Model quotas and back pressure", subtitle "The queues absorb a burst below the 429
  line". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card;
  do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Model quotas and back pressure

## Content

- Each deployment has a **TPM** allocation, the RPM limit follows from it, and either limit returns 429
- The token estimate counts the prompt plus **max_tokens**, so size max_tokens to a customer notice
- Failed requests count against the budget; back off with jitter and a fixed maximum
- The **queues absorb the burst**: booking-worker keeps publishing, the agent drains at the quota's rate
- Producers never see the 429; the backlog shows up as queue depth

## Speaker Notes

Topic 04 starts from a burst: a van breakdown can delay 300 parcels in ten minutes, and the Foundry deployment has a fixed budget of tokens and requests per minute. The diagram shows the plan that keeps the deployment below its 429 line.

Quotas: each deployment receives a tokens-per-minute allocation, the requests-per-minute limit follows from it, and a request over either limit gets a 429. The RPM limit is checked over short windows, so a burst can be throttled while the minute total is still below the limit.

Token estimate: each request counts as its prompt plus its max_tokens value, because the rate limit does not wait for the real output size. A large max_tokens reserves budget the model may never spend, so set it to the size of a customer notice.

Backing off: failed requests still count, so retries during a burst make throttling worse unless they back off. The Azure OpenAI SDKs retry 429 by default and respect the retry-after headers, and the Foundry quota guidance recommends exponential backoff with jitter and a fixed maximum.

Back pressure: booking-worker keeps publishing DeliveryDelayed at its own pace while the agent drains the queues at the rate the quota allows. Producers never see the 429, and operators see the backlog as queue depth on the dashboard.
