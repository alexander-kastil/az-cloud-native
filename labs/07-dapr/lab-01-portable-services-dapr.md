# Make the Services Portable with Dapr

Each Contoso Logistics service talks to its broker, its store and its peers through a vendor SDK, so changing a broker means changing code. Routing must also stop two requests from assigning one parcel to two vans, or from overfilling one van. At the end you hold the scan, tracking, notification and routing flows on Dapr building blocks, with the lab 4 and lab 6 gates still green and the same components set on Container Apps.

What you practice here: swapping an infrastructure dependency behind a named building block, then proving the swap with the gates you already have.

Duration: about 65 minutes.

Topics practiced: [Dapr Sidecar: Run with Aspire](../../demos/07-dapr/01-sidecar/readme.md), [Service Invocation: mTLS & Retries](../../demos/07-dapr/02-service-invocation/readme.md), [State Stores: ETags & Concurrency](../../demos/07-dapr/03-state-stores/readme.md), [Pub/Sub: Swap Brokers via Components](../../demos/07-dapr/04-pub-sub/readme.md), [Resiliency Policies: Retries & Breakers](../../demos/07-dapr/05-resiliency/readme.md), [Dapr Actors: Virtual Actor Model](../../demos/07-dapr/06-actors/readme.md), [Managed Dapr: Container Apps vs. AKS](../../demos/07-dapr/07-managed-dapr/readme.md)

------

Open `labs/07-dapr/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack.

---

## Gate Tests

Run one gate with `dotnet test tests/ContosoLogistics.Tests --filter "Gate=<GateName>"`. Each gate is already in the starter and fails until its step is done.

| Gate | What it proves |
| --- | --- |
| `SidecarsStartWithAppHost` | The AppHost starts tracking-service, notification-service and shipping-api, and each one has a Dapr sidecar that reports healthy. |
| `PubSubFlowsUseDapr` | Tracking-service publishes ParcelScanned messages through the `pubsub` component, notification-service receives them through a Dapr subscription, and neither service references `Azure.Messaging.ServiceBus`. |
| `RoutingStateUsesETags` | A second assignment of the same parcel returns a conflict, and a write with a stale ETag reads again and retries before it gives up. |
| `ShippingCallsRoutingThroughSidecar` | A booking reaches routing-service by app ID through the shipping-api sidecar and receives an answer. |
| `RoutingCallRetryAndBreak` | A transient routing-service failure succeeds after a retry, and the circuit breaker opens after the configured run of consecutive failures. |
| `VanActorSerialisesAssignments` | Concurrent assignments to one van never push it past its capacity limit. |
| `PortableScanFlow` | The lab 4 scan gates (duplicate message, poison message, scan order per parcel) still pass with the scan path on Dapr pub/sub. |
| `PortableShipmentFlow` | The lab 6 gates (shipment concurrency check, outbox, change feed projection, Redis cache invalidation) still pass after the swap. |
| `ContainerAppsComponentsTier1` | The Azure component examples name Tier 1 component types, leave every secret empty, and scope each component to app IDs. |

## Step 1: Run the Services with Dapr Sidecars

Overview: Each service gets a Dapr sidecar from the AppHost, and the sidecar loads two local components named `pubsub` and `statestore`, both backed by Redis. Run `dapr init` once on your machine before this step, as topic 01 describes. Later steps call these names, so a change of broker or store stays in the component files.

Finding: The Aspire dashboard lists a sidecar beside tracking-service, notification-service and shipping-api, and both components load. Push back if a sidecar starts with no component file behind it, because that sidecar serves no building block.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

Add the CommunityToolkit.Aspire.Hosting.Dapr package to src/AppHost. In src/AppHost/Program.cs, enable Dapr and attach a Dapr sidecar to tracking-service, notification-service and shipping-api. Register two components named pubsub and statestore, both backed by Redis, and load them from the files src/AppHost/components/pubsub-local.yaml and src/AppHost/components/statestore-local.yaml. Use the pubsub.redis and state.redis component types. Keep the Service Bus and Cosmos DB emulators running for the flows that have not moved yet.

Do not change any service logic in this step. Make the gate test SidecarsStartWithAppHost in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: The AppHost starts without errors. The dashboard shows a sidecar for each of the three services, and both components show as loaded. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=SidecarsStartWithAppHost"` passes.

