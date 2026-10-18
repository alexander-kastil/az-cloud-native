# Delivery Service

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port|
| -------                   | --------- | ---------- | --------- | -------------        | -----      |
| DeliveryService            | 5006      | 5026       | 5016      | delivery-service | 5056       |

>Note: Http ports can be configured in launchSettings.json of the .net projects.

- Docker Build & Run: 

    ```bash
    docker build --rm -f dockerfile -t delivery-service .
    docker run -it --rm -p 5056:8080 delivery-service
    ```

- Environment Variables:
    - ApplicationInsights__ConnectionString
    - GraphCfg__TenantId
    - GraphCfg__ClientId
    - GraphCfg__ClientSecret    

- Rest Tester:

    ```bash
    POST http://localhost:5006/send HTTP/1.1
    Content-Type: application/json

    {
        "subject": "A test mail",
        "text": "Explore - Let life surprise you!",
        "recipient": "alexander.kastil@integrations.at"
    }
    ```

- Dapr Run & Test:

    ```bash
    dapr run --app-id delivery-service --app-port 5006 --dapr-http-port 5016 --resources-path ./components -- dotnet run
    ```
    
    ```bash
    dapr invoke --app-id delivery-service --method pubsub-test --data '{\"id\": \"1\", \"subject\": \"Explore - Let life surprise you!\" }'
    ```   

    ```bash
     dapr publish --publish-app-id delivery-service --pubsub 'food-pubsub" --topic "delivery-requests" --data "{\"subject\": \"A test mail\", \"text\": \"Explore - Let life surprise you!\", \"recipient\": \"alexander.kastil@integrations.at"}'
    ```   