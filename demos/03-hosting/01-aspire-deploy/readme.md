# Aspire Deploy: ACR & Container Apps

> **_NOTE:_** Duration: about 7 minutes.

Contoso Logistics has nine components. Each one needs a container image, a registry to hold it, an environment to run in, and an identity that lets it pull the image. Building that by hand for every service makes the first deploy slow and every later change risky.

This topic shows how the AppHost declares the Azure Container Apps environment as a resource and how `aspire deploy` provisions the registry, the environment, and the apps. It then covers how ingress decides which services other apps can call, and it ends with ACR Tasks for image builds in a CI pipeline.

## Declare the environment and deploy it

The environment is a resource in the AppHost. Aspire 9.4 removed the hybrid mode in which azd created the environment, so an app that targets Container Apps must declare the environment explicitly. Without that call, the deployment has no target for the containers.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment("aca-env");

var routing = builder.AddProject<Projects.RoutingService>("routing-service");

builder.AddProject<Projects.ShippingApi>("shipping-api")
    .WithExternalHttpEndpoints()
    .WithReference(routing);

builder.Build().Run();
```

The environment includes the Aspire dashboard by default. Chain `WithDashboard(false)` on the environment call to omit it. The dashboard can't use a custom domain, so it runs on the default Container Apps domain name.

Run `az login` once, then run `aspire deploy` from the AppHost directory. The first run prompts for a sign-in, a subscription, a resource group, and a location. The command provisions the infrastructure, builds and pushes the images, deploys the apps, and prints the dashboard URL.

```bash
az login
aspire deploy
```

The deployment creates these resources.

| Resource | Purpose |
| --- | --- |
| Container Apps environment | Hosts the apps and includes the Aspire dashboard by default |
| Azure Container Registry | Stores the images, either provisioned for the environment or attached with `WithAzureContainerRegistry` |
| User-assigned managed identity | Gives the apps an identity that can pull images from the registry |
| Image pull permission | Lets the apps pull images without registry credentials in the deployment |
| Container app per compute resource | Runs each service from the AppHost |
| Azure Files shares | Back the volumes and bind mounts |

Because the deployment creates the registry and the identity, you don't pass registry credentials. Run `aspire destroy --yes` when you finish, because it deletes the entire resource group. The Aspire CLI reference labels `aspire deploy` as preview, so confirm its current status before you depend on it in a production pipeline.

## Ingress and calls by app name

Ingress decides whether a service receives traffic from outside its environment. `WithExternalHttpEndpoints` publishes an endpoint. A service without that call stays internal, so only apps in the same environment can reach it.

Use the external call only for endpoints that clients outside the environment must reach.

Each app gets one HTTP ingress. Other apps in the environment call it by app name, so `shipping-api` reaches `routing-service` at `http://routing-service`, and the traffic never leaves the environment.

```csharp
var response = await httpClient.GetAsync("http://routing-service/");
response.EnsureSuccessStatusCode();
```

Container Apps app names must be 2 to 32 characters long. They use lowercase letters, numbers, and hyphens, start with a letter, and end with an alphanumeric character. The names `routing-service` and `shipping-api` meet those rules.

## Build images with ACR Tasks

`aspire deploy` builds the images on your machine, so Docker Desktop or Podman must be running. A CI job can skip the local container runtime by building in the registry with ACR Tasks. The `az acr build` command builds the image in the registry without pulling or pushing it locally.

```bash
az acr build --registry <REGISTRY_NAME> --image routing-service:<TAG> --file Dockerfile .
```

Registry names must be unique in Azure and contain 5 to 50 lowercase alphanumeric characters. After the build, point the app at the new image with `az containerapp update --image`, which creates a new revision. The revisions topic explains how traffic moves during that change.

## Key Topics

- [Deploy to Azure Container Apps with Aspire](https://aspire.dev/deployment/azure/container-apps/)
- [Aspire deployment overview](https://aspire.dev/deployment/deploy-with-aspire/)
- [Communicate between container apps in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/connect-apps)
- [Ingress in Azure Container Apps](https://learn.microsoft.com/azure/container-apps/ingress-overview)
- [az acr build](https://learn.microsoft.com/cli/azure/acr#az-acr-build)
