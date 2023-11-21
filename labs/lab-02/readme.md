# Lab 02 - Container Essentials

In this Lab we will explore the basics of containers. We will start by containerizing the following apps:

- Catalog Service
- Order Service (Simplified version that will be replaced later on)
- Food Shop

>Note: To be able to update your fork of this repository, without having to deal with merge conflicts, use a `lab-02/solution-$env` folder. Copy the content of the `lab-02/solution` folder to `lab-02/solution-$env` and work in the new folder. This way you can always pull the latest changes from the upstream repository and merge them into your fork and keep a clean starter- and solution folder.

## Task: Containerize Catalog Service

- Add a docker file to Catalog Service build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set the value of UseSQLite to true
    - Set the value of the ApplicationInsight Connection String
    - Set a mock connection string to the new Azure SQL Database
- Build the container using Azure Container Registry (ACR) Build.

- You can use the following modules as a reference: 

    - [Building containers](/demos/02-containers/01-dev-workflow)    
    - [Container config management](/demos/02-containers/05-config-mgmt/)

## Task: Containerize Order Service

- Add a docker file to Orders Api build and test the container locally.
- Build the container using Azure Container Registry (ACR) Build.
    
## Task: Containerize Food Shop

- Add a docker file to Food Shop build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set ENV_CATALOG_API_URL to the Catalog Service Url
    - Set ENV_ORDERS_API_URL to the Order Service Url

- You can use the following modules as a reference: 

    - [Building Angular containers](/demos/00-app/config-ui/)    

## Task: Docker Compose

- Write a docker compose file to run the containers locally.

- You can use the following modules as a reference: 
    
    - [docker-compose.yml](/demos/02-containers/03-docker-dompose/docker-compose.yml)

## Task: Push to ACR

- Outsource the image creation using Azure Container Registry (ACR) Build Tasks

- You can use the following modules as a reference: 

    - [Azure Container Registry (ACR) Build Tasks](/demos/02-containers/02-publish/publish-images.azcli)    

## Task: Setup an Azure SQL Server and Database - Optional

- If your time permits you can setup an Azure SQL Server and Database and use it in the Catalog Service.