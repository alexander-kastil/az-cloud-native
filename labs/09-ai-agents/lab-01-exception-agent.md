# Add an Exception Agent to the Event Flow

When a parcel misses its window, Contoso Logistics raises DeliveryDelayed, but no service reacts to it, so operators still write the customer message and the reroute by hand. At the end you hold an exception agent that consumes the event, drafts the customer message, and proposes a reroute for a person to approve. You also hold an embedding worker that indexes delivery notes for similarity search.

What you practice here: calling a Microsoft Foundry model from an idempotent, retried, quota-capped event consumer.

Duration: about 55 minutes.

Topics practiced: [Microsoft.Extensions.AI: Foundry Model Client](../../demos/09-ai-agents/01-extensions-ai/readme.md), [Agent Subscribers: React to Events](../../demos/09-ai-agents/02-agent-subscribers/readme.md), [LLM Calls: Durable Activities](../../demos/09-ai-agents/03-durable-llm-calls/readme.md), [Rate Limits: Priority Queues & KEDA](../../demos/09-ai-agents/04-rate-limits/readme.md), [Vector Search: Change Feed Embeddings](../../demos/09-ai-agents/05-vector-search/readme.md)

------

Open `labs/09-ai-agents/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack.

Before Step 2, run `labs/09-ai-agents/provision.azcli` in Bash or Azure Cloud Shell after `az login`. It creates the Microsoft Foundry project in `rg-azn-labs`. The commands need Azure CLI 2.80.0 or later. The Foundry resource name must be globally unique; if `contoso-foundry-09` is taken, change it in `provision.azcli` and `deprovision.azcli`.

---

## Step 1: Subscribe the Exception Agent to DeliveryDelayed

Overview: A delayed parcel raises DeliveryDelayed today, and nothing receives it. This step adds the ExceptionAgent worker with its own subscription, so every later step builds on a consumer that already receives the event.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. Create the worker project src/ExceptionAgent/ with ExceptionAgent.csproj and Program.cs. Reference src/ServiceDefaults and src/Contracts, add the project to ContosoLogistics.sln, and add it to the AppHost in src/AppHost/ as the resource named exception-agent.

Create a subscription named exception-agent on the Service Bus topic parcel-events, with a SQL filter on the application property type that passes only DeliveryDelayed. Create the subscription in the AppHost so the emulator has it at startup.

The worker receives messages from that subscription with Azure.Messaging.ServiceBus. Each body is a CloudEvents 1.0 envelope. Read the id attribute and the parcelId and shipmentId fields of the DeliveryDelayed record in src/Contracts/. Log the event id and the parcel id, then complete the message.

Add no model call, database or other queue in this step.
```

Expected Outcome: The AppHost dashboard shows exception-agent running. A DeliveryDelayed message sent to parcel-events logs one line with its event id and parcel id. A ShipmentBooked message on the same topic logs nothing from this worker.

## Step 2: Connect IChatClient to the Foundry Chat Deployment

Overview: The agent needs a model to draft the customer message. This step connects IChatClient to the contoso-chat deployment with Entra ID, so the agent holds no key.

Run this once in Bash or Azure Cloud Shell and keep the output for the next prompt:

```bash
az cognitiveservices account show --name contoso-foundry-09 --resource-group rg-azn-labs --query properties.endpoint --output tsv
```

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. In src/ExceptionAgent/, add the packages Microsoft.Extensions.AI, Microsoft.Extensions.AI.OpenAI, Azure.AI.OpenAI and Azure.Identity. Before you write any call, confirm its current API on Microsoft Learn through the Microsoft Learn MCP server listed in .mcp.json.

Read the endpoint from the configuration key Foundry:Endpoint and the chat deployment name from Foundry:ChatDeployment. The AppHost passes both to the exception-agent resource. The deployment name is contoso-chat. Authenticate with DefaultAzureCredential. Put no key in source, in configuration files or in the AppHost.

Create src/ExceptionAgent/ExceptionAgentChatClient.cs with one extension method, AddExceptionAgentChatClient, on IHostApplicationBuilder. It registers an IChatClient built from AzureOpenAIClient for the chat deployment, converted with AsIChatClient and wrapped with UseLogging and UseOpenTelemetry. Call it once from Program.cs.
```

Expected Outcome: The solution builds. The AppHost starts exception-agent with Foundry:Endpoint and Foundry:ChatDeployment set. A search of src/ExceptionAgent/ and src/AppHost/ finds no API key.

## Step 3: Run the Draft as a Durable Activity with Retry

Overview: A model call can be throttled, or it can fail after the service billed it. This step moves the draft into the DraftCustomerMessage activity, called by DelayedParcelOrchestration with a retry policy, so a replay reads the recorded result and never calls the model again.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. In src/ExceptionAgent/, add the orchestration DelayedParcelOrchestration and the activity DraftCustomerMessage, and register both with the Durable Task worker in Program.cs. The project already references the Durable Task packages from the previous lab. Confirm the TaskOptions, TaskRetryOptions and RetryPolicy API on Microsoft Learn first.

DelayedParcelOrchestration takes the DeliveryDelayed fields (event id, shipment id, parcel id) as input. It calls DraftCustomerMessage with a retry policy of 5 maximum attempts, a first retry interval of 2 seconds, a backoff coefficient of 2 and a maximum retry interval of 60 seconds. The orchestration makes no model call of its own.

DraftCustomerMessage takes the same input and calls the IChatClient from AddExceptionAgentChatClient. Its system message asks for a short customer notice about a delayed parcel. Set MaxOutputTokens to 1000, return the reply text and store nothing.

Start DelayedParcelOrchestration from the DeliveryDelayed handler, with the event id as the instance id.
```