## Step 2: Publish and Subscribe through Dapr Pub/Sub

Overview: The tracking and notification flows stop calling the Service Bus SDK and use the `pubsub` component by name, so the broker becomes a component file. Dapr wraps each message in a CloudEvents envelope, and that envelope changes what a subscriber reads.

Research:

```text
Read the Dapr pub/sub overview and the Azure Service Bus topics component page on docs.dapr.io. Explain how a publish that carries a SessionId metadata value reaches a subscriber, what the CloudEvents envelope adds to the message, and which Service Bus features the Dapr path does not expose. Name each page you used.
```

Finding: A good answer names the publish call on the `pubsub` component with the topic `parcel-events`, the SessionId metadata set to the parcel id, the payload under the `data` field of the envelope, and a subscription declared with the Topic attribute or a subscription file. Push back if the answer keeps `Azure.Messaging.ServiceBus` in the publish path, or if it says sessions work end to end through Dapr. The topic page documents the session id on publish but not a session receiver.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

In src/tracking-service and src/notification-service, replace each Azure.Messaging.ServiceBus publisher and receiver in the tracking and notification flows with Dapr pub/sub. Publish through the pubsub component to the topic parcel-events, and pass the parcel id as the SessionId metadata value. Subscribe to parcel-events on the pubsub component, with the handler taking the record types from src/Contracts. Register Dapr with the CloudEvents middleware and the subscribe handler in each service's Program.cs.

Keep the record types in src/Contracts unchanged. Remove the Azure.Messaging.ServiceBus package reference from these two services.

Make the gate test PubSubFlowsUseDapr in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: Tracking-service publishes ParcelScanned messages through the `pubsub` component, and notification-service receives them through a Dapr subscription. The traces show the publish and the delivery through both sidecars. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PubSubFlowsUseDapr"` passes.

## Step 3: Store Routing State with ETags

Overview: This step creates routing-service and its first endpoint, which assigns a parcel to a van exactly once. The assignment record lives in the `statestore` component, and its ETag makes a second writer fail with a conflict. The ETag check covers each assignment record on its own, so the van capacity check arrives in step 6.

Research:

```text
Read the Dapr state management overview and its section on ETags and concurrency on docs.dapr.io. Compare first-write-wins and last-write-wins for a record that must change only once, such as a parcel assigned to a van. Recommend one mode, and explain what the caller does when a write conflicts.
```

Finding: A good answer chooses first-write-wins with the ETag from the read, a bounded reread loop, and a conflict response after the last attempt. Push back if it chooses last-write-wins for an assignment that must happen once, or if it proposes a transaction for one key, because a transaction does not replace the ETag check across separate requests.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

Create the project src/routing-service as an ASP.NET Core service with Dapr enabled. Add it to ContosoLogistics.sln and to the AppHost with a Dapr sidecar and a reference to the statestore component.

Add the endpoint POST /reserve-capacity. It takes a parcel id and a van id, and it stores the assignment under the parcel id as the key in the statestore component. Read the record with its ETag and write it with first-write-wins concurrency. If the parcel already has an assignment, return HTTP 409. If a write conflicts, reread and retry up to three times, then return HTTP 409.

Make the gate test RoutingStateUsesETags in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: Routing-service starts with its sidecar. A first POST for a parcel stores the assignment and returns success, and a second POST for the same parcel to another van returns a conflict. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=RoutingStateUsesETags"` passes.

## Step 4: Call routing-service through Dapr Service Invocation

Overview: Shipping-api stops building an address for routing-service and calls it by app ID through its own sidecar. Container Apps resolves the same app ID through its Dapr settings, so the call stays the same when the service moves there.

Finding: The call goes through the sidecar, and the trace of the shipping-api span shows the invoke path `/v1.0/invoke/routing-service/method/reserve-capacity`. Push back if shipping-api still builds a URL with a host name or port for routing-service.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

