# Lab 01 - Architecture & Provisioning

In this lab you will deploy the base resources for the application. The resources will be deployed using Azure CLI Bicep. The resources will be deployed to a resource group named `az-native-dev`. The resources will be deployed to the `westeurope` region. The resources will be deployed using the free tier where possible.

![architecture](_images/app.png)

## Task: Provision Resources

- Use Azure CLI or Bicep to provision the resources below. 
- Use the provided names. 
- Store the outputs (endpoints and keys) needed for the app in Azure App Configuration and Azure Key Vault (keys & credentials).
- Assign the managed identity to Azure App Configuration resource
- Add the managed identity to the Azure Key Vault access policy with get and list permissions

- Resource Group: az-native-dev
- Managed Identity: az-native-mi-dev
- Azure Key Vault: az-native-kv-dev
- Azure App Configuration: az-native-app-config-dev
- Log Analytics Workspace & Application Insights: az-native-log-dev & -ai-dev
- Azure Container Registry: aznativecontainers-dev
- Azure Container Apps Environment: acaenv-az-native-dev
- Storage Account: aznativestoragedev
- Azure Cosmos Db Account (free tier - Autoscale limit to 1.000 RU): az-native-cosmos-nosql-dev
- Azure Cosmos DB: food-nosql-dev
- Azure Service Bus (standard tier): az-native-sb-dev

>Note: Create a the consumption based Api Management instance as the last resource and use the --no-wait flag to speed up the deployment.