# Lab 08 - Optimizing and Securing Access using Api Management & Application Gateway

In this lab we will publish Catalog and Order services using Azure API Management and secure access to the services using Azure Application Gateway.

## Task: Publish Catalog and Order services using Azure API Management and consume this services from the Food Shop UI

- Create a new API Management instance using the `Consumption pricing tier` in the same resource group as the other services.

- Create a new API in API Management for the Catalog service hosted by Azure Container Apps. Use the suffix `catalog` for the API URL.

    ![add-api](_images/add-api.png)

- Repeat this steps for the Order Service CQRS using the suffix `orders`.

    >Note: If you have re-deployed your services, do not forget to set `* CORS policies` in Azure Container Apps for the Catalog and Order services.

- Create a Subscription Key for all APIS in the API Management instance and add it to the KeyVault as a secret with the name `subscription-key` so you cannot forget it.

    ![subscription-key](_images/subscription-key.png)

- The Subscription key will be attached to the header of the http requests to the services using a functional Angular interceptor that takes the value from the Angular environment. You will override the value by injecting an environment variable `ENV_APIM_KEY` in the Azure Container Apps instance.

    ```typescript
    export function apimInterceptor(req: HttpRequest<unknown>,
        next: HttpHandlerFn) {
        var request = req.clone({
            headers: req.headers.set(
                'Ocp-Apim-Subscription-Key',
                environment.azure.apimSubscriptionKey
            )
        });
        return next(request);
    };
    ```

- The interceptor is will be registered in the Angular App

    ```typescript
    providers: [
        ...
        provideHttpClient(withInterceptors([apimInterceptor]))
    ],    
    ```

    >Note: For your better understanding you can check the Network tab in the browser developer tools to see the http requests and the headers.

    ![network-tab](_images/network-tab.png)

- Deploy or re-deploy the Food Shop UI to Azure Container instances and set the environment variable `ENV_APIM_KEY` to the value of the `subscription-key` secret in the KeyVault. Also update the values for `ENV_CATALOG_API_URL` and `ENV_ORDERS_API_URL` to reflect the `Gateway URL` You should be familiar with this process by now. Use the following values for the URLs:

    ```
    Service URL: `https://<your-apim-name>.azure-api.net/<service-suffix>`
    ```

    >Note: As alternative to hosting a Single Page Application in a container, you could as well use [Azure Static Web Apps](https://learn.microsoft.com/en-us/azure/static-web-apps/). A possible Azure DevOps pipeline is available [here](/app/deploy/pipelines/angular-ci-cd-swa.yml). It uses a [tokenizer](https://josh-ops.com/posts/angular-tokenization/) as injecting environment variables is not an option in Static Web Apps, and would need adjustments to reflect our configuration. In this class we use container based deployment to have a consistent deployment process for all services and frontends.

## Task: Implement a Backend for Frontend (BFF) service using GraphQL

- Create a file catalog-schema.graphql and paste the following schema into it. It does not include the description property for the catalog items as it will not be displayed in the mobile app. It also contains a query for all catalog items.

    ```graphql
    type CatalogItem{
        id: String!
        name: String!
        price: Float!
        inStock: Boolean!
        pictureUrl: String!
    }

    type Query{
        catalogItems: [CatalogItem]
    }
    ```
- In your APIM instance create a new GraphQL API using the suffix `catalog-mobile` and the schema from the file you just created.

    ![new-graphql](_images/new-graphql.png)

- The schema will be displayed in the APIM portal. You can also test the API using the `Run` button.

    ![graphql-schema](_images/graphql-schema.png)

- Go to settings and enter the Web API URL of the Catalog service hosted by Azure Container Apps. The URL is available in the Azure Container Apps instance overview. Add `/food` to the URL as this is the path `food query`

    ![graphql-settings](_images/graphql-settings.png)    

- Last you will add a resolver for the query:

    ![graphql-resolver-catalogItems](_images/graphql-resolver-catalogItems.png)

- Now you can test the API using the `Run Test` button. Before you test make sure that the container app is running and the Catalog service is available.