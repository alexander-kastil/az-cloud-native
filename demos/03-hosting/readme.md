# Module 3: Hosting on Container Apps & AKS

Modules 1 and 2 designed the Contoso Logistics services and modeled them in an Aspire AppHost. This module puts them on Azure. Most services run on Azure Container Apps, the managed host for this class, and the routing service runs on AKS because it depends on Dapr actors.

You learn how `aspire deploy` provisions the registry, the environment, and the apps from the AppHost. You also learn how secrets and settings reach the services through Key Vault and App Configuration, how revisions shift traffic and drain message consumers, and how Application Insights follows a parcel scan from the HTTP request to the Service Bus handler.

After this module, you can deploy the Contoso Logistics services to Container Apps, release a new version behind a traffic split, and choose the host that a workload needs.

## Topics

| Topic | Description |
| --- | --- |
| [Aspire Deploy: ACR & Container Apps](./01-aspire-deploy/readme.md) | `AddAzureContainerAppEnvironment`, `aspire deploy`, `WithExternalHttpEndpoints`, and `az acr build` |
| [Key Vault & App Configuration](./02-key-vault-app-config/readme.md) | `DefaultAzureCredential`, `SecretClient`, `AddAzureKeyVault`, `AddAzureAppConfiguration`, and `UseFeatureFlags` |
| [Revisions: Traffic Splitting & Draining](./03-revisions/readme.md) | Revision modes, `az containerapp ingress traffic set`, `terminationGracePeriodSeconds`, and `CloseAsync` |
| [AKS: Aspire-Generated Kubernetes Manifests](./04-aks/readme.md) | `AddAzureKubernetesEnvironment`, `aspire publish` with Helm charts, and the Dapr extension for AKS |
| [Application Insights: Traces & KQL](./05-app-insights/readme.md) | `UseAzureMonitor`, the managed OpenTelemetry agent, and KQL over `AppRequests` and `AppDependencies` |

## Helpful Claude Slash Commands

| Command | What it does here |
| --- | --- |
| `/doctor` | Checks the Claude Code installation and its MCP servers before a session runs `az` and `aspire deploy` against your subscription. |
| `/review` | Reviews the AppHost and deployment changes on the current branch before you deploy them. |
| `/security-review` | Audits Key Vault access, role assignments, and managed identity changes on the branch. |
| `/cost` | Shows the session cost when a deployment loop runs many `az` and `aspire` commands. |

## Key Topics covered in this module

- [Deploy to Azure Container Apps with Aspire](https://aspire.dev/deployment/azure/container-apps/)
- [Update and deploy changes in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/revisions)
- [Azure Key Vault developer's guide](https://learn.microsoft.com/azure/key-vault/general/developers-guide)
- [Dapr extension for Azure Kubernetes Service](https://learn.microsoft.com/azure/aks/dapr-overview)
- [Enable OpenTelemetry for .NET applications](https://learn.microsoft.com/azure/azure-monitor/app/opentelemetry-enable)
