# Lab 03 - Developing & Publishing Microservices using Azure Container Apps (ACA)

In this lab we will explore the basics of Azure Container Apps. We will start by deploying the following apps:

- Catalog Service
- Orders Service
- Shop UI

>Note: You can use Azure CLI or Bicep to complete the provisioning tasks of this lab. The solution provides an Azure CLI implementation.

## Task: Configuration Management

- Add a Key Vault to the Resource Group
- Add an Azure App Configuration Service to the Resource Group. 
- Assign permissions on the Key Vault to the App Configuration Service.
- Use Service Connector to connect the App Configuration Service to Azure Container Apps.

## Task: Deploying the Catalog Service

- Create a new Azure Container App and deploy the Catalog Service container to it.
- Use Azure App Configuration Service to configure the Catalog Service. 
- Save the URL of the Catalog Service to the App Configuration Service.

## Task: Deploying the Orders Service

- Create a new Azure Container App and deploy the Orders Service container to it.
- Use Azure App Configuration Service and a Key Vault Reference to configure `CosmosDB:ConnectionString` with a mock value.
- Save the URL of the Orders Service to the App Configuration Service.

## Task: Deploying the Food Shop UI

- Create a new Azure Container App and deploy the Orders Service container to it.
- Use Azure App Configuration Service to override the `ORDERS_API_URL` and `CATALOG_API_URL` values defined in `environment.ts`.