Expected Outcome: If the first model call returns HTTP 429, the logs show a retry of DraftCustomerMessage, and the orchestration in the Durable Task dashboard completes with a draft after the second call.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - The stub throws HTTP 429 on its first call, the DraftCustomerMessage activity retries, and the orchestration completes with a draft after two stub calls.

## Step 4: Make the Delay Handling Idempotent and Propose the Reroute

Overview: Brokers deliver at least once, so a DeliveryDelayed event can arrive twice, and a model answer changes between calls. This step records each event before and after the model call, so a duplicate reuses the stored draft and creates no second proposal.

Research:

```text
Read the Service Bus duplicate detection page and the Idempotent Consumer pattern on Microsoft Learn. Explain which repeated deliveries duplicate detection stops and which ones reach the agent again. Propose where the processed-event marker is stored, the order of the check, the draft write and the proposal write, and the key for the reroute proposal. Do not write code.
```

Finding: A good answer says duplicate detection covers sends inside its 10-minute history window, and a redelivery after a lock expires still reaches the agent. It checks the marker before the model call, keys the proposal by event id and parcel id, and keeps the proposal and the marker in one partition so they can share a batch. Push back if the answer calls the model first and checks afterwards, or if it keeps processed events in memory, because a restart clears memory.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. In src/ExceptionAgent/, add a Cosmos DB container named agent-state, with partition key /parcelId, to the Cosmos DB database that shipping-api uses. Use the Cosmos DB emulator resource from the AppHost and the Azure.Cosmos package.

Before DelayedParcelOrchestration calls DraftCustomerMessage, read the item whose id is the DeliveryDelayed event id. If it exists with status processed, return its stored draft and skip the model call. Otherwise continue.

DraftCustomerMessage writes its result to agent-state under the event id, and returns the stored result when that item already exists.

After the draft, write a reroute proposal item whose id is the prefix reroute- followed by the event id, and a processed marker item whose id is the event id. Both carry the parcel id as partition key. Write them in one transactional batch.

After the batch succeeds, publish a RerouteRequested event with the event id, parcel id and shipment id to the parcel-events topic, using Azure.Messaging.ServiceBus. Complete the incoming message only after the batch succeeds. If a start for the same event id already exists, complete the message without starting a second orchestration.
```

Expected Outcome: Send the same DeliveryDelayed event twice. The agent-state container holds one draft item, one reroute proposal item and one processed marker, and parcel-events shows one RerouteRequested event. The second send reuses the stored draft and makes no model call.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - Publishing the same DeliveryDelayed twice stores one draft, publishes one RerouteRequested event and calls the stub model once.

## Step 5: Queue Urgent Exceptions Ahead of Bulk Work

Overview: A van breakdown can delay hundreds of parcels in minutes, and the urgent exceptions must not wait behind the bulk sweep. This step gives urgent exceptions their own queue and dispatches them first.

Research:

```text
Read the Priority Queue pattern and the Container Apps scale rules page on Microsoft Learn. Compare three layouts: one queue with a priority value, two queues drained by one consumer, and two queues each with its own consumer pool. Recommend one layout for the Contoso Logistics exception agent. Name where the Priority value of an exception should come from, given that DeliveryDelayed carries only the shipment id and the parcel id today. Do not write code.
```

Finding: A good answer names the starvation risk of a single consumer that drains urgent first, and it names the source of Priority as a value the publisher sets, not a guess from the free text of a delivery note. It says what the event contract gains from the field. Push back if the answer reads the delivery note text to decide urgency, because that mixes a model decision into routing.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. Add two Service Bus queues, exceptions-urgent and exceptions-bulk, to the Service Bus resource in src/AppHost/. Create both in the AppHost so the emulator has them at startup.

In src/ExceptionAgent/, change the DeliveryDelayed handler so it sends a message to exceptions-urgent when the application property Priority equals urgent, and to exceptions-bulk for any other value or no value. The message body carries the event id, parcel id and shipment id. Complete the incoming message after the send succeeds. Remove the direct start of DelayedParcelOrchestration from the handler.

Add src/ExceptionAgent/Queues/PriorityDispatcher.cs. The dispatcher keeps one message in flight. It receives from exceptions-urgent with a short wait, and it receives from exceptions-bulk only when the urgent receive returns nothing. For each message it starts DelayedParcelOrchestration with the event id as the instance id, and it completes the message after the orchestration starts.

Register PriorityDispatcher as a hosted service in src/ExceptionAgent/Program.cs.
```

