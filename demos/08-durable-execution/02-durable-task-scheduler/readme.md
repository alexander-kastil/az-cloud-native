# Durable Task Scheduler: Managed Backend

> **_NOTE:_** Duration: about 9 minutes.

Orchestration state has to survive restarts and grow with the number of workers. Contoso Logistics does not want to run and tune a state store for every booking service. The Durable Task Scheduler is the managed backend that keeps orchestration state for Durable Functions and for the Durable Task SDKs.

This topic covers the Azure resources you create, the two billing SKUs, and the local emulator. It also covers the connection string formats, the managed identity roles that grant access, and the dashboard that shows an orchestration's history.

## Schedulers and Task Hubs

A scheduler is the Azure resource. A task hub is a container inside the scheduler that holds the state of a set of orchestrations, and each app connects to one task hub. Add the `durabletask` extension to the Azure CLI, then create the scheduler and the task hub.

```bash
az extension add --name durabletask
az durabletask scheduler create --name contoso-dts --resource-group rg-contoso-dts --location westeurope --ip-allowlist "[0.0.0.0/0]" --sku-name "dedicated" --sku-capacity "1"
az durabletask taskhub create --resource-group rg-contoso-dts --scheduler-name contoso-dts --name bookings
```

The allow list `0.0.0.0/0` permits any IP address. Restrict it to your network ranges before you send production traffic. Learn sets the quotas by SKU, and the table lists the values for each one.

| Limit | Dedicated SKU | Consumption SKU |
| --- | --- | --- |
| Schedulers per region per subscription | 25 | 10 |
| Task hubs | 25 per region per subscription | 5 per scheduler |

Some limits apply to every scheduler, whatever the SKU. An orchestrator, activity, or entity input or output is limited to 1 MB. A single work item completion request is limited to 3.9 MB.

An orchestration instance ID is limited to 100 printable ASCII characters. IDs that start with `@` are reserved for entities.

## Dedicated and Consumption SKUs

The Dedicated SKU reserves capacity units (CUs), and you pay a fixed monthly cost for each unit. The Consumption SKU bills per action, where an action is a message the scheduler dispatches to trigger an orchestrator, activity, or entity. The table compares the two on the properties that drive the choice.

| Property | Dedicated | Consumption |
| --- | --- | --- |
| Billing | Fixed monthly cost per capacity unit | Pay per action dispatched |
| Throughput | 2,000 actions per second per unit | 500 actions per second |
| Data retention | Up to 90 days | Up to 30 days |
| High availability | Three capacity units | Not available |
| Best fit | Production with predictable volume | Development and variable workloads |

A single capacity unit gives no redundancy, so configure three units for a high-availability deployment. Both SKUs are generally available; the Consumption SKU reached general availability in April 2026.

## Emulator and Connection Strings

The emulator runs the same protocol in a Docker container, so you can develop without an Azure subscription. Port 8080 serves the endpoint, and port 8082 serves the dashboard. The command binds both ports to loopback, because the emulator accepts unauthenticated requests.

```bash
docker run --name dtsemulator -d -p 127.0.0.1:8080:8080 -p 127.0.0.1:8082:8082 mcr.microsoft.com/dts/dts-emulator:latest
```

Local apps use a connection string that names the endpoint, the task hub, and the authentication mode.

```text
Endpoint=http://localhost:8080;TaskHub=default;Authentication=None
```

Azure apps use managed identity. The connection string carries the scheduler endpoint and the client ID of a user-assigned identity. A system-assigned identity omits the `ClientID` segment.

```text
Endpoint={scheduler endpoint};Authentication=ManagedIdentity;ClientID={client id}
```

The emulator does not support private endpoints, because private endpoints apply only to schedulers deployed in Azure. Learn recommends a user-assigned identity, because it outlives the app and you can reuse it after the app is removed.

## Managed Identity and Roles

The Durable Task Scheduler accepts managed identity only, and it does not accept connection strings that carry storage keys. A private endpoint secures the network path to the scheduler, but the app still needs a role to authenticate. Assign the role to the identity at the task hub scope, which follows least privilege.

Most apps need the Durable Task Data Contributor role, which includes every other data role.

```bash
az role assignment create --assignee CLIENT_ID --role "Durable Task Data Contributor" --scope /subscriptions/SUBSCRIPTION_ID/resourceGroups/rg-contoso-dts/providers/Microsoft.DurableTask/schedulers/contoso-dts/taskHubs/bookings
```

| Role | Grants |
| --- | --- |
| Durable Task Data Contributor | Full data access, a superset of the other data roles |
| Durable Task Worker | Process orchestrations, activities, and entities |
| Durable Task Data Reader | Read orchestration and entity data |

The dashboard also needs a role on the task hub. Learn names Durable Task Dashboard Viewer on one page and Durable Task Data Contributor on another, so check the current dashboard page before you grant access to a team.

## The Dashboard

The emulator dashboard opens at `http://localhost:8082`. For an Azure scheduler, open the task hub overview and select the dashboard URL shown in the essentials section. The dashboard shows each orchestration's status and its execution history.

Use the history to debug a workflow. When an instance stays in the Running state, find its last completed event, because the next expected event is the one that blocks progress. A `TimerCreated` event without a matching fire means the timer has not expired yet.

## Key Topics

- [Durable Task Scheduler](https://learn.microsoft.com/azure/durable-task/scheduler/durable-task-scheduler)
- [Durable Task Scheduler billing](https://learn.microsoft.com/azure/durable-task/scheduler/durable-task-scheduler-billing)
- [Configure managed identity for Durable Task Scheduler](https://learn.microsoft.com/azure/durable-task/scheduler/durable-task-scheduler-identity)
- [Durable Task Scheduler dashboard](https://learn.microsoft.com/azure/durable-task/scheduler/durable-task-scheduler-dashboard)
- [Develop with Durable Task Scheduler](https://learn.microsoft.com/azure/durable-task/scheduler/develop-with-durable-task-scheduler)