In src/shipping-api, replace the direct HTTP call that reserves capacity with a Dapr service invocation to app ID routing-service and method reserve-capacity. Use the Dapr client typed invocation method and a request record that carries the parcel id and the van id from the booking flow. Add the van id to the booking request if it is missing. Remove any host name or port for routing-service from the shipping-api configuration files.

Make the gate test ShippingCallsRoutingThroughSidecar in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: A booking in the shipping-api flow reserves its parcel on routing-service through the sidecar, and the trace shows the invoke path with the app ID. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=ShippingCallsRoutingThroughSidecar"` passes.

## Step 5: Add a Resiliency Policy to the Routing Calls

Overview: The timeout, the retries and the circuit breaker for the shipping-api to routing-service call go into one resiliency spec. The caller code stays free of retry loops, and a change to the limits touches one file.

Research:

```text
Read the Dapr resiliency overview on docs.dapr.io. Explain how a timeout, a retry policy and a circuit breaker combine on calls to one app, which one ends the whole operation first, and what a trip condition counts. Propose a timeout, retry and trip setting for calls from shipping-api to routing-service, and explain how the values interact.
```

Finding: A good answer sets the timeout so it covers the full retry sequence, uses a constant retry policy first, and writes the trip condition in the form `consecutiveFailures >= N`. Push back if the timeout is shorter than the retry sequence, because the timeout then ends the operation before the retries run.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

Add a Dapr resiliency spec named shipping-resiliency in src/AppHost/components/resiliency.yaml. Define a timeout policy named general, a constant retry policy named retryPolicy with a two second delay and three retries, and a circuit breaker named cbPolicy that trips after five consecutive failures. Target the app routing-service with all three policies. Attach the spec to the Dapr setup in src/AppHost/Program.cs so the shipping-api sidecar loads it.

Set the general timeout so it covers the full retry sequence. Keep retry loops out of src/shipping-api.

Make the gate test RoutingCallRetryAndBreak in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: A call that fails once and then succeeds returns success after one retry. After five consecutive failures from routing-service, the next call fails fast while the breaker is open. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=RoutingCallRetryAndBreak"` passes.

## Step 6: Model Each Van as a VanActor

Overview: Each van gets one VanActor, so two assignments for the same van run one after the other and cannot both see the same free capacity. The actor runs on the local AppHost only, because the Dapr actor packages do not run on Container Apps managed Dapr.

Research:

```text
Compare two places to keep the capacity of one van: an actor that owns the van, and a state record with ETag checks as used in step 3. Name what each needs from the platform, what each costs when many assignments arrive at once, and which one you would choose for one van that receives many requests.
```

Finding: A good answer chooses the actor for single-owner assignment, and it names the state store that holds actor state, which is the statestore component with `actorStateStore` set to true for local runs. Push back if the answer deploys VanActor to Container Apps, because the Container Apps documentation states that the Dapr actor packages are not compatible with it.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

In src/routing-service, add the interface IVanActor and the class VanActor. VanActor holds the parcel count of one van, with a capacity limit read from configuration. It exposes AssignParcelAsync(parcelId), which returns false when the van is full, and ReportPositionAsync(latitude, longitude), which stores the last position. Register VanActor in the routing-service host and map the actor endpoints. In src/AppHost/components/statestore-local.yaml, mark the statestore component as the actor state store for local runs.

Change the POST /reserve-capacity endpoint so it calls VanActor.AssignParcelAsync for the van before it writes the parcel record from step 3. Do not change the ETag logic of that record. Do not add any actor package to Container Apps deployment files.

Make the gate test VanActorSerialisesAssignments in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: The gate fires concurrent assignments at one van and counts the accepted ones against the capacity limit. The count never passes the limit. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=VanActorSerialisesAssignments"` passes.

## Step 7: Prove Portability with the Lab 4 and Lab 6 Gates

Overview: The swap only counts if the earlier gates still pass, so this step runs the lab 4 scan gates and the lab 6 shipment gates against the Dapr path. A failing gate shows what the swap broke, and you fix the Dapr path, never the gate.

