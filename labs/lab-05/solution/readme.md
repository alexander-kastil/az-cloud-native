# Solution - Domain Driven Design, NoSQL Data & Event storage using Cosmos DB

## Architecture

![architecture](_images/app.png)

## Task: Domain Driven Design

![domain-model](_images/domain-model.png)

## Task: Order Service Bounded Context

Food App Messaging Flow

![message-flow](_images/message-flow.png)

Order Service Bounded Context

todo: create bounded context canvas

## Task: Designing the Data Model

![data-model](_images/data-model.png)

## Task: Create the Physical Design

![physical-design](_images/physical-design.png)

## Task: Containerize Apps

```bash
env=dev
grp=az-native-$env
loc=westeurope
acr=aznativecontainers$env
cd order-events-processor
az acr build --image order-events-processor --registry $acr --file dockerfile .
cd ..
cd orders-service-cqrs
az acr build --image orders-service-cqrs --registry $acr --file dockerfile .
cd ..
```

## Task: Test the CQRS Orders Service