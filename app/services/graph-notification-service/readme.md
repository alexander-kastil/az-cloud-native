# Graph Notification Service

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port|
| -------                   | --------- | ---------- | --------- | -------------        | -----      |
| Graph NotificationService | 5008      | 5028       | 5018      | notification-service | 5058       |


- Docker Build & Run: 

    ```bash
    docker build --rm -f dockerfile -t graph-notification-service .
    docker run -it --rm -p 5058:8080 graph-notification-service
    ```

- Environment Variables:
    - ApplicationInsights__ConnectionString
    - GraphCfg__TenantId
    - GraphCfg__ClientId
    - GraphCfg__ClientSecret    

- Rest Tester:

    ```bash
    POST http://localhost:5008/send HTTP/1.1
    Content-Type: application/json

    {
        "subject": "A test mail",
        "text": "Explore - Let life surprise you!",
        "recipient": "alexander.pajer@integrations.at"
    }
    ```

- Dapr Tester:

    ```bash
    dapr run --app-id notification-service --app-port 5008 --dapr-http-port 5018 --resources-path ./components -- dotnet run
    ```
    
    ```bash
    dapr invoke --app-id notification-service --method test --data "{'id': '1', 'subject': 'Explore - Let life surprise you!' }"
    ```   

    ```bash
     dapr publish --publish-app-id notification-service --pubsub "food-pubsub" --topic "notification-requests" --data "{\"subject\": \"A test mail\", \"text\": \"Explore - Let life surprise you!\", \"recipient\": \"alexander.pajer@integrations.at"}"
    ```   