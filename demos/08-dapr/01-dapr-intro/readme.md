# Dapr Introduction

This sample introduces Dapr and how to use it to build microservices and is based on the [Dapr quickstarts](https://docs.dapr.io/getting-started/quickstarts/). 

It contains two projects:

- `food-dapr-backend` - A .NET Core Web API project that uses Entity Framework and Dapr to store and retrieve food orders.
- `food-dapr-frontend` - An MVC project that uses Dapr to call the backend API.

Dapr configuration is stored in the [components](components) folder and container the following files:

- `statestore.yaml` - Configures the state store to use Azure Blob Storage.

## Demo Steps

- Install Dapr CLI

    ```
    Set-ExecutionPolicy RemoteSigned -scope CurrentUser
    powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
    ```

    >Note: Restart the terminal after installing the Dapr CLI

- Initialize default Dapr containers

    ```
    dapr init
    ```

    ![dapr-init](_images/dapr-init.png)

   >Note: To remove the default Dapr containers run `dapr uninstall` 

- Run project `food-dapr-backend`

    ```
    cd food-dapr-backend
    dapr run --app-id food-backend --app-port 5001 --dapr-http-port 5010 dotnet run
    ```

- Run project `food-dapr-fronted`

    ```
    cd food-dapr-fronted
    dapr run --app-id food-fronted --app-port 5002 --dapr-http-port 5011 dotnet run
    ```    

- Show Dapr Dashboard

    ```
    dapr dashboard
    ``` 

- Examine Dapr Dashboard on http://localhost:8080:

    ![dapr-dashboard](_images/dapr-dashboard.png)

- Examin Dapr Debug Attach Config in `launch.json`:

    ```json
    {
        "name": "Dapr Attach",
        "type": "coreclr",
        "request": "attach",
        "processId": "${command:pickProcess}"
    }
    ```

- Lauchn `Dapr Attach` Config and filter for the `hello-food-dapr.exe` process to attach the debugger.

    ![filter-process](_images/filter-process.png)

- Install [Tye](https://github.com/dotnet/tye/). Project Tye is an experimental developer tool that makes developing, testing, and deploying microservices and distributed applications easier

    ```
    dotnet tool install -g Microsoft.Tye --version "0.11.0-alpha.22111.1"
    ```

- Create a `tye.yaml` file in the root of the solution by running:

    ```    
    tye init
    ```

    >Note: You can skip this step as the `tye.yaml` file is already included in the solution.

- Run the two projects with Tye

    ```
    tye run
    ```    

    ![tye](_images/tye.png)