# Solution - Container Essentials

- Build containers in their corresponding folders:

    ```bash
    cd catalog-service
    docker build --rm -f dockerfile -t catalog-service .
    cd ..
    cd order-service
    docker build --rm -f dockerfile -t order-service .
    cd ..
    cd food-shop
    docker build --rm -f dockerfile -t food-shop .
    cd ..
    ```
    >Note: Before building the Angular food-shop run: `npm install` to install the package dependencies.

- To test environment variables injection run:

    ```bash
    docker run -it --rm -p 5051:80 -e "Title=Container Essentials" -e "App:UseSQLite=true" -e "ApplicationInsights:ConnectionString=<Application_Insights_Connection_String>" catalog-service
    ```

- Test catalog-service using it swagger ui on http://localhost:5051/

- Docker compose:

    - Examine docker-compose.yml and run:

    ```bash
    docker-compose up
    ```

- Play with appsettings.json and override other config values until you feel absolutely comfortable with the concept of configuration in containers. Quickly note that in Azure Container Apps you will have to mimic the json structure in the environment variables by using the `__` separator. For example, to override the `App:UseSQLite` value you will have to set the `App__UseSQLite` environment variable.

- To build the containers using ACR Build Tasks, examine [create-images.azcli](./create-images.azcli) and run the script in WSL
