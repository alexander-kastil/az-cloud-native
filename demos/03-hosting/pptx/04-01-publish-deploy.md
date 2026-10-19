---
slide: "04-01"
topic: 04-aks
title: "Helm charts and the AKS target"
subtitle: "Publish for a pipeline or deploy in one step"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/03-hosting/pptx/images/04-01-publish-deploy.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, ground #F8FAFC as in the module's existing
  diagrams, 'Segoe UI', Inter, Arial, sans-serif, Consolas for commands and folders, text #0F172A and
  secondary #64748B, minimum 26px type. No heading inside the image; the slide title carries it. No
  arrow crosses a box or a label. Two dashed white lanes, one above the other. Top lane: a navy command
  pill "aspire publish -o ./k8s-output" with the note "Helm charts you apply with helm or CI", then four
  cards left to right joined by arrows: blue "Helm chart in the output folder" with "aspire-output by
  default", blue "Review the chart" with "publish doesn't change the cluster", blue "helm or a CI job
  applies the chart", green "Cluster". Bottom lane: a navy pill "aspire deploy" with the note "with the
  AKS target, provisions Azure resources and deploys in a single command", then blue "Bicep provisions
  the AKS cluster", blue "Deploys the Helm charts" and green "AKS cluster" in the same column as
  "Cluster".
order: 10
source: demos/03-hosting/04-aks/readme.md
gamma_prompt: >
  Slide titled "Helm charts and the AKS target", subtitle "Publish for a pipeline or deploy in one
  step". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the provided
  diagram as an inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never
  cropped, never used as a background or side accent. Use exactly the one image given for this card; do
  not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Helm charts and the AKS target

## Content

- `aspire publish` writes a **Helm chart** to `aspire-output`; `-o` changes the folder
- A pipeline applies the chart with `helm` or a CI job
- Publishing leaves the cluster unchanged, so you can review the chart first
- The **AKS target** adds Bicep: `aspire deploy` provisions and deploys in one command
- The Aspire CLI reference labels both commands as preview

## Speaker Notes

The routing service keeps one Dapr actor per van, and Container Apps managed Dapr doesn't support the actor SDK package. Aspire generates its Kubernetes deployment from the same AppHost, so the service graph stays in one place.

Helm charts: for the Kubernetes target, `aspire publish` writes a Helm chart to `aspire-output` beside the AppHost, and the `-o` option changes the folder.

Applying the chart: a pipeline applies the chart with `helm` or a CI job. For the plain Kubernetes target, `aspire deploy` runs `helm install` against the current `kubectl` context.

Review first: the publish step doesn't change the cluster, so you can review the chart before it reaches an environment. Learn's Orleans Kubernetes guide describes plain YAML files in the same output, so open the folder before you apply anything.

The AKS target: `aspire publish` produces Helm charts and Bicep infrastructure, and `aspire deploy` provisions the Azure resources and deploys in a single command.

Preview status: the Aspire CLI reference labels both `aspire publish` and `aspire deploy` as preview.
