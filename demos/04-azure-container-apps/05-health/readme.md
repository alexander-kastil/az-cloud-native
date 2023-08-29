# Health Probes, Monitoring and Logging in Azure Container Apps

- Add [HealthController.cs](HealtController.cs) located in this  to [config-api](/demos/00-app/config-api/). It contains a simple health check endpoint that returns a 200 OK response. The liveness returns a BadRequest response when the number of logs exceeds 10. The readiness and startup endpoints return a 200 OK response.

    ```csharp
    // http://localhost:PORT/health/liveness
    [HttpGet("liveness")]
    public IActionResult GetLiveness()
    {
        LogProbe($"{DateTime.UtcNow} -- Liveness {logs.Count}");
        if (logs.Count <= 10)
            return Ok();
        else
            return BadRequest();
    }

    // http://localhost:PORT/health/readiness
    [HttpGet("readiness")]
    public IActionResult GetReadiness()
    {
        LogProbe($"{DateTime.UtcNow} -- Readiness {logs.Count}");
        return Ok();
    }

    // http://localhost:PORT/health/startup
    [HttpGet("startup")]
    public IActionResult GetStartup()
    {
        LogProbe($"{DateTime.UtcNow} -- Startup {logs.Count}");
        return Ok();
    }
    ```

- Build and push the image to ACR:

    ```bash
    env=dev
    acr=aznative$env
    imgApi=config-api:v4
    az acr build --image $imgApi --registry $acr --file dockerfile .
    ```    

- Update the container app:

    ```bash
    env=dev
    grp=az-native-$env
    loc=westeurope
    appApi=config-api
    apiImg=$acr.azurecr.io/$appApi:v4

    az containerapp update -n $appApi -g $grp --image $apiImg
    ```

- Create healt probes manually    