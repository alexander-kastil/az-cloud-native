# Lab 05 - Domain Driven Design, NoSQL Data & Event storage using Cosmos DB

In this lab we will design our basic data model and keep aspects of Domain Driven Design in mind. 

We will use Azure SQL for the Catalog service, Redis for Production (Kitchen) and Cosmos DB as our NoSQL database and event store for all other services. 

## Task: Domain Driven Design

- With the given Architecture diagram in mind, identify the entities, value objects and aggregates for the following services. We will discuss the results in class afterwards.

  - Catalog Service
  - Orders Service
  - Payment Service
  - Kitchen Service (Production)
  - Delivery Service
  
    ![architecture](_images/app.png)

- Do not go into too much detail. Just identify the main entities and value objects for each service as you will create 

- You will use the output of this task later on to create containers in Cosmos DB.

- You can use a pice of paper or an online whiteboard like [draw.io](https://draw.io/) or [Miro](https://miro.com/) to draw your results.  

## Task: Order Service Bounded Context

- Document the `Order Service Bounded Context`. If you want you can use [Miro and the Bounded Context Canvas](https://miro.com/miroverse/the-bounded-context-canvas/), some other tool or even a piece of paper.

- Follow the guide on [GitHub - https://github.com/ddd-crew/bounded-context-canvas](https://github.com/ddd-crew/bounded-context-canvas)


## Task: Designing the Data Model

- `Catalog Service` has one main entity - `CatalogItem`:

    ```c#
    public class CatalogItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int InStock { get; set; } 
        public string PictureUrl { get; set; }
        public string Description { get; set; }
    }
    ```

- `Food Shop UI` has the following entities and value objects:   

    ```typescript
    export class Order {
        id = '';
        customer: Customer;
        shippingAddress: Address;
        payment: Payment;
        items: CartItem[];
        total = 0;
    }

    export class Payment {
        type = '';
        account = '';
    }

    export class Address {
        street = '';
        city = '';
        state = '';
        zip = '';
    }

    export class Customer {
        id = '';
        name = '';
        email = '';
        phone = '';
    }

    export class CartItem {
        id = 0;
        name = '';
        price = 0;
        quantity = 0;
    }
    ```

- Design the data model for the `Orders Service` based on the entities and value objects you identified in the previous task and the `Food Shop UI` data model.

- You can use the [Miro - Entity Relationship Diagram Template](https://miro.com/templates/entity-relationship-diagram/) or some other tool or even a piece of paper.

- If you don't want to draw charts just implement the classes and the interfaces for the message flow in `C#` or `TypeScript` and document the message flow in the code. You can do this in a separate project or in plain markdown or a diagram tool of your choice.

## Task: Create & Deploy the Physical Design

- Use the Cosmos DB Account created in Lab 01 to create the containers for `food-app` in a `food-app` database. To keep this simple we will use the same database for all services. In a real world scenario you would create a database for each service.

- Create the following containers and chose a partition key 

  - `Orders` for the `Orders Service`
  - `Payments` for the `Payment Service`
  - `Production` for the `Kitchen Service`
  - `Deliveries` for the `Delivery Service`  

- Use `IaC (Azure CLI or Bicep)` in order to be able to drop and recreate the containers easily.

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