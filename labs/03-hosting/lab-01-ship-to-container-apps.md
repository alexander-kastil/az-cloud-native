# Ship Contoso Logistics to Container Apps

Contoso Logistics runs only on your machine. Nobody outside your laptop can book a shipment, the carrier API key has no safe home, and a booking leaves no trace you can search in production. By the end, shipping-api and tracking-service run in Azure Container Apps, read the key and a setting through managed identity, release tracking-service v2 behind a 90/10 traffic split, and answer a booking question with a KQL query.

What you practice here: deploying an Aspire AppHost to Container Apps and wiring its services to Azure over managed identity.

Duration: about 55 minutes.

Topics practiced: [Aspire Deploy: ACR & Container Apps](../../demos/03-hosting/01-aspire-deploy/readme.md), [Key Vault & App Configuration](../../demos/03-hosting/02-key-vault-app-config/readme.md), [Revisions: Traffic Splitting & Draining](../../demos/03-hosting/03-revisions/readme.md), [Application Insights: Traces & KQL](../../demos/03-hosting/05-app-insights/readme.md)

------

Setup: run `az login` yourself, then run each command in `labs/03-hosting/provision.azcli` once. You need the Azure CLI, the Aspire CLI and a running container runtime.

Open `labs/03-hosting/contoso-logistics/` in your editor with your AI coding agent (GitHub Copilot agent mode or Claude Code). The agent reads `AGENTS.md` for the class stack, and every path in the prompts is relative to that folder. The starter is the lab 2 solution.

---

## Step 1: Deploy the AppHost to Container Apps

Overview: `aspire deploy` turns the AppHost model into a Container Apps environment, a registry and one container app per service. This step declares the environment, gives shipping-api and tracking-service external endpoints, and maps `/health` outside Development. ServiceDefaults maps `/health` only in Development, and you call it over the public endpoint.

Research: Ask the agent to reason through the deploy before any file changes.

```text
Read src/AppHost/Program.cs in this folder. List every resource it declares. For each one, say whether it runs as an emulator during a local run and what aspire deploy creates in Azure for it. Cite the Aspire deployment page you used for the Azure part. Do not change any file.
```

Finding: A good answer separates the emulator setting, which applies to local runs, from the Azure resource that a deploy creates. It names the Container Apps environment, the registry and one container app per compute resource. Push back if it says an emulator container runs inside Container Apps without citing a source, or if it says a resource is skipped without naming the page that says so.

Recipe:

```text
Work in this folder. Read src/AppHost/Program.cs and src/ServiceDefaults/Extensions.cs first.

In src/AppHost/Program.cs, add an Azure Container Apps environment named aca-env with AddAzureContainerAppEnvironment. Add the Aspire hosting package for Azure Container Apps to src/AppHost/ContosoLogistics.AppHost.csproj. Keep every resource that lab 2 declares.

Add WithExternalHttpEndpoints to the shipping-api and tracking-service project resources. This lab deploys only shipping-api and tracking-service, so exclude ops-console and exception-agent from publish and deploy with the Aspire API for that, and keep them in the AppHost for local runs.

In src/ServiceDefaults/Extensions.cs, change MapDefaultEndpoints so that /health maps in every environment, not only Development. The /health response is JSON with a status property and an entries object keyed by health check name, where each entry has its own status property. Keep /alive as the template generates it.

Use the Aspire 13.6 packages and write no code comments.
```

Expected Outcome: Set the Azure settings that `aspire deploy` reads, so it asks no questions. In Bash:

```bash
export Azure__SubscriptionId="$(az account show --query id --output tsv)"
export Azure__Location="swedencentral"
export Azure__ResourceGroup="rg-azn-labs"
```

In PowerShell, set `$env:Azure__SubscriptionId`, `$env:Azure__Location` and `$env:Azure__ResourceGroup` the same way. Then run `aspire deploy --non-interactive` from the `contoso-logistics/` folder. Every later `aspire deploy` in this lab uses the same variables. The command finishes without errors. Then run these commands:

```bash
az containerapp list --resource-group rg-azn-labs --output table
az containerapp show --name shipping-api --resource-group rg-azn-labs --query properties.configuration.ingress.fqdn --output tsv
```

