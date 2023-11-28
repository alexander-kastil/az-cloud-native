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

- Dapr Run & Test:

    ```bash
    dapr run --app-id BankActor --app-port 5005 --dapr-http-port 3500 --resources-path './components' dotnet run 
    ```