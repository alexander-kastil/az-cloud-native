---
slide: "90-01"
topic: 09-ai-agents
title: "What the lab builds"
subtitle: "An exception agent on DeliveryDelayed, in seven steps"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/09-ai-agents/_images/lab-overview.svg"
media-file: labs/09-ai-agents/_images/lab-overview.svg
visual-prompt: >
  Existing diagram of the exception agent and the embedding worker after step 7, from the exception-agent subscription on parcel-events through the urgent and bulk queues, PriorityDispatcher, DelayedParcelOrchestration, agent-state and contoso-chat, with the step that adds each piece, no generation. Referenced at its repo path.
order: 17
source: labs/09-ai-agents/readme.md
gamma_prompt: >
  Slide titled "What the lab builds", subtitle "An exception agent on DeliveryDelayed, in seven steps".
  Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram
  as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped,
  never used as a background or side accent. Use exactly the one image given for this card; do not
  generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# What the lab builds

## Content

- At the start no service reacts to DeliveryDelayed, and operators write the message and reroute by hand
- Steps 1 and 2: a filtered subscription and **IChatClient** on contoso-chat with Entra ID
- Steps 3 and 4: a durable draft activity with retries, and one batch per event in agent-state
- Steps 5 and 6: urgent work ahead of bulk, and KEDA capped at **three replicas**
- Step 7: a Python embedding-worker indexes delivery notes for VectorDistance search

## Speaker Notes

The lab turns the module into one running agent, in about 55 minutes. At the start Contoso Logistics publishes DeliveryDelayed and no service reacts, so operators still write the customer message and the reroute by hand.

Subscribe and connect: step 1 adds the ExceptionAgent worker with its own subscription to DeliveryDelayed. Step 2 connects IChatClient to the contoso-chat deployment with Entra ID, so the agent holds no key.

Draft once: step 3 moves the draft into DraftCustomerMessage, called by DelayedParcelOrchestration with a retry policy. Step 4 adds the processed check and writes the proposal and the marker in one batch before RerouteRequested goes out.

Stay in quota: step 5 sends urgent exceptions to their own queue, which PriorityDispatcher drains first. Step 6 caps the agent at three replicas, sized from the chat deployment's quota.

Search and setup: step 7 adds the Python embedding worker on the delivery-notes change feed and a VectorDistance query. Run provision.azcli after az login before step 2 and deprovision.azcli at the end; the finished state is in contoso-logistics-solution.
