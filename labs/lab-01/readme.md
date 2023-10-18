# Lab 01 - Architecture & Provisioning

In this lab you will deploy the base resources for the application. The resources will be deployed using Azure CLI Bicep. The resources will be deployed to a resource group named `az-native-dev`. The resources will be deployed to the `westeurope` region. The resources will be deployed using the free tier where possible.

![architecture](_images/app.png)

## Task: Provision Resources

Use Azure CLI Bicep to provision the resources below. Use the provided names. Store the outputs (endpoints and keys) needed for the app in Azure App Configuration and Azure Key Vault (keys & credentials).

- Resource Group: az-native-dev
- Azure App Configuration: az-native-app-config-dev
- Azure Key Vault: az-native-kv-dev
- Log Analytics Workspace & Application Insights: az-native-log-dev & -ai-dev
- Azure Container Registry: aznativecontainers-dev
- Azure Container Apps Environment: acaenv-az-native-dev
- Storage Account: aznativestoragedev
- Azure SQL Server (free tier): az-native-sql-dev (server admin: `aznative-admin` / `P@ssw0rd1234`)
- Azure Cosmos Db Account (free tier): az-native-cosmos-nosql-dev
- Azure Cosmos DB: food-nosql-dev
- Azure Service Bus: az-native-sb-dev
- Azure Event Grid: az-native-eg-dev
- Api Management: az-native-apim-dev
- Managed Identity: az-native-mi-dev
