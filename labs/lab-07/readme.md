# Lab 07 - Using Distributed Application Runtime - Dapr

- Setup Developer Environment to support Dapr
- Connect micro-services using Dapr Pub/Sub

## Task: Setup Developer Environment to support Dapr

- Install Dapr CLI

    ```powershell
    Set-ExecutionPolicy RemoteSigned -scope CurrentUser
    powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
    ```

    >Note: Restart the terminal after installing the Dapr CLI

- Install the [Dapr Visual Studio Code extension](https://docs.dapr.io/developing-applications/local-development/ides/vscode/vscode-dapr-extension/)    

- Initialize default Dapr containers and check running containers:

    ```bash
    dapr init
    ```

    >Note: In case you need to remove the default Dapr containers run `dapr uninstall` 

- Check the running containers using the [Docker Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)

    ![dapr-init](_images/dapr-init.png)

- Run project [orders-service-cqrs](./starter/orders-service-cqrs/)

    ```bash
    dapr run --app-id order-service --app-port 5002 --dapr-http-port 5012 --resources-path './components' dotnet run
    ```
- Show Dapr Dashboard

    ```
    dapr dashboard
    ``` 

- Examine Dapr Dashboard on http://localhost:8080:

    ![dapr-dashboard](_images/dapr-dashboard.png)

- Setup for Dapr Debugging can be done using the Dapr Extension for Visual Studio Code. 

    ![dapr-debug](_images/dapr-debug.png)

    >Note: Make sure you have setup .NET debugging in advance

- Test your debug configuration

    ![launch-debug](_images/launch-debug.png)    

## Connect micro-services using Dapr Pub/Sub

To make local development and debug easier use the following ports reference for the services:

| .NET Api Services         | Https Port | Http Port | Dapr Port | Dapr App ID          | Docker Port|
| -------                   | --------- | ---------- | --------- | -------------        | -----|
| Order Service             | 5002      | 5022       | 5012      | order-service        | 5052 |
| Payment Service           | 5004      | 5024       | 5014      | payment-service      | 5054 |
| Bank Actor Service        | 5005      | 5025       | 5015      | bank-actor           | 5055 |
| Cooking Service           | 5006      | 5026       | 5016      | cooking-service      | 5056 |
| Delivery Service          | 5007      | 5027       | 5017      | deliver-service      | 5057 |
| Graph NotificationService | 5008      | 5028       | 5018      | notification-service | 5058 |

 
| Azure Functions                 | Http Port | Docker Port|
| -------                         | --------- | ---------- | 
| Order Event Processor Function  | 7073      |            |	
| Payment Service Function        | 7074      |            | 
| Cooking Dashboard Function      | 7076      |            |
| Optimizer Function              | 7077      |            |
| Invoices Job Function           | 7078      |            |