# Deploying a muliti-container App (Ingress, Exgress)

[Azure Container Apps documentation](https://learn.microsoft.com/en-us/azure/container-apps/)

## Demo

Task 1: Create a container app environment and deploy two apps

- Execute [create-aca-env.azcli](create-aca-env.azcli) to create the base ressources of the app. It creates the following resources:

    - resource group
    - container registry
    - log analytics workspace
    - container app environment

- Execute [publish-images.azcli](/demos/00-app/publish-images.azcli) to build and publish the images to the container registry. 

- Excute [deploy-app.azcli](deploy-app.azcli) to deploy the apps to the container app environment. It deploys the following apps:

    - [config-api](/demos/00-app/config-api/)
    - [config-ui](/demos/00-app/config-ui/)