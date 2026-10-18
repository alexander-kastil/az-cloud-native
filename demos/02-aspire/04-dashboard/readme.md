---
minutes: 10
---

# Aspire Dashboard: Traces Across Services

A parcel scan that takes four seconds to reach the tracking page could lose that time in any of several services, and the console output of each terminal rarely shows where.
The Aspire dashboard puts the resources, logs, traces and metrics of the running system in one browser tab, and this topic shows how to read them and how an AI agent can query the same data.

## Resources and Console Output

The Resources page lists every resource from the AppHost with its state and health. From that page, a developer can stop, start or restart a single resource without stopping the rest of the Contoso system.

Each resource also has console output, which shows standard output and standard error from its process. The configuration view shows the settings the resource received, so a missing connection shows up before the first request. When a service fails to start, its console output shows the exception, and its state shows whether the process crashed or a health check failed.

| View | Shows |
| --- | --- |
| Resources | State, health, and the start, stop and restart actions |
| Console | Standard output and standard error for each resource |
| Structured logs | Log records with their fields, from every service |
| Traces | Spans that connect requests across services |
| Metrics | Counters, histograms and gauges from OpenTelemetry |

## Structured Logs That Carry Parcel Identifiers

Structured logs keep each field as data, so the dashboard can filter by a tracking number without parsing text. The message template defines the field names, so every service that logs a `TrackingNumber` field gives the dashboard the same key.

```csharp
logger.LogInformation("Parcel {TrackingNumber} scanned at {ScanType}", trackingNumber, scanType);
```

## Traces Across HTTP and Service Bus

HTTP calls carry the W3C trace context in the `traceparent` header, and the defaults instrument both ends. A call from one service to another creates a span in the caller and a span in the callee, and both spans share one trace identifier.

Service Bus carries the trace context too. The Azure SDK writes it to the `Diagnostic-Id` application property of each message, and it creates spans such as `ServiceBusSender.Send` and `ServiceBusReceiver.Receive`. The dashboard links the publish to the processing, so one trace covers the scan from the scanner API to the tracking projection.

Each span shows its start time, its duration and its parent. The longest bar in the waterfall names the step that costs the most time, so the developer reads the cause before opening any log.

The Service Bus client integration registers the instrumentation for `ServiceBusClient`, so these spans appear without manual activity code.

## Metrics for the Running System

The defaults register ASP.NET Core, `HttpClient` and runtime metrics for every service. The dashboard charts them per resource, so a rising request duration on the routing service is visible next to its traces.

A metric shows the trend, and a trace shows one request. The developer uses the chart to find the time window of a slowdown and the trace to find the request and the step that caused it.

## Agent Support Through MCP

The dashboard can share its telemetry with AI coding agents through the Aspire CLI or an optional MCP server. An agent can then read the same resources, logs and traces that the developer sees.

`aspire agent init` sets up agent configuration for the agent environments that the CLI detects. `aspire agent mcp` starts the Model Context Protocol server, which exposes the telemetry as tools.

| Command | Purpose |
| --- | --- |
| `aspire agent init` | Sets up agent configuration for the detected agent environments |
| `aspire agent mcp` | Starts the MCP server |
| `aspire agent mcp --dashboard-url "http://localhost:18888" --api-key "<api-key>"` | Connects the MCP server to a standalone dashboard |

The telemetry tools include `list_resources`, `list_structured_logs`, `list_traces` and `list_console_logs`. The standalone connection exposes telemetry tools only, so an agent can read a failing trace but cannot manage AppHost resources through it.

## Standalone Dashboard for Apps Outside an AppHost

The standalone dashboard is an OTLP viewer for any application that exports OpenTelemetry, including Python and Node.js apps. It has no AppHost, so it has no resource start and stop controls. It suits a tool or script that runs outside the AppHost and still needs the same traces and logs.

```bash
aspire dashboard run
```

The container image runs the same dashboard with explicit port mappings.

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -p 4318:18890 -d --name aspire-dashboard mcr.microsoft.com/aspire/dashboard:latest
```

Point the application at the dashboard with the OTLP environment variables. The gRPC endpoint listens on port 4317 and the HTTP endpoint on port 4318.

```bash
export OTEL_EXPORTER_OTLP_ENDPOINT=http://localhost:4317
export OTEL_EXPORTER_OTLP_PROTOCOL=grpc
```

| Host port | Purpose |
| --- | --- |
| 18888 | Dashboard UI at `http://localhost:18888` |
| 4317 | OTLP over gRPC |
| 4318 | OTLP over HTTP |

## Key Topics

- [Aspire dashboard overview](https://aspire.dev/dashboard/overview/)
- [Aspire dashboard with AI coding agents](https://aspire.dev/dashboard/ai-coding-agents/)
- [Standalone Aspire dashboard](https://aspire.dev/dashboard/standalone/)
- [aspire agent command reference](https://aspire.dev/reference/cli/commands/aspire-agent/)
- [Distributed tracing and correlation through Service Bus messaging](https://learn.microsoft.com/azure/service-bus-messaging/service-bus-end-to-end-tracing)
