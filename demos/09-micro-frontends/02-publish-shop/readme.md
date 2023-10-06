# Publishing the Shop Micro Frontend using Azure Container Apps



## Readings



## Demo

In order to publish the Shop UI to Azure Container Apps you have to implement the following steps:

- Open project [shop-ui](/app/web/shop-ui/)

- Build and Publish the docker image using Azure Container Registry

    ```bash
    cd web/shop-ui
    az acr build --image $imgShopUI --registry $acr --file dockerfile .
    ```

- Get the [catalog-api](/app/services/catalog-api/) Url from APIM and provide it to the Container App

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