Open `https://<SHIPPING_API_FQDN>/health` in a browser. The page shows JSON with a status property of `Healthy`.

> **_NOTE:_** Gate tests could prove this step, but this lab skips them to save time. They would check:
>
> - The AppHost, built without starting it, declares the Container Apps environment `aca-env`, the resources `key-vault`, `config` and `app-insights`, and external endpoints on `shipping-api` and `tracking-service`.
> - The deployed `shipping-api` and `tracking-service` both answer `GET /health` with 200 and a JSON body whose status is `Healthy`.

## Step 2: Read the Carrier Key from Key Vault

Overview: The carrier API key stays out of source code, configuration files and the container image. This step adds a Key Vault to the AppHost, and Aspire grants the shipping API read access to it through its managed identity. A health check reads the secret on each call, so `/health` proves the read works.

Recipe:

```text
Work in this folder. Read src/AppHost/Program.cs, src/ShippingApi/Program.cs and src/ShippingApi/ShippingApi.csproj first.

In src/AppHost/Program.cs, add an Azure Key Vault resource named key-vault with AddAzureKeyVault. Add the Aspire hosting package for Azure Key Vault to src/AppHost/ContosoLogistics.AppHost.csproj. Reference key-vault from shipping-api with WithReference, so Aspire grants the shipping API a Key Vault role on the vault.

In src/ShippingApi/ShippingApi.csproj, add the Aspire client package Aspire.Azure.Security.KeyVault and the package Azure.Security.KeyVault.Secrets. In src/ShippingApi/Program.cs, register the Key Vault client from the Aspire integration.

Create src/ShippingApi/CarrierKeyHealthCheck.cs with a class named CarrierKeyHealthCheck that implements IHealthCheck. The check reads the secret named carrier-api-key through the SecretClient, returns Healthy when the read succeeds, and returns Unhealthy with the exception message when it fails. Register the check in src/ShippingApi/Program.cs under the name carrier-api-key.

Run dotnet build ContosoLogistics.sln and fix every error. When a package version conflicts with the one the Aspire client package requires, use the required version.

Write no code comments.
```

Expected Outcome: Run `aspire deploy` from the `contoso-logistics/` folder again. It creates the vault and redeploys shipping-api. Then grant your sign-in access to the vault and store the secret. Replace the placeholders with the names your deployment created:

```bash
az keyvault list --resource-group rg-azn-labs --query "[].name" --output tsv
az keyvault show --name <VAULT_NAME> --resource-group rg-azn-labs --query id --output tsv
az role assignment create --assignee <SIGN_IN_EMAIL> --role "Key Vault Secrets Officer" --scope <VAULT_ID>
az keyvault secret set --vault-name <VAULT_NAME> --name carrier-api-key --value lab-carrier-key-0001
```

Role assignments take a minute or two to propagate, so the `carrier-api-key` entry can read Unhealthy until then. Open `https://<SHIPPING_API_FQDN>/health` again. The JSON lists `carrier-api-key` with a status of `Healthy`.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - The `carrier-api-key` entry in the deployed `shipping-api` health report is `Healthy`, which proves the service read the secret with its managed identity.

## Step 3: Read a Setting from App Configuration

Overview: A setting that changes between releases belongs in App Configuration, not in a file that ships with the build. This step declares a store, lets tracking-service read `Tracking:PageSize` through its managed identity, and sets the value from the CLI.

Recipe:

```text
Work in this folder. Read src/AppHost/Program.cs, src/TrackingService/Program.cs and src/TrackingService/TrackingService.csproj first.

In src/AppHost/Program.cs, add an Azure App Configuration resource named config with AddAzureAppConfiguration. Do not call RunAsEmulator. Reference config from tracking-service with WithReference, so Aspire grants the service a role on the store. Add the Aspire hosting package for Azure App Configuration to src/AppHost/ContosoLogistics.AppHost.csproj.

In src/TrackingService/TrackingService.csproj, add the Aspire client package Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration. In src/TrackingService/Program.cs, add the App Configuration provider through the Aspire integration and load the keys that start with Tracking:.

Create src/TrackingService/AppConfigHealthCheck.cs with a class named AppConfigHealthCheck that implements IHealthCheck. The check reads Tracking:PageSize from IConfiguration, returns Healthy when the value is 50, and returns Unhealthy otherwise. Register the check in src/TrackingService/Program.cs under the name app-configuration.

Run dotnet build ContosoLogistics.sln and fix every error. When a package version conflicts with the one the Aspire client package requires, use the required version.

Write no code comments.
```

