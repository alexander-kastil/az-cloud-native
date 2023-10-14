# Deploying a multi-container App (Ingress, Egress)

Azure Container Apps is a serverless container host that enables you to run microservices and containerized applications. 
It is built on top of Kubernetes with a developer centric focus

Azure Container Apps is ideal for:

1. Deploying API endpoints, 
2. Hosting background processing jobs, 
3. Handling event-driven processing,
4. Apps that need scaling using KEDA

## Links and Resources

[Azure Container Apps documentation](https://learn.microsoft.com/en-us/azure/container-apps/)

## Demo

Task 1: Create a container app environment and deploy two apps

- Execute [create-aca-env.azcli](create-aca-env.azcli) to create the base resources of the app. It creates the following resources:

    - resource group
    - container registry
    - log analytics workspace
    - container app environment

- Execute [publish-images.azcli](/demos/00-app/publish-images.azcli) to build and publish the images to the container registry. 

- Execute [deploy-app.azcli](deploy-app.azcli) to deploy the apps to the container app environment. It deploys the following apps:

    - [config-api](/demos/00-app/config-api/)
    - [config-ui](/demos/00-app/config-ui/)