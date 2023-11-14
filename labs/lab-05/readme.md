# Lab 05 - Domain Driven Design, NoSQL Data & Event storage using Cosmos DB

In this lab we will design our basic data model and keep aspects of Domain Driven Design in mind. 

We will use Azure SQL for the Catalog service, and Cosmos DB as our NoSQL database and event store for all other services. 

## Task: Domain Driven Design

- With the given Architecture diagram in mind, identify the entities, value objects and aggregates for the following services. We will discuss the results in class afterwards.

  - Catalog Service
  - Orders Service
  - Payment Service
  - Kitchen Service (Production)
  - Delivery Service
  
    ![architecture](_images/app.png)

- Do not go into too much detail. Just identify the main entities and value objects for each service as you will create. 

- Possible Solution:

  ![domain-model](_images/domain-model.png)

## Task: Designing the Data Model

- `Catalog Service` has one main entity:

    ![catalog-item](_images/catalog-item.png)

- `Food Shop UI` has the following entities and value objects:   

    ![food-shop-ui](_images/food-shop-ui.png)

- Design the data model for the `Orders Service` based on the entities and value objects you identified in the previous task and the `Food Shop UI` data model.

- Possible Solution:

  ![data-model](_images/data-model.png) 

## Task: Create & Deploy the Physical Design

- Use the Cosmos DB Account created in Lab 01 to create the containers for `food-app` in a `food-nosql-$env` database. To keep this simple we will use the same database for all services. In a real world scenario you would create a database for each service.

- Create the containers `orders` and `order-events` and chose a partition key. Use `IaC (Azure CLI or Bicep)` in order to be able to drop and recreate the containers easily.

- Possible Solution:

  ![physical-design](_images/physical-design.png) 

## Task: Event Sourcing & CQRS

- Take the [Event Sourcing](../../demos/05-cosmos/05-event-sourcing/) & [CQRS demos](../../demos/05-cosmos/06-cqrs/) from this module as a reference and implement CQRS & Event Sourcing for the `Orders Service` in your own project.

    - Deploy Cosmos DB and the required containers
    - Test the event store
    - Implement the event processor
    - Implement the CQRS pattern using MediatR
    - Test the CQRS pattern
    - Add a `getAllOrdersForCustomer` method with a `customerId` parameter to the `OrdersController` and implement the query side of the CQRS pattern
    
    >Note: You can copy some of the code but use your own project to set up the solution. The goal is to be able to setup CQRS & Event Sourcing on your own.   

## Task: Containerize Apps

- Containerize the following apps:

  - Food Shop UI
  - Orders Service CQRS
  - Catalog Service

## Task: Test the CQRS Orders Service

- Test the following apps are working together correctly on Azure Container Apps:

  - Food Shop UI
  - Orders Service CQRS
  - Order Events Processor
  - Catalog Service

- If your time permits you can deploy them to Azure Container Apps and test them there.  