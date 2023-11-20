# Lab 04 - Stateful Microservices using Azure Durable Entities

- Create a Durable Entity Bank Account Microservice using Functions
- Publish the microservice to Azure Functions

## Task: Create a Durable Entity Bank Account Microservice using Functions

In this lab you will create a stateful microservice using Azure Functions. The microservice will be used to create a customer account using a functional approach. It will offer the following operations:

- GetBalance
- Deposit / Withdraw
- Execute Payment

You can use the [starter project](./starter/payment-service-func/) as a reference.

## Task: Publish the microservice to Azure Functions

- Create Function App Resources using Azure CLI IaC to host the microservice

- Script the deployment using [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=windows%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp#publish)