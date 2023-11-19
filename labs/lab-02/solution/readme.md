# Solution - Container Essentials

- Build containers:

    ```bash
    docker build --rm -f dockerfile -t catalog-service .
    docker build --rm -f dockerfile -t order-service .
    docker build --rm -f dockerfile -t food-shop .
    ```
    >Note: Before building the Angular app run: `npm install` to install the dependencies.

- To test environment variables injection run:

    ```bash
    docker run -it --rm -p 5051:80 -env "Title:Container Essentials" -env "App:UseSQLite=true" -env "ApplicationInsights:ConnectionString='<CONNECTION_STRING>'" catalog-service
    ```

- Test catalog-service using it swagger ui on http://localhost:5051/

- Docker compose:

    - Examine docker-compose.yml and run:

    ```bash
    docker-compose up
    ```

- To build the containers using ACR Build Tasks, examine [create-images.azcli](./create-images.azcli) and run the script in WSL
