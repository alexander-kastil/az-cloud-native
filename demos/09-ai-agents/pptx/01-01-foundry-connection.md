---
slide: "01-01"
topic: 01-extensions-ai
title: "Connecting to a Foundry model deployment"
subtitle: "IChatClient, DefaultAzureCredential and the resource endpoint"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/09-ai-agents/pptx/images/01-01-foundry-connection.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, white background, Segoe UI, Inter, Arial, sans-
  serif, Consolas for the endpoint URL, module palette, minimum 27px type. No heading inside the image.
  No arrow crosses a box or a label. Middle row left to right: blue #DBEAFE box "exception-agent",
  secondary "depends on IChatClient", with white chips "GetResponseAsync" and
  "GetStreamingResponseAsync"; purple #EDE9FE box "AddChatClient", secondary "registered once in DI",
  with chips "AzureOpenAIClient", "GetChatClient(deployment)", "AsIChatClient()"; blue box "Foundry
  resource", secondary "resource endpoint", the URL https://<resource>.openai.azure.com/ and a green
  #DCFCE7 chip "chat deployment" "a name you choose". Grey #F1F5F9 box above the middle "AppHost
  settings" "Foundry:Endpoint, Foundry:ChatDeployment" with an arrow down into AddChatClient. Amber
  #FEF3C7 box below the middle "DefaultAzureCredential" "local: your Azure CLI sign-in" "in Azure: the
  managed identity" "Entra ID instead of a key" with an arrow up into AddChatClient and an elbow arrow to
  the Foundry resource labelled "role on the identity" "Cognitive Services OpenAI User".
order: 3
source: demos/09-ai-agents/01-extensions-ai/readme.md
gamma_prompt: >
  Slide titled "Connecting to a Foundry model deployment", subtitle "IChatClient, DefaultAzureCredential
  and the resource endpoint". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Connecting to a Foundry model deployment

## Content

- **IChatClient** offers GetResponseAsync and GetStreamingResponseAsync, so the agent needs no vendor client
- `AzureOpenAIClient` takes the resource endpoint and `DefaultAzureCredential`, then `AsIChatClient`
- The identity holds **Cognitive Services OpenAI User**: Azure CLI sign-in locally, managed identity in Azure
- `AddChatClient` registers the pipeline once; the project endpoint is for Agent Framework clients
- `Aspire.Hosting.Foundry` (13.6.0-preview) adds `AddFoundry`; the lab passes `Foundry:Endpoint` and `Foundry:ChatDeployment`

## Speaker Notes

Topic 01 starts with the client the exception-agent uses to reach its model. The model runs as a Microsoft Foundry deployment, but the service should not depend on the vendor SDK underneath.

IChatClient: the contract between a service and a chat model, with GetResponseAsync for a complete reply and GetStreamingResponseAsync for incremental updates. Any library that implements it plugs into the same consumer code.

Resource endpoint: AzureOpenAIClient takes the resource endpoint and DefaultAzureCredential, GetChatClient picks the deployment by the name you gave it, and AsIChatClient adapts it. The project endpoint with the api/projects path belongs to the Agent Framework and Foundry Agent Service clients.

Identity and role: assign Cognitive Services OpenAI User to the identity that runs the service. DefaultAzureCredential uses your Azure CLI sign-in locally and the managed identity in Azure, and Entra ID avoids keys that grant every caller the same rights and need manual rotation.

Registration and Aspire: AddChatClient registers the pipeline once in dependency injection, so every consumer resolves the same one. Aspire.Hosting.Foundry with AddFoundry is a 13.6.0 preview, and the lab's AppHost passes Foundry:Endpoint and Foundry:ChatDeployment as settings instead.
