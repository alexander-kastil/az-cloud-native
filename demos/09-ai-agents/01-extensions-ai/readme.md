# Microsoft.Extensions.AI: Foundry Model Client

> **_NOTE:_** Duration: about 9 minutes.

Contoso Logistics' exception-agent needs a language model to read a delayed parcel, draft a customer notice, and explain why a reroute makes sense. The model runs as a Microsoft Foundry deployment, but the service that calls it should not depend on the vendor SDK underneath. Microsoft.Extensions.AI gives the service one chat abstraction and middleware that wraps every call.

This topic covers the IChatClient interface, how a .NET service connects to a Foundry model deployment with Microsoft Entra ID, how logging and OpenTelemetry middleware observe each call, and what the Aspire integration for Foundry offers. The Aspire package is a preview, and this topic marks it as one.

## IChatClient: One Abstraction for Every Model

IChatClient is the contract between a service and a chat model. It is defined in the Microsoft.Extensions.AI.Abstractions package and exposes GetResponseAsync for a complete reply and GetStreamingResponseAsync for incremental updates. Any library that implements the interface plugs into the same consumer code, so the exception-agent depends on IChatClient and not on a vendor client.

Your code sends a list of messages, and each message has a role. The response carries the generated messages, and you add them to the history when the conversation continues.

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

IChatClient client = new AzureOpenAIClient(
        new Uri("https://<resource>.openai.azure.com/"),
        new DefaultAzureCredential())
    .GetChatClient("<chat-deployment>")
    .AsIChatClient();

Console.WriteLine(await client.GetResponseAsync(
[
    new(ChatRole.System, "You draft short delivery exception notices."),
    new(ChatRole.User, "Parcel 4471 missed its delivery window."),
]));
```

Microsoft.Extensions.AI references the abstractions implicitly, so a service usually references that package and one provider adapter.

| Package | Reference it when |
| --- | --- |
| Microsoft.Extensions.AI.Abstractions | You write a library that implements IChatClient |
| Microsoft.Extensions.AI | Your service builds pipelines with Use methods and helpers |
| Microsoft.Extensions.AI.OpenAI | You adapt an OpenAI or Azure OpenAI client with AsIChatClient |

```bash
dotnet add package Microsoft.Extensions.AI
dotnet add package Microsoft.Extensions.AI.OpenAI
dotnet add package Azure.AI.OpenAI
dotnet add package Azure.Identity
```

## Connecting to a Foundry Model Deployment

A Foundry model deployment has a name that you choose when you deploy the model. The service addresses it through the resource endpoint and the deployment name, and it authenticates with Microsoft Entra ID through DefaultAzureCredential. Microsoft recommends Entra ID over keys, because keys grant the same rights to every caller and need manual rotation.

Assign the Cognitive Services OpenAI User role (role ID 5e0bd9bd-7b93-4f28-af87-19fc36ad61bd) to the identity that runs the service. For local development, DefaultAzureCredential picks up your Azure CLI sign-in. In Azure, it uses the managed identity assigned to the app.

Register the client once in the dependency injection container, so every consumer resolves the same pipeline.

```csharp
using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Extensions.AI;

builder.Services.AddChatClient(services =>
    new AzureOpenAIClient(
            new Uri(builder.Configuration["Foundry:Endpoint"]!),
            new DefaultAzureCredential())
        .GetChatClient(builder.Configuration["Foundry:ChatDeployment"]!)
        .AsIChatClient()
        .AsBuilder()
        .UseLogging()
        .UseOpenTelemetry()
        .Build(services));
```

Use the resource endpoint with AzureOpenAIClient. The project endpoint, which has the form of a services.ai.azure.com URL with an api/projects path, belongs to the Agent Framework and Foundry Agent Service clients.

## Middleware: Logging and OpenTelemetry on Every Call

Each Use method wraps the client built before it, so the chain behaves like a stack of decorators. UseLogging writes an entry for each request and response through ILogger. UseOpenTelemetry emits spans and metrics that follow the OpenTelemetry semantic conventions for generative AI, so the model spans sit beside the HTTP and messaging spans of the same operation.

The generative AI conventions are experimental, and Learn states that the telemetry output can change. Pin the package version in production and check the span names after each upgrade. Pass a source name so you can filter the spans, and add that name to your tracer configuration.

Other middleware follows the same pattern. UseDistributedCache stores complete responses keyed by the message history, so a repeated prompt skips the model call. Custom middleware derives from DelegatingChatClient and overrides only the methods it needs, which keeps rate limiting and redaction out of the business code.

## Aspire Integration for Foundry (Preview)

Aspire models the Foundry resources in the AppHost, next to the services that call them. The hosting package is Aspire.Hosting.Foundry, and it replaces Aspire.Hosting.Azure.AIFoundry from earlier previews.

The AddAzureAIFoundry call became AddFoundry. The current build is 13.6.0-preview, so treat the integration as a preview.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var foundry = builder.AddFoundry("foundry");
var chat = foundry.AddDeployment("chat", FoundryModel.OpenAI.Gpt5Mini);

builder.Build().Run();
```

AddProject adds a Foundry project to the same resource. The aspire.dev page does not label the whole integration as generally available, and it notes that toolbox availability depends on the Foundry preview and on regional support. Expect the API names to change between Aspire releases.

The client wiring differs from the hosting side. The Aspire connection page documents Aspire.OpenAI with AddOpenAIClient and AddChatClient for Foundry Local, so confirm the client call for a cloud deployment against the current page before you copy it.

## Key Topics

- [Use the IChatClient interface](https://learn.microsoft.com/dotnet/ai/ichatclient)
- [Microsoft.Extensions.AI libraries](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)
- [Foundry tools authentication and authorization using .NET](https://learn.microsoft.com/dotnet/ai/azure-ai-services-authentication)
- [Aspire Microsoft Foundry hosting integration](https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-host/)
- [What is Microsoft Foundry?](https://learn.microsoft.com/azure/foundry/what-is-foundry)
