# Lab - Container Essentials

In this Lab we will explore the basics of containers. We will start by containerizing the following apps:

- Catalog Api
- Orders Api
- Food Shop UI

## Task: Containerizing the Catalog Api

- Add a docker file to Catalog Api build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set UseSQLite to false
    - Create a new Azure SQL Database
    - Set the connection string to the new Azure SQL Database

    >Note: You can use the following module as a reference: 

    - [container build](/demos/02-containers/01-dev-workflow)    
    - [config management](/demos/02-containers/05-config-mgmt/)    
    
## Task: Containerizing the Food Shop UI

- Add a docker file to Shop UI build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set CatalogUrl to the Catalog Api container url
    - Set OrdersUrl to the Orders Api container url

## Task: Containerizing the Orders Api

- Add a docker file to Orders Api build and test the container locally.

## Task: Docker Compose

- Write a docker compose file to run the containers locally. Use the following [reference](/demos/02-containers/03-docker-dompose/docker-compose.yml)

## Task: Push to ACR

- Outsource the container build to ACR. Use the following [reference](/demos/02-containers/02-publish/publish-images.azcli)