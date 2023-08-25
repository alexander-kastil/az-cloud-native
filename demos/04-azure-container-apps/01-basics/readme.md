# Deploying a muliti-container App (Ingress, Exgress)

[Azure Container Apps documentation](https://learn.microsoft.com/en-us/azure/container-apps/)

## Demo

[food-catalog-api](/app/food-catalog-api/) provides a REST API to manage a food catalog.

[food-shopp-ui](/app/food-shop-ui//) consumes it and provides an Online Food Shop implemented in Angular.

Task 1: Create a container app environment and deploy two apps

- Execute [create-aca-env.azcli](create-aca-env.azcli) to create the base ressources of the app. It creates the following resources:

    - resource group
    - container registry
    - log analytics workspace
    - container app environment

- Excute [deploy-apps.azcli](deploy-apps.azcli) to deploy the apps to the container app environment.