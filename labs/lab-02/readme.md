# Lab - Container Essentials

In this Lab we will explore the basics of containers. We will start by containerizing the following apps:

- Catalog Api
- Shop UI
- Orders Api

## Task: Containerizing the Catalog Api

- Add a docker file to Catalog Api build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set UseSQLite to false
    - Create a new Azure SQL Database
    - Set the connection string to the new Azure SQL Database
    
## Task: Containerizing the Shop UI

- Add a docker file to Shop UI build and test the container locally.
- Override values from appsettings.json using environment variables.
    - Set CatalogUrl to the Catalog Api container url
    - Set OrdersUrl to the Orders Api container url

## Task: Containerizing the Orders Api

- Add a docker file to Orders Api build and test the container locally.

## Task: Docker Compose

- Write a docker compose file to run the containers locally.

## Task: Push to ACR

- Outsource the container build to ACR