# Lab 07 - Using Distributed Application Runtime - Dapr

- Setup Developer Environment to support Dapr
- Provision the required infrastructure for Dapr Pub/Sub
- Get familiar with the starter projects
- Implement the payment process
- Publish to Azure Container Apps
- Cooking Service, Delivery Service - Optional


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

## Task: Provision the required infrastructure for Dapr Pub/Sub

- Create a new Azure Service Bus Namespace

    ```bash
    sbNS=aznativesb$env
    az servicebus namespace create --name <your-namespace-name> --resource-group <your-resource-group-name> --location <your-location> --sku Standard
    ```    

- Create the following Azure Service Bus topics using the following command:

    - payment-request
    - payment-response
    - cooking-request
    - cooking-response
    - delivery-request
    - delivery-response

    ```bash
    sbNS=aznativesb$env
    az servicebus topic create --name <your-topic-name> --namespace-name <your-namespace-name> --resource-group <your-resource-group-name>
    ```

## Task: Get familiar with the starter projects

- Examine the [starter projects](./starter/). Some of the projects we have used in previous labs, other are well prepared starters. All projects have the required Dapr components defined in the `<project>/components` folder. The required NuGet packages are already installed in all projects that require Dapr.

- To make local development and debug easier use the following ports reference for the services:

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

- Examine the `Food App Domain Message Flow`. 

    ![message-flow-model](_images/message-flow.png)    	

- Examine the `Food App Domain Message Flow`. 

    ![message-flow-model](_images/message-flow-data-model.png)

## Task: Implement the payment process

- With this task we will implement the full Payment Process using Dapr Pub/Sub including the Bank Actor Service.

    ![payment-process](_images/payment-process.png)
   
## Task: Publish to Azure Container Apps


## Task: Cooking Service, Delivery Service - Optional

- If your time permits you can repeat the pattern uses with the Payment Service for the Cooking Service, Delivery Service and Notification Service. We will extend the Cooking Service with the Cooking Dashboard Function later on

- Send the requests to the according service, wait for a few seconds and send a positive that will be consumed by the Order Service

- If you want you can also publish the Cooking Service and Delivery Service to Azure Container Apps