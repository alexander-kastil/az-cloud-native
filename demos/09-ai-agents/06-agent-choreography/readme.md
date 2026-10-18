---
minutes: 10
---

# Agent Choreography: Events vs. A2A

A delayed parcel touches several services at once. Routing proposes a reroute, notification prepares the customer message, the exception-agent explains the delay, and the ops console shows the result.

The question for the design is how these agents coordinate. They can react to events that someone publishes, or one agent can call another agent directly and wait for its answer. This topic compares the two styles, covering choreography through published events, direct calls with the A2A protocol and its agent cards, the governance of A2A 1.0, and a decision table for each interaction.

## Choreography: Agents React to Published Events

In choreography, a producer publishes a fact, such as DeliveryDelayed, and every interested service subscribes to it. No central component tells each service what to do next. The exception-agent reacts to DeliveryDelayed, routing reacts to RerouteRequested, and notification reacts to NotifyCustomer, and the business flow emerges from those reactions.

The Architecture Center describes the choreography pattern as a way for services to control a business transaction through messaging. Its guidance lists the issues you inherit: duplicate and out-of-order messages, event schema changes that break consumers, and event storms, where one event triggers a chain of reactions. Use event filters, consumer concurrency limits, and explicit rules as guardrails against those chains.

Choreography fits when the agents are peers and new consumers appear over time. Add a reporting consumer next quarter, and the publisher changes nothing. Module 8 covers the saga orchestration style, which fits a flow that needs compensations and a single place to track progress.

## Direct Calls with A2A

The A2A protocol lets one agent call another agent as a request and response exchange. The callee publishes an agent card at the well-known path /.well-known/agent-card.json.

The card describes the agent's name, its skills, the interfaces it supports, and the authentication it accepts. A caller reads the card, chooses a binding, and sends a task.

```bash
curl https://<agent-host>/.well-known/agent-card.json
```

The A2A 1.0 specification defines three bindings, JSON-RPC 2.0, gRPC, and HTTP with JSON, and it permits custom bindings. An agent advertises the bindings it supports in its card, so a caller can pick one it understands. Microsoft Agent Framework can expose a remote A2A agent as a tool for a Foundry agent, and the pro-code-agents class covers that hosting work.

A2A works well when the caller needs one answer and waits for it. The ops console might ask the exception-agent to explain one parcel's delay while an operator watches. The caller owns the timeout, and the callee owns the task state.

## A2A Governance and Adoption

A2A 1.0 is the first stable release of the specification. The Linux Foundation announced it on April 9, 2026, and the press release describes the project as hosted by the Linux Foundation, with more than 150 supporting organizations.

Since August 17, 2026, A2A is a hosted project of the Agentic AI Foundation, which operates under the Linux Foundation.

Governance matters for a design that spans vendors. An agent built on one platform can call an agent built on another, and both can rely on the same card format and the same bindings.

## Decision Table: Events or A2A

Use the table to choose a style for each interaction. Most flows in the Contoso Logistics system should start as choreography, and only the interactions that need a single answer should use A2A.

| Question | Choreography | A2A |
| --- | --- | --- |
| Who knows about whom | The publisher knows nothing about its consumers | The caller knows the callee's agent card |
| Is a response needed | No, consumers publish their own follow-up events | Yes, the caller waits for a result or a task state |
| How many consumers | Any number, added later without changes to the publisher | One named agent per call |
| Failure handling | Retries, dead-letter queues, idempotent consumers | Timeouts and retries in the caller, task state in the callee |
| Audit trail | The event log and each consumer's processed-events store | Task records in each agent |
| Contoso example | DeliveryDelayed fans out to the exception-agent and notification-service | The ops console asks the exception-agent to explain one delay |

The decision rests on two questions. Does the caller need an answer now, and will more consumers appear later? A yes to the first points to A2A, and a yes to the second points to events.

## Key Topics

- [Choreography pattern](https://learn.microsoft.com/azure/architecture/patterns/choreography)
- [Saga distributed transactions pattern](https://learn.microsoft.com/azure/architecture/patterns/saga)
- [A2A protocol specification](https://a2a-protocol.org/latest/specification/)
- [A2A protocol surpasses 150 organizations (Linux Foundation)](https://linuxfoundation.org/press/a2a-protocol-surpasses-150-organizations-lands-in-major-cloud-platforms-and-sees-enterprise-production-use-in-first-year)
- [Microsoft Foundry model provider for Agent Framework](https://learn.microsoft.com/agent-framework/integrations/by-provider/microsoft-foundry)
