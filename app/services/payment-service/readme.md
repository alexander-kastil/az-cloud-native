# Payment Service

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port|
| -------                   | --------- | ---------- | --------- | -------------        | -----      |
| Payment Service           | 5004      | 5024       | 5014      | payment-service      | 5054       |
| Bank Actor Service        | 5005      | 5025       | 3500      | bank-actor           | 5055       |

- Docker Build & Run: 

    ```bash
    docker build --rm -f dockerfile -t payment-service .
    docker run -it --rm -p 5054:8080 payment-service
    ```

- Environment Variables:
    - ApplicationInsights__ConnectionString
    - GraphCfg__TenantId
    - GraphCfg__ClientId
    - GraphCfg__ClientSecret    

- Rest Tester:

    ```bash
    POST http://localhost:5004/send HTTP/1.1
    Content-Type: application/json

    {
        "subject": "A test mail",
        "text": "Explore - Let life surprise you!",
        "recipient": "alexander.kastil@integrations.at"
    }
    ```

- Dapr Run & Test:

    ```bash
    dapr run --app-id payment-service --app-port 5008 --dapr-http-port 5014 --resources-path ./components -- dotnet run
    ```
    
    ```bash
    dapr invoke --app-id payment-service --method pubsub-test --data '{\"id\": \"1\", \"subject\": \"Explore - Let life surprise you!\" }'
    ```   

    ```bash
     dapr publish --publish-app-id payment-service --pubsub 'food-pubsub" --topic "payment-requests" --data "{\"subject\": \"A test mail\", \"text\": \"Explore - Let life surprise you!\", \"recipient\": \"alexander.kastil@integrations.at"}'
    ```   