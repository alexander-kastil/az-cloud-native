---
slide: "99-01"
topic: 03-hosting
title: "Takeaways"
subtitle: "What to carry into reliable messaging with Service Bus"
layout: content
visual-weight: 1/2
visual-type: photo
media-source: nano-banana
media-file: demos/03-hosting/pptx/images/99-01-takeaways.jpg
visual-prompt: >
  ONE single continuous photograph taken with one camera in one moment: no split screen, no diptych, no
  collage. Four software engineers in their late twenties to mid thirties, two women and two men in
  modern work-casual clothes, relax in a bright lounge at the end of a release day: they sit on a light
  sofa and two armchairs around a low light-oak table with two open laptops and coffee mugs. One laptop
  is turned toward the group and shows softly blurred horizontal coloured bars like a timeline, one
  engineer gives a relaxed thumbs up and the others laugh. The group sits in the centre third of the
  frame, faces clearly visible, tall windows behind them. The mugs and notebooks on the table are plain
  and blank. Modern, light, bright and friendly: high-key natural daylight, white and light-oak
  contemporary interior, soft pastel accents in light blue, mint and lavender, green plants, 2020s
  design. Screens show only soft blurred shapes and colours, never readable UI. 16:9 landscape
  1920x1080, contemporary editorial photography. Avoid: readable text, logos, watermarks, extra people,
  dark or moody lighting, vintage or retro decor, grime, clutter.
order: 16
source: demos/03-hosting/readme.md
gamma_prompt: >
  Closing slide titled "Takeaways", subtitle "What to carry into reliable messaging with Service Bus".
  Side-accent layout: title, subtitle and five bullets on the left half on plain white, the provided
  photograph of a team relaxing in a bright lounge after a release filling the right half, no text over
  it. Use exactly the one image given for this card; do not generate or add any other images, icons or
  logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Takeaways

## Content

- Declare the environment in the AppHost and let `aspire deploy` build the rest
- Read secrets and settings through a **managed identity**, so no password is stored
- Release behind a **traffic split** and keep the previous revision for rollback
- Give consumers a grace period longer than the host shutdown timeout
- Run Dapr actors on AKS, and join one scan's telemetry by `OperationId`

## Speaker Notes

Close with the habits this module built for every service that goes to Azure.

Deploy from the AppHost: the Container Apps environment is a resource you declare, and `aspire deploy` creates the registry, the identity and one container app per service.

Managed identity: `DefaultAzureCredential` runs the same code on a laptop and in Azure, with secrets in Key Vault and settings in App Configuration.

Traffic split: multiple revision mode and weights that add up to 100 let a release start at 10 percent, and the rollback is only a traffic change.

Draining: set `terminationGracePeriodSeconds` longer than `HostOptions.ShutdownTimeout`, and settle every message before the handler returns.

Host and trace: the routing service runs on AKS with the Dapr extension because Container Apps managed Dapr doesn't support the actor SDK package, and `OperationId` joins one scan across the telemetry tables. Module 4 connects the services with Service Bus.