Expected Outcome: Run `aspire deploy` from the `contoso-logistics/` folder again. It creates the store and redeploys tracking-service. Then grant your sign-in access to the store and set the value. Replace the placeholders with the names your deployment created:

```bash
az appconfig list --resource-group rg-azn-labs --query "[].name" --output tsv
az appconfig show --name <CONFIG_NAME> --resource-group rg-azn-labs --query id --output tsv
az role assignment create --assignee <SIGN_IN_EMAIL> --role "App Configuration Data Owner" --scope <CONFIG_ID>
az appconfig kv set --name <CONFIG_NAME> --key Tracking:PageSize --value 50 --auth-mode login --yes
```

The provider reads the store when the service starts, and the value did not exist then. Role assignments also take a minute or two to propagate. Restart the current revision, then open `https://<TRACKING_SERVICE_FQDN>/health`:

```bash
az containerapp revision restart --name tracking-service --resource-group rg-azn-labs --revision $(az containerapp show --name tracking-service --resource-group rg-azn-labs --query properties.latestRevisionName --output tsv)
```

The JSON lists `app-configuration` with a status of `Healthy`.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - The `app-configuration` entry in the deployed `tracking-service` health report is `Healthy`, which proves `Tracking:PageSize` reads as 50 through the App Configuration provider.

## Step 4: Send Booking Telemetry to Application Insights

Overview: A booking crosses shipping-api, Service Bus and tracking-service. This step adds an Application Insights resource and sends telemetry from both services to it, so Step 6 can follow one booking across those hops. Learn now recommends the Microsoft OpenTelemetry Distro for new .NET apps, and the research below compares it with the Azure Monitor distro that this lab uses.

Research: Ask the agent to compare the two distros before it edits the ServiceDefaults project.

```text
Read src/ServiceDefaults/Extensions.cs and src/ServiceDefaults/ServiceDefaults.csproj in this folder. Compare the Microsoft OpenTelemetry Distro (package Microsoft.OpenTelemetry) with the Azure Monitor OpenTelemetry Distro (package Azure.Monitor.OpenTelemetry.AspNetCore) for these two services. Use the Microsoft Learn MCP server for both packages. Recommend one package with reasoning, and name the call that initializes it. Do not change any file.
```

Finding: A good answer cites the Learn guidance for new apps and for existing Azure Monitor distro users, and it recommends one package. It states that the two distros must not start in the same application. Push back if it proposes both packages, or if it names a package that Learn does not list.

Recipe:

```text
Work in this folder. Read src/AppHost/Program.cs and src/ServiceDefaults/Extensions.cs first.

In src/AppHost/Program.cs, add an Application Insights resource named app-insights with AddAzureApplicationInsights. Reference it from shipping-api and tracking-service with WithReference, so each service receives the connection string in the APPLICATIONINSIGHTS_CONNECTION_STRING variable. Add the Aspire hosting package for Azure Application Insights to src/AppHost/ContosoLogistics.AppHost.csproj.

In src/ServiceDefaults/ServiceDefaults.csproj, add the package Azure.Monitor.OpenTelemetry.AspNetCore. In src/ServiceDefaults/Extensions.cs, call UseAzureMonitor on the OpenTelemetry builder inside AddServiceDefaults, so both services export traces, metrics and logs to Application Insights. Do not add the Microsoft.OpenTelemetry package.

Run dotnet build ContosoLogistics.sln and fix every error. When a package version conflicts with the one the Aspire client package requires, use the required version.

Write no code comments.
```

Expected Outcome: Run `aspire deploy` from the `contoso-logistics/` folder again. It creates the Application Insights resource and redeploys both services. Then run this command, which lists one Application Insights component in the resource group:

```bash
az resource list --resource-group rg-azn-labs --resource-type Microsoft.Insights/components --output table
```

## Step 5: Release tracking-service v2 Behind a Traffic Split

Overview: A new tracking-service release must not reach every parcel at once. This step deploys v2 as a new revision, switches the service to multiple revision mode, and then moves 10 percent of requests to v2.

