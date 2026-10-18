---
minutes: 10
---

# Application Insights: Traces & KQL

When a parcel scan fails, the cause can sit in several places: the HTTP request to tracking-service, the Service Bus message that carried the scan, the database write that followed, or an exception in a consumer that nobody reads. Without one trace across those hops, each team checks its own logs and guesses at the link between them.

This topic configures the Azure Monitor OpenTelemetry distro, explains the managed OpenTelemetry agent for Container Apps, shows how Service Bus carries the trace context between producer and consumer, and then queries the telemetry tables with KQL.

## Send telemetry from the app and the environment

The Azure Monitor OpenTelemetry distro adds traces, metrics, and logs to an ASP.NET Core service. Install the `Azure.Monitor.OpenTelemetry.AspNetCore` package and call `UseAzureMonitor` on the OpenTelemetry builder. The distro reads the connection string from the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable.

```csharp
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry().UseAzureMonitor();

var app = builder.Build();
app.Run();
```

Learn now recommends the Microsoft OpenTelemetry Distro for new .NET apps, and existing Azure Monitor distro users need no action. This topic teaches the Azure Monitor distro and its `UseAzureMonitor` call.

The managed OpenTelemetry agent is an alternative for apps that send OTLP over gRPC. It runs inside the Container Apps environment as a single replica, in preview, with a fixed 0.5 vCPU and 1.5 GB of memory. The agent sends each signal type to a destination, and the destinations differ in what they accept.

| Destination | Logs | Metrics | Traces |
| --- | --- | --- | --- |
| Azure Monitor Application Insights | Yes | No | Yes |
| Datadog | Yes | Yes | Yes |
| OTLP-compatible endpoint | Yes | Yes | Yes |

Configuration lives at the environment level, so you can't send one app's metrics to one destination and another app's traces to a different one. The agent injects `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_EXPORTER_OTLP_PROTOCOL`, and `OTEL_RESOURCE_ATTRIBUTES` into each app, and a value that the app sets itself overrides the injected one.

## Trace requests across HTTP and Service Bus

Correlation depends on one identifier that travels with each hop. The HTTP call and the Service Bus message both carry the operation context. The Azure Service Bus SDK writes the W3C trace context into the message as the `Diagnostic-Id` application property, in the `traceparent` format.

The SDK creates spans for `ServiceBusSender.Send`, `ServiceBusReceiver.Receive`, `ServiceBusProcessor.ProcessMessage`, and `ServiceBusReceiver.Complete`, and it links them through a `Message` span. Application Insights draws that chain in the end-to-end transaction view, so a scan that starts at tracking-service and ends in a consumer appears as one transaction. You don't write the propagation code yourself, but both services need a tracing setup that exports the spans.

## Query the tables with KQL

Each telemetry type has its own table in Log Analytics. `AppRequests` holds inbound requests, `AppDependencies` holds outbound calls, `AppTraces` holds log messages, and `AppExceptions` holds exceptions. Each row carries `OperationId` and `ParentId`, which join the rows of one transaction.

This query lists the failed requests in the last hour, with their duration and operation ID.

```kusto
AppRequests
| where TimeGenerated > ago(1h) and Success == false
| project TimeGenerated, AppRoleName, Name, Url, ResultCode, DurationMs, OperationId
| order by TimeGenerated desc
```

This query ranks outbound dependencies by their 95th percentile duration and counts their failures.

```kusto
AppDependencies
| where TimeGenerated > ago(1h)
| summarize Calls = count(), P95Ms = percentile(DurationMs, 95), Failures = countif(Success == false) by DependencyType, Target
| order by P95Ms desc
```

This query groups the day's exceptions by problem ID and outer type.

```kusto
AppExceptions
| where TimeGenerated > ago(24h)
| summarize Count = count() by ProblemId, OuterType
| order by Count desc
```

Given one operation ID, this query shows every row of that transaction in time order across the four tables.

```kusto
union AppRequests, AppDependencies, AppTraces, AppExceptions
| where OperationId == "<OPERATION_ID>"
| project TimeGenerated, Type, OperationId, ParentId
| order by TimeGenerated asc
```

Transaction search lists the telemetry that matches a filter, and selecting an item opens the end-to-end transaction details view. That view draws a Gantt chart of each event with its duration and response code. Learn calls this list the Search view.

When sampling is enabled, the portal shows only the telemetry the sampler kept, so a missing row can mean the sampler dropped it.

## Key Topics

- [Enable OpenTelemetry for .NET applications](https://learn.microsoft.com/azure/azure-monitor/app/opentelemetry-enable)
- [Azure Monitor OpenTelemetry Distro client library for .NET](https://learn.microsoft.com/dotnet/api/overview/azure/monitor.opentelemetry.aspnetcore-readme?view=azure-dotnet)
- [Distributed tracing and correlation through Service Bus messaging](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-end-to-end-tracing)
- [Collect and read OpenTelemetry data in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/opentelemetry-agents)
- [AppRequests table reference](https://learn.microsoft.com/azure/azure-monitor/reference/tables/apprequests)
- [Investigate failures, performance, and transactions with Application Insights](https://learn.microsoft.com/azure/azure-monitor/app/failures-performance-transactions)
