# Dapr Bindings

Dapr bindings are a way to declaratively connect your application to another service. Dapr bindings are event-driven and can be triggered by an event or run on a schedule. Dapr bindings are implemented as an output binding, an input binding, or a bidirectional binding.

![dapr-bindings](_images/dapr-bindings.png)

## Links & Resources

[Bindings Overview](https://docs.dapr.io/developing-applications/building-blocks/bindings/)

[Dapr State Bindings Components](https://docs.dapr.io/reference/components-reference/supported-bindings/)

[Azure Samples - Event-driven work using bindings and Postgres SQL](https://github.com/Azure-Samples/bindings-dapr-csharp-cron-postgres/tree/main)

## Demos

The notification service is a simple service that does two things:

- It has a corn job that runs every 5 seconds and writes a message to the console.
- It listens to events from the service bus and sends an order payment notification as sms using twilio.

    >Note: In order to use the twilio binding you need to have a twilio trial account.

- Examine the bindings configuration in the `components` folder. 

- Run the notification service with the following command:

    ```bash
    dapr run --app-id notification-service --app-port 5007 --dapr-http-port 5017 --resources-path './components' dotnet run
    ```