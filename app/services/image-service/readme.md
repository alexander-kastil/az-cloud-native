# Image Service

| .NET Api Services         | Http Port | Https Port | Dapr Port | Dapr App ID          | Docker Port| Stack   |
| -------                   | --------- | ---------- | --------- | -------------        | -----      |---------|
| Image Service             | 5009      | 5029       |           | image-service        | 5059       | .NET 8  |

>Note: Http ports can be configured in launchSettings.json of the .net projects.

- Docker Build & Run: 

    ```bash
    docker build --rm -f dockerfile -t cooking-service .
    docker run -it --rm -p 5059:8080 cooking-service
    ```

- Environment Variables:
    - ApplicationInsights__ConnectionString
    - IMG_FOLDER_NAME

- Rest Tester:

    ```bash
    POST http://localhost:5009/send HTTP/1.1
    Content-Type: application/json

    {
        "subject": "A test mail",
        "text": "Explore - Let life surprise you!",
        "recipient": "alexander.kastil@integrations.at"
    }
    ```