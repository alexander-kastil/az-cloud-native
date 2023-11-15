# Solution - Using Distributed Application Runtime - Dapr

## Cleanup unused resources

- Execute: 

    ```bash
    env=dev
    loc=westeurope
    grp=az-native-$env
    funcApp=payment-service-$env

    az functionapp delete -n $funcApp -g $grp
    ```

## Test the services locally

-   Run Order Service:

    ```bash
    dapr run --app-id order-service --app-port 5002 --dapr-http-port 5012 --resources-path './components' dotnet run
    ```

-   Run Order Events Processor Service:

    ```bash
    func start
    ```

-   Run Payment Service:

    ```bash
    dapr run --app-id payment-service --app-port 5004 --dapr-http-port 5014 --resources-path './components' dotnet run
    ```

-  Run Bank Actor Service:

    ```bash
    dapr run --app-id bank-actor-service --app-port 5005 --dapr-http-port 3500 --resources-path './components' dotnet run
    ```

## Deploy App    

- Execute `create-images.azcli`

- Execute `add-dapr-components.azcli` to add the dapr components to the container app environment

- Execute `deploy-app.azcli` to deploy the app 