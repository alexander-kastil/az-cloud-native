# Lab 03 - Developing & Publishing Microservices using Azure Container Apps (ACA)

In this lab we will explore the basics of Azure Container Apps. We will start by deploying the following apps:

- Catalog Service
- Orders Service
- Food Shop

You can use Azure CLI or Bicep to complete the provisioning tasks of this lab. The solution provides an Azure CLI implementation.

>Note: To be able to update your fork of this repository, without having to deal with merge conflicts, use a `lab-03/solution-$env` folder. Copy the content of the `lab-03/solution` folder to l`ab-03/solution-$env `and work in the new folder. This way you can always pull the latest changes from the upstream repository and merge them into your fork and keep a clean starter- and solution folder.

## Task: Configuration Management

- Ensure that you have a Key Vault `az-native-kv-$env` in your Resource Group
- When executing the individual tasks, ensure that the required values are stored in the key vault
- We will not directly couple Azure Key Vault or Azure App Configuration service to our services an thous will avoid tight coupling. Instead, in this early stage we will interact with this services using the Azure CLI and pull the values of our variables from there.

> Note: A samples of tighter coupling is provided for [Azure App Configuration](/demos/00-app/config-service/Program.cs)

## Task: Deploy Catalog Service using the User Interface

- Create a new Azure Container App using the User interface and deploy the [Catalog Service](/app/services/catalog-service/) container to it.

- Use the following settings:

    - Name: catalog-service
    - Container Apps: Environment: acaenv-az-native-$env
    - Image: `$acr/catalog-service:lab-02`
    - Ingress: Allow any kind of traffic
    - Port: `80`

- Create a secret that is linked to Azure Key Vault:

    - Name: aiconstr   

- Create a revision with injected environment variables:

    - Title: Catalog Service ACA
    - App__UseSQLite: true
    - ApplicationInsights__ConnectionString: linked to secret aiconstr

## Task: Deploy Orders Service

- Create a new Azure Container App and deploy the [Orders Service](/app//services/orders-service/) container to it.


## Task: Deploy Food Shop

- Create a new Azure Container App and deploy the [Food Shop](/app/web/food-shop/) container to it.
