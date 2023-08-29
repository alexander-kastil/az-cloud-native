# Build Serverless OData APIs with Azure Functions

- Install packages in [/db-setup](db-setup) and [/az-rest-funct](az-rest-funct) folders

    ```bash
    npm install
    ```

- Run [db-setup/create-db.azcli](db-setup/create-db.azcli) in folder `db-setup` to create the db. It uses `@azure/cosmos` to populate the db with sample data.

- Replace the connection string in `local.settings.json`

    ```json
    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "",
            "FUNCTIONS_WORKER_RUNTIME": "node",
            "CONNECTION_STRING": "YOUR CONNECTION_STRING",
            "dbname": "productsdb"
        },
        "Host": {
            "CORS": "*"
        }
    }
    ```

- Execute [az-rest-funct/create-product.http](az-rest-funct/create-product.http)
- Execute [az-rest-funct/get-products.http](az-rest-funct/get-products.http)

