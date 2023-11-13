# Introduction to Dapr Actors

In this demo we will re-build the base of `Payment Service`, the bank, using Dapr Actors.

## Demo

- Run the [bank-actor](./bank-actor/) project using:

    ```bash
    dapr run --dapr-http-port 3500 --app-id bank_actor --app-port 5010 --resources-path './components' dotnet run 
    ```

- Start the [bank-client-console](./bank-client-console/) project in `F5-debug` mode.:    

## Links and Resources

[Dapr Actors](https://docs.dapr.io/developing-applications/building-blocks/actors/)

[Dapr Actors - Microsoft Lean](https://learn.microsoft.com/en-us/dotnet/architecture/dapr-for-net-developers/actors)