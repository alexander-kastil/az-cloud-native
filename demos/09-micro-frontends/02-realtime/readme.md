# Real-time connected Angular Micro Frontend using Azure Event Grid and SignalR

[Kitchen Dashboard](/app/web/cooking-dashboard/) implemented as Angular Micro-Frontend using `@ngrx/component-store` displaying orders.  [Kitchen dashboard function](/app/functions/cooking-dashboard-func/) that acts as an endpoint for the event grid topic webhook subscription and communicates with the SignalR service that provides a real time connection to the orders dashboard.

![architecture](_images/architecture.png)

## Readings

[CloudEvent schema](https://docs.microsoft.com/en-us/azure/event-grid/cloudevents-schema)

[Understand event filtering for Event Grid subscriptions](https://learn.microsoft.com/en-us/azure/event-grid/event-filtering)

[Understand event domains for managing Event Grid topics](https://learn.microsoft.com/en-us/azure/event-grid/event-domains?tabs=event-grid-event-schema)

[SignalR](https://docs.microsoft.com/en-us/azure/azure-signalr)

## Demo

-   Execute `create-kitchen-app.azcli` in [wsl bash](https://learn.microsoft.com/en-us/windows/wsl/install) to provision the environment and deploy the function app. Navigate to the Azure portal and check that the resources have been created.

    ![azure](_images/azure.png)

-   Update SignalR config key `fxEndpoint` in `environment.ts` of `kitchen-dashboard` using the values from the terminal of the previous step.

    ![azure](_images/cfg.png)

    ```typescript
    export const environment = {
        production: false,
        fxEndpoint: 'https://foodorders-7325.azurewebsites.net/api',
    };
    ```

-   Start the Micro-Frontend using `ng serve` in `kitchen-dashboard` and open [http://localhost:4200](http://localhost:4200). Open the F12 Dev tools and check that the SignalR connection is established.

    ![websocket](_images/websocket.png)

-   Send a mock CloudEvent using `post-order.http` by updating `@topicurl` and `@topickey` with the values from the terminal:

    ```
    @topicUrl=foodtopic-prod.westeurope-1.eventgrid.azure.net
    @topicKey=C1q1BdqhPGsNsmy5wBzjtsgTTN1u2GbiffNoU8EJlcM=

    POST  https://{{topicUrl}}//api/events HTTP/1.1
    content-type: application/cloudevents+json; charset=utf-8
    aeg-sas-key: {{topicKey}}

    { ...
    ```

## Credits

The demo is an updated and modernized version of [https://github.com/DavidGSola/serverless-eventgrid-viewer](https://github.com/DavidGSola/serverless-eventgrid-viewer)
