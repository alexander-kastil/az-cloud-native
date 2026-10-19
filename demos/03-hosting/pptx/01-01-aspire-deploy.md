---
slide: "01-01"
topic: 01-aspire-deploy
title: "Deploying with aspire deploy"
subtitle: "The AppHost declares the environment, one command provisions the rest"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/03-hosting/01-aspire-deploy/_images/aspire-deploy-resources.svg"
media-file: demos/03-hosting/01-aspire-deploy/_images/aspire-deploy-resources.svg
visual-prompt: >
  Existing diagram of what aspire deploy creates in the resource group and how the container apps pull their images as the managed identity, no generation. Referenced at its repo path.
order: 3
source: demos/03-hosting/01-aspire-deploy/readme.md
gamma_prompt: >
  Slide titled "Deploying with aspire deploy", subtitle "The AppHost declares the environment, one
  command provisions the rest". Layout: Two columns. Left column: the bullets. Right column, wider than
  the left: the provided diagram as an inline content image at its natural wide aspect ratio, scaled to
  fit the column, never cropped, never used as a background or side accent. Use exactly the one image
  given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Deploying with aspire deploy

## Content

- `AddAzureContainerAppEnvironment` declares the environment in the AppHost
- `aspire deploy` provisions, builds and pushes the images, then deploys the apps
- The **managed identity** pulls the images, so you pass no registry credentials
- A CI job can build in the registry with `az acr build`
- `aspire destroy --yes` deletes the whole resource group

## Speaker Notes

Grey Logistics has nine components, and each one needs an image, a registry, an environment and an identity that can pull the image. This slide shows how the AppHost and one command create all of that.

The environment as a resource: since Aspire 9.4 removed the hybrid mode, an app that targets Container Apps must declare the environment with `AddAzureContainerAppEnvironment`. The AppHost needs the `Aspire.Hosting.Azure.AppContainers` package for that call.

One command: after `az login`, `aspire deploy` provisions the infrastructure, builds and pushes the images, deploys the apps and prints the dashboard URL. With `Azure__SubscriptionId`, `Azure__Location` and `Azure__ResourceGroup` set, `--non-interactive` skips the prompts.

Image pull: the deployment creates the registry and a user-assigned managed identity with the image pull permission, so no registry credentials go into the deployment. Azure Files shares are created only when the model declares a volume or bind mount.

Builds in the registry: `aspire deploy` builds on your machine with Docker Desktop or Podman, and a CI job can skip the local runtime with `az acr build`. `az containerapp update --image` then points the app at the new image and creates a new revision.

Cleanup: `aspire destroy --yes` deletes the entire resource group. The Aspire CLI reference labels `aspire deploy` as preview, so confirm its status before a production pipeline depends on it.
