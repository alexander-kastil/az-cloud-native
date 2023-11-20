# Lab 03 - Developing & Publishing Microservices using Azure Container Apps (ACA)

In this lab we will explore the basics of Azure Container Apps. We will start by deploying the following apps:

- Catalog Service
- Orders Service
- Shop UI

You can use Azure CLI or Bicep to complete the provisioning tasks of this lab. The solution provides an Azure CLI implementation.

>Note: To be able to update your fork of this repository, without having to deal with merge conflicts, use a `lab-03/solution-$env` folder. Copy the content of the `lab-03/solution` folder to l`ab-03/solution-$env `and work in the new folder. This way you can always pull the latest changes from the upstream repository and merge them into your fork and keep a clean starter- and solution folder.

## Task: Configuration Management

- Ensure that you have a Key Vault `az-native-kv-$env` in your Resource Group
- Assign permissions on the Key Vault to the App Configuration Service.
- Use Service Connector to connect the App Configuration Service to Azure Container Apps.

## Task: Deploy Catalog Service

- Create a new Azure Container App and deploy the [Catalog Service](/app/services/catalog-service/) container to it.
- Use Azure App Configuration Service to configure the Catalog Service. 
- Save the URL of the Catalog Service to the App Configuration Service.

## Task: Deploy Orders Service

- Create a new Azure Container App and deploy the [Orders Service](/app//services/orders-service/) container to it.
- Use Azure App Configuration Service and a Key Vault Reference to configure `CosmosDB:ConnectionString` with a mock value.
- Save the URL of the Orders Service to the App Configuration Service.

## Task: Deploy Food Shop UI

- Create a new Azure Container App and deploy the [Orders Service](/app/web/food-shop/) container to it.
- Use Azure App Configuration Service to override the `ORDERS_API_URL` and `CATALOG_API_URL` values defined in `environment.ts`.
