# Publishing the Real Time Kitchen Dashboard

- Publish to Azure Container Apps

## Readings

[Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/)

## Publish to Azure Container Apps

In order to publish the Shop UI to Azure Container Apps you have to implement the following steps:

- Open project [Kitchen Dashboard](/app/web/cooking-dashboard/)

- Build and Publish the docker image using Azure Container Registry

    ```bash
    cd web/food-shop
    az acr build --image cooking-dashboard --registry $acr --file dockerfile .
    ```

- Get the [catalog-service](/app/services/catalog-service/) Url from APIM and provide it to the Container App

- Create a Container app with ingress enabled

    ```bash
    az containerapp create -n $appUI -g $grp --image $uiImg \
        --environment $acaenv \
        --target-port 80 --ingress external \
        --registry-server $loginSrv \
        --registry-username $acr \
        --registry-password $pwd \
        --env-vars ENV_API_URL=https://$apiUrl/ \
        --query properties.configuration.ingress.fqdn -o tsv
    ```

# Publish to Azure Static Web Apps

