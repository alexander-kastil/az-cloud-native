# Using Service Connector to access services

- Build the image and push it to ACR:

    ```bash
    env=dev
    acr=aznative$env
    img=kv-api:v2
    az acr build --image $img --registry $acr --file dockerfile .
    ```

- Execute [create-kv-app-sc.azcli](create-kv-app-sc.azcli)

- Create the service connection manually