Expected Outcome: Put three messages on exceptions-bulk and one on exceptions-urgent before PriorityDispatcher starts. The orchestration for the urgent event starts first, and the bulk events follow.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - With three bulk messages and one urgent message waiting when the dispatcher starts, the urgent message is processed first.

## Step 6: Cap the Replicas to the Model Quota with KEDA

Overview: KEDA adds replicas as the queues grow, and the model quota does not grow with them. This step caps the agent at three replicas, sized from the chat deployment's quota, and writes the scale rules for both queues.

Research:

```text
Read the Manage Azure OpenAI quota page and the Set scaling rules in Azure Container Apps page on Microsoft Learn. The contoso-chat deployment has 30,000 tokens per minute. Each call uses about 2,000 tokens and takes about 10 seconds. Work out the calls per minute the quota allows, the calls that must run at once to sustain that rate, and the replica cap that follows. Say what a caller sees above the quota and above the cap. Do not write code.
```

Finding: A good answer shows the arithmetic: 30,000 divided by 2,000 is 15 calls per minute, and 15 calls per minute at 10 seconds per call needs 2.5 calls in flight, which rounds up to 3. It says a burst above the quota returns HTTP 429, and that the activity retries it. Push back if the answer copies maxReplicas 10 from the topic example without this calculation, or if it misses that a backlog on the bulk queue can take all three replicas and make urgent work wait for a free one.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. Add src/ExceptionAgent/scale/exception-agent-scale.json. The file holds one scale block with minReplicas 0 and maxReplicas 3, and two rules of type azure-servicebus. The rule urgent-exceptions watches queue exceptions-urgent, and the rule bulk-exceptions watches queue exceptions-bulk. Each rule sets messageCount to 1 and namespace to the Service Bus namespace that the Container Apps deployment uses. The file holds no keys and no connection strings. Check the azure-servicebus metadata names on Microsoft Learn through the Microsoft Learn MCP server before you write the file.

In src/ExceptionAgent/Program.cs, set the Durable Task worker to process one activity at a time, using the concurrency option that the Durable Task SDK documents on Microsoft Learn. With one message in flight in the dispatcher, each replica then keeps one model call in flight.
```

Expected Outcome: exception-agent-scale.json parses as JSON, maxReplicas is 3, and each rule names its queue. With the cap in place, a backlog on exceptions-bulk does not hold back the urgent event, which still starts first.

## Step 7: Embed Delivery Notes from the Change Feed and Search Them

Overview: Delivery notes are free text, and keyword search misses most repeated problems. This step embeds each new note with the Python exception-agent, stores the vector on the note, and makes the notes searchable with VectorDistance.

Research:

```text
Read the Cosmos DB vector search page and the change feed guidance on Microsoft Learn. Compare two designs: the vector written back to the note item, and the vector written to a separate container. For each design, explain how the worker avoids embedding its own output again, and what a change of embedding model requires. Do not write code.
```

Finding: A good answer names the loop: the write-back creates another change feed entry, so the worker skips any note whose text hash is unchanged. It says a separate container removes the loop but adds a second read per note. It states that the vector policy is immutable, so a model or dimension change needs a new container and a full re-embedding run. Push back if the answer treats the change feed as exactly once.

Recipe:

```text
Work in labs/09-ai-agents/contoso-logistics/. Add a Cosmos DB container named delivery-notes, with partition key /parcelId, to the database that the AppHost provides for the Cosmos DB emulator. Create it with a vector embedding policy on /embedding (float32, 1536 dimensions, cosine distance) and a vector index on /embedding of type diskANN. Keep the default indexing for all other paths and exclude /_etag. Check the container and policy parameter names on Microsoft Learn first.

Create agents/exception-agent/embedding_worker.py and agents/exception-agent/requirements.txt with azure-cosmos, azure-identity and openai. The worker reads the change feed of delivery-notes. For each note whose textHash differs from the hash of its text, it requests an embedding from the deployment named by the environment variable EMBEDDING_DEPLOYMENT, at the endpoint in FOUNDRY_ENDPOINT, using DefaultAzureCredential. It stores the vector in the note's embedding property and the new textHash in the same upsert.

Add a function similar_notes(container, vector) that runs a query with TOP 5 that orders by VectorDistance over the embedding property. Read both environment values from the environment, never from source. The AppHost sets EMBEDDING_DEPLOYMENT to contoso-embed and FOUNDRY_ENDPOINT to the Foundry endpoint from Step 2.
```

Expected Outcome: The new note holds an embedding of 1,536 numbers. With the contoso-embed deployment, similar_notes called with the text of one stored note returns that note first.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - A new delivery note gets an `embedding` property with 1,536 numbers from the Python embedding worker.

## Solution

The finished state is in `labs/09-ai-agents/contoso-logistics-solution/`.

## Cleanup

Run `labs/09-ai-agents/deprovision.azcli` in Bash or Azure Cloud Shell. It deletes both model deployments, the project and the Foundry resource, purges the soft-deleted account, and deletes `rg-azn-labs`. The local files stay in your clone.
