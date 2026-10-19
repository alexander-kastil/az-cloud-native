---
slide: "04-02"
topic: 04-aks
title: "AKS environment and the Dapr extension"
subtitle: "A host where you control the Dapr extension"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/03-hosting/pptx/images/04-02-aks-dapr.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, ground #F8FAFC as in the module's existing
  diagrams, 'Segoe UI', Inter, Arial, sans-serif, Consolas for code and commands, text #0F172A and
  secondary #64748B, minimum 26px type. No heading inside the image; the slide title carries it. No
  arrow crosses a box or a label. Left top: a white "AppHost" box, "Aspire.Hosting.Azure.Kubernetes",
  holding a navy code panel with the readme calls AddAzureKubernetesEnvironment("aks"),
  .WithSystemNodePool(), aks.AddNodePool("routing", vmSize: "Standard_D2s_v5", minCount: 1, maxCount:
  3) and AddProject<Projects.RoutingService>("routing-service"). An arrow labelled "aspire deploy" leads
  to a white container "aks, AKS cluster" with a #1D4ED8 border, holding a blue card "system node pool",
  "WithSystemNodePool()", "default VM size and node counts", a blue card "node pool routing",
  "Standard_D2s_v5", "minCount 1, maxCount 3" with a white inner card "routing-service", "one Dapr
  actor per van", and below them a green bar "Dapr runtime", "installed as a managed cluster extension",
  "--configuration-settings for options such as high availability". Left bottom: a navy command box "az
  k8s-extension create --cluster-type managedClusters --name dapr --extension-type Microsoft.Dapr" with
  an arrow labelled "installs" into the Dapr bar.
order: 11
source: demos/03-hosting/04-aks/readme.md
gamma_prompt: >
  Slide titled "AKS environment and the Dapr extension", subtitle "A host where you control the Dapr
  extension". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# AKS environment and the Dapr extension

## Content

- `AddAzureKubernetesEnvironment` declares the cluster as a compute environment
- `WithSystemNodePool` and `AddNodePool("routing", ...)` set the node pools
- `WithContainerRegistry`, `WithWorkloadIdentity` and `WithSubnet` fit existing resources
- The **Dapr extension** installs the runtime as a managed cluster extension
- Ingress and Gateway API on AKS are preview in the Aspire documentation

## Speaker Notes

This slide shows how the AppHost declares the AKS cluster and how the Dapr runtime gets onto it.

The environment: `AddAzureKubernetesEnvironment` from the `Aspire.Hosting.Azure.Kubernetes` package declares the cluster, and `aspire deploy` creates it the first time it runs. The cluster bills until you delete its resource group.

Node pools: `WithSystemNodePool` uses the default VM size and node counts, and `AddNodePool` adds a second pool named `routing` with `Standard_D2s_v5` nodes for the routing workload.

Existing resources: `WithContainerRegistry` attaches the registry that holds the images, `WithWorkloadIdentity` turns workload identity on or off, and `WithSubnet` joins a subnet you already defined.

The Dapr extension: `az k8s-extension create` with `--extension-type Microsoft.Dapr` installs the runtime as a managed cluster extension, so you don't install or upgrade it yourself. It accepts `--configuration-settings` for Dapr options such as high availability.

Ingress: ingress and Gateway API on AKS are preview features, so check the Aspire reference for the current API before routing-service depends on them. Module 7 compares the two hosts for actors.