Finding: Both gates pass once the subscriber reads the CloudEvents `data` field. Push back if a gate fails because Dapr does not expose a feature that the gate depends on, such as a session receiver. Name the feature and the gate in your report and stop there, because editing the gate hides the break.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

Run the gates PortableScanFlow and PortableShipmentFlow in tests/ContosoLogistics.Tests with dotnet test and the Gate filter for each. PortableScanFlow covers the lab 4 scan gates: duplicate message, poison message, and scan order per parcel. PortableShipmentFlow covers the lab 6 gates: the shipment concurrency check, the outbox, the change feed projection, and the Redis cache invalidation.

Where a gate fails, fix the Dapr path in src/tracking-service, src/notification-service or src/AppHost. Keep every gate test unchanged. Report each file you changed and the gate that change fixed.
```

Expected Outcome: Both gates pass in one run. The report lists each file changed in this step and the gate that each change fixed. `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PortableScanFlow"` and `dotnet test tests/ContosoLogistics.Tests --filter "Gate=PortableShipmentFlow"` both pass.

## Step 8: Set the Components on the Container Apps Environment

Overview: The pub/sub and state components move to the Container Apps environment `cae-azn-labs-07` in `rg-azn-labs`, region `swedencentral`, so the same component names resolve in Azure. Run `labs/07-dapr/provision.azcli` once before this step. Managed Dapr does not host VanActor or the local Redis components, so this step sets only pub/sub and state.

Finding: Container Apps uses the short component schema with `componentType`, `version`, `metadata`, `secrets` and `scopes`, not the `apiVersion` and `kind` wrapper of the local files. Push back if an Azure file uses a Redis type, because Redis state and pub/sub are Tier 2 on Container Apps. Push back also if the `routingstate` container lacks the partition key `/partitionKey`, which the Dapr Cosmos DB state component requires.

Recipe:

```text
Work in the contoso-logistics folder. Read AGENTS.md first and follow the class stack.

Create src/AppHost/components/azure/pubsub.example.yaml with componentType pubsub.azure.servicebus.topics for the Service Bus namespace sb-azn-labs-07-contoso, and scope it to the app IDs tracking-service and notification-service. Create src/AppHost/components/azure/statestore.example.yaml with componentType state.azure.cosmosdb for the Cosmos DB account cosmos-azn-labs-07-contoso, database logistics and container routingstate, and scope it to the app ID routing-service. In each file, list every secret in the secrets list with an empty value, and reference it from metadata with secretRef. Follow the Dapr components in Azure Container Apps page on Microsoft Learn for the exact field names.

Add src/AppHost/components/azure/pubsub.yaml and src/AppHost/components/azure/statestore.yaml to .gitignore, so the filled-in copies never reach git.

Make the gate test ContainerAppsComponentsTier1 in tests/ContosoLogistics.Tests pass without editing the test.
```

Expected Outcome: Copy each example to its real file, fill in the Service Bus connection string and the Cosmos DB endpoint and key from the Azure portal, then run the three commands below from the contoso-logistics folder. The list command shows `pubsub` and `statestore`, and the gate passes.

```azurecli
az containerapp env dapr-component set --name cae-azn-labs-07 --resource-group rg-azn-labs --dapr-component-name pubsub --yaml src/AppHost/components/azure/pubsub.yaml
az containerapp env dapr-component set --name cae-azn-labs-07 --resource-group rg-azn-labs --dapr-component-name statestore --yaml src/AppHost/components/azure/statestore.yaml
az containerapp env dapr-component list --name cae-azn-labs-07 --resource-group rg-azn-labs
```

`dotnet test tests/ContosoLogistics.Tests --filter "Gate=ContainerAppsComponentsTier1"` passes.

## Solution

The finished reference is `labs/07-dapr/contoso-logistics-solution/`, produced by running this guide.

## Cleanup

Keep the Azure environment: the next lab deploys into the same resource group `rg-azn-labs`. Stop the AppHost. Run `deprovision.azcli` only if you end the class before lab 9; it deletes `rg-azn-labs` with everything the labs created.