Recipe:

```text
Work in this folder. Read src/TrackingService/Program.cs first.

In src/TrackingService/Program.cs, change the GET /tracking/{trackingNumber} endpoint so that its JSON response includes the property schemaVersion with the number 2. Leave the rest of the response and the tracking projection unchanged. Write no code comments.
```

Expected Outcome: Run the Recipe above, then run `aspire deploy --non-interactive` from the `contoso-logistics/` folder. A deploy sets the app to single revision mode and sends all traffic to the new revision, so the next commands switch the mode back after it. Revision names are generated, so copy them from the table output.

1. Switch tracking-service to multiple revision mode, then list its revisions. Note the older revision as `<V1_REVISION>` and the newest as `<V2_REVISION>`. Single revision mode deactivated the older revision, so activate it again.

```bash
az containerapp revision set-mode --name tracking-service --resource-group rg-azn-labs --mode multiple
az containerapp revision list --name tracking-service --resource-group rg-azn-labs --output table
az containerapp revision activate --name tracking-service --resource-group rg-azn-labs --revision <V1_REVISION>
```

2. Move 10 percent of traffic to v2.

```bash
az containerapp ingress traffic set --name tracking-service --resource-group rg-azn-labs --revision-weight <V1_REVISION>=90 <V2_REVISION>=10
```

Confirm the mode with the command below. It reports Multiple.

```bash
az containerapp show --name tracking-service --resource-group rg-azn-labs --query properties.configuration.activeRevisionsMode --output tsv
```

The split is weighted per request, so the counts vary between runs. After the booking in Step 6, send 20 GET requests to `https://<TRACKING_SERVICE_FQDN>/tracking/CL-2026-000142`. Some responses include `schemaVersion`, and others do not.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - The `tracking-service` app runs in multiple revision mode with two revisions, the previous one at weight 90 and the v2 revision at weight 10.

## Step 6: Query the Booking Trace with KQL

Overview: The telemetry from Step 4 helps only when a query follows one booking across services. This step sends a booking to the deployed shipping-api, then writes a KQL query that returns every row of that booking's operation.

Recipe:

```text
Work in this folder. Read src/ShippingApi/Program.cs and src/TrackingService/Program.cs first to learn the request names and the Service Bus message flow.

Create docs/booking-trace.kql with one KQL query for Application Insights. The query:
1. Finds the most recent POST request in AppRequests from the shipping-api role within the last 30 minutes and takes its OperationId. AppRoleName carries the environment name as a prefix, for example [env-name]/shipping-api, so match it with endswith.
2. Returns every row in AppRequests, AppDependencies and AppTraces that has that OperationId, in time order.
3. Projects these columns: TimeGenerated, AppRoleName, Type, Name, Target, DurationMs, Success, ResultCode and OperationId. Every table already has a Type column that holds its table name, and AppTraces has no Name column, so project its Message as Name.

Write no code comments.
```

Expected Outcome: Send the booking below with any HTTP client to `https://<SHIPPING_API_FQDN>/shipments`. The response is 201 Created.

```json
{
  "shipmentId": "3f2c9a1e-7b4d-4e8a-9c21-5d6f0b8e4a17",
  "trackingNumber": "CL-2026-000142"
}
```

Give Application Insights a few minutes to ingest the telemetry. Then open the `app-insights` resource in the Azure portal, select Logs, and paste the contents of `docs/booking-trace.kql`. The result lists the shipping-api POST row first, then the rows that shipping-api and tracking-service recorded for the same OperationId, in time order.

> **_NOTE:_** A gate test could prove this step, but this lab skips it to save time. It would check:
>
> - Application Insights holds one booking operation with a `shipping-api` POST request and a `tracking-service` row that share its OperationId.

## Solution

The finished state of this lab is in `labs/03-hosting/contoso-logistics-solution/`.

## Cleanup

Keep the environment for the next lab, which deploys into the same resource group. Run each command in `labs/03-hosting/deprovision.azcli` only if you stop the class before lab 9. It deletes the shared resource group `rg-azn-labs` and every resource in it, including the Container Apps environment, the Key Vault with its secret, and the App Configuration store. The deployed resources bill until you run it.
