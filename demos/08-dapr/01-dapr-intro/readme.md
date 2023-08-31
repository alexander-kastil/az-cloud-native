# Dapr Introduction

This sample introduces Dapr and how to use it to build microservices and is based on the [Dapr quickstarts](https://docs.dapr.io/getting-started/quickstarts/). 

It contains two projects:

- `food-dapr-backend` - A .NET Core Web API project that uses Entity Framework and Dapr to store and retrieve food orders.
- `food-dapr-frontend` - An MVC project that uses Dapr to call the backend API.

## Demo Steps

- Install Dapr CLI

    ```
    Set-ExecutionPolicy RemoteSigned -scope CurrentUser
    powershell -Command "iwr -useb https://raw.githubusercontent.com/dapr/cli/master/install/install.ps1 | iex"
    ```

    >Note: Restart the terminal after installing the Dapr CLI

- Initialize self-hosted Dapr

    ```
    dapr init
    ```

- Run project `food-dapr-backend`

    ```
    cd food-dapr-backend
    dapr run --app-id food-backend --app-port 5001 --dapr-http-port 5010 dotnet run
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

- Repeat the prev steps to create a second project that you with a name of your choice, ie: `second-dapr`.

- Install [Tye](https://github.com/dotnet/tye/). Project Tye is an experimental developer tool that makes developing, testing, and deploying microservices and distributed applications easier

    ```
    dotnet tool install -g Microsoft.Tye --version "0.11.0-alpha.22111.1"
    ```

- Create a `tye.yaml` file in the root of the solution by running:

    ```    
    tye init
    ```

- Run the two projects with Tye

    ```
    tye run
    ```    

    ![tye](_images/tye.png)