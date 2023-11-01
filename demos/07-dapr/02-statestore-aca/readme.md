# Use StateStore Component in Azure Container Apps

## Links and Resources

[Dapr State Store Components](https://docs.dapr.io/reference/components-reference/supported-state-stores/)

[Dapr Component Schema](https://learn.microsoft.com/en-us/azure/container-apps/dapr-overview?tabs=bicep1%2Cyaml#component-schema)

## Demo

>Note: This guide continues using [food-service-dapr](../00-app/food-service-dapr/) from the previous module.

- Add DaprClient to `Program.cs`

    ```c#
    var builder = WebApplication.CreateBuilder(args);
    ...
    // Add DaprClient to the ioc container
    builder.Services.AddDaprClient();
    ```
- Examine `CountController.cs` and call `getCount()` multiple times to increment the counter and receive its current value:

    ```c#
    public CountController(DaprClient daprClient)
    {
        client = daprClient;
    }

    [HttpGet("getCount")]
    public async Task<int> Get()
    {
        var counter = await client.GetStateAsync<int>(storeName, key);
        await client.SaveStateAsync(storeName, key, counter + 1);
        return counter;
    }
    ```

- Execute the following command to create a Redis state store component in the `food-api-dapr` project:

    ```bash
    dapr run --app-id food-api --app-port 5000 --dapr-http-port 5010 --resources-path './components' dotnet run
    ```     

- To increment the counter you can use the pre-configured REST calls in [test-dapr.http](./food-service-dapr/test-dapr.http) which is using the [Rest Client for Visual Studio Code Extension](https://marketplace.visualstudio.com/items?itemName=humao.rest-client).      

    ```bash
    @baseUrl = http://localhost:5000
    ### Get the count and increment it by 1
    GET {{baseUrl}}/count/get HTTP/1.1
    GET {{baseUrl}}/count/reset HTTP/1.1
    ```

- Examine the `Dapr Attach` config in `launch.json` and use it to attach the debugger to the `food-api-dapr` process and debug the state store code:

    ```json
    {
        "name": "Dapr Attach",
        "type": "coreclr",
        "request": "attach",
        "processId": "${command:pickProcess}"
    }
    ```
    ![filter-process](_images/filter-process.png)

### Deploy to Azure Container Apps

- Build the food-api-dapr image

    ```bash
    env=dev
    loc=westeurope
    grp=az-native-$env
    acr=aznativecontainers$env
    imgBackend=food-api-dapr:v1
    az acr build --image $imgBackend --registry $acr --file dockerfile .
    ```
- Execute [create-storage-container.azcli](./create-storage-container.azcli) to create a storage account and a container to store the state store data

- Examine the schema for the Azure Blob Storage state store component:

    ```yaml
    apiVersion: dapr.io/v1alpha1
    kind: Component
    metadata:
    name: <NAME>
    spec:
    type: state.azure.blobstorage
    version: v1
    metadata:
    - name: accountName
        value: "[your_account_name]"
    - name: accountKey
        value: "[your_account_key]"
    - name: containerName
        value: "[your_container_name]"
    ```        

- Note that Azure Container Apps uses a simplified schema which is implemented in [statestore-blob.yaml](./statestore-blob.yaml)    


    ```yml
    componentType: state.azure.blobstorage
    version: v1
    metadata:
    - name: accountName
    value: "[your_account_name]"
    - name: accountKey
    value: "[your_account_key]"
    - name: containerName
    value: "[your_container_name]"
    ```

- Add the Dapr component to the Azure Container Apps environment

    ```bash
    az containerapp env dapr-component set -n $acaenv -g $grp \
        --dapr-component-name $statestore \
        --yaml statestore-blob.yaml
    ```    
    >Note. In Azure Portal you can also create the Dapr component in the Azure Container Apps environment. It allows you to choose between Redis, Azure Blob Storage, Azure Cosmos DB and others as a state store. The interaction with the specifics of the state store is abstracted away by Dapr:

- Execute deploy-app.azcli to create the container app

    ```bash
    az containerapp create -n $appBackend -g $grp \
    --image $imgBackend \
    --environment $acaenv \
    --target-port 80 --ingress external \
    --min-replicas 0 --max-replicas 1 \
    --enable-dapr \
    --dapr-app-port 80 \
    --dapr-app-id $appBackend \
    --registry-server $loginSrv \
    --registry-username $acr \
    --registry-password $pwd 
    ```

    >Note: Accessing ACR could also done using a managed identity. Check the [documentation](https://learn.microsoft.com/en-us/azure/container-apps/managed-identity-image-pull?tabs=azure-cli&pivots=command-line) for more details.

- Execute the /count/getCount method multiple times to increment the counter

    ```bash
    curl -X GET "http://<URL>.$loc.azurecontainer.io/count/getCount" -H  "accept: text/plain"
    ```

- Examine the storage account to see the state store data

    ![counter](_images/counter.png)
