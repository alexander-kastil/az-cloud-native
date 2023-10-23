# Dapr Bindings

Dapr bindings are a way to declaratively connect your application to another service. Dapr bindings are event-driven and can be triggered by an event or run on a schedule. Dapr bindings are implemented as an output binding, an input binding, or a bidirectional binding.

![dapr-bindings](_images/dapr-bindings.png)

## Notification Service

The notification service is a simple service that listens to events from the service bus and writes them to the console.

Run the notification service with the following command:

```bash
dapr run --app-id notification-service --app-port 5007 --dapr-http-port 5017 --resources-path './components' dotnet run
```

## Service Bus

- To publish some sample events to service bus use [sb-data-generator](./sb-data-generator//)

- Run the payment-service

```bash
dapr run --app-id payment-service --app-port 5008 --dapr-http-port 5018 --resources-path './components' dotnet run
```