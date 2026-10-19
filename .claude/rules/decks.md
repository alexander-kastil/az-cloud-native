---
paths:
  - "demos/*/pptx/**"
  - "demos/pptx/**"
---

# Decks

Facts for the global create-pptx skill (`references/gamma-deck.md`) and the `/create-repo-pptx` command. The json block is the input of `scripts/repo_decks.py`; keep it in step with the lists below.

```json
{
  "modules": "demos/*/pptx",
  "portraits": "demos/pptx/portraits.json",
  "footer": "Event-Driven Distributed Apps on Azure: Aspire, Dapr & Durable Execution",
  "gammaTemplate": "g_dnrfhnxxzj9ligv",
  "rawBase": "https://raw.githubusercontent.com/alexander-kastil/az-cloud-native/master",
  "deliverTo": "E:/Onedrive/OneDrive - Integrations IT Solutions/Classes/az-native"
}
```

## Photos

- Audience: Azure developers building distributed and event-driven applications, and software architects designing microservices on Azure.
- Business case: Grey Logistics, a last-mile parcel delivery company: shipments are booked, vans report their position, scanners report every parcel, and an operations console shows the network live. People, roles, documents and sites come from the module and topic readmes, nothing else.
- Module-slide portraits: `demos/pptx/portraits.json`.

## Build

- Footer: `Event-Driven Distributed Apps on Azure: Aspire, Dapr & Durable Execution` (the class readme title, `demos/readme.md`).
- Gamma template: Professsional Process Review, `g_dnrfhnxxzj9ligv`. Template calls pass no `themeId` and no `imageOptions`.
- Raw image base: `https://raw.githubusercontent.com/alexander-kastil/az-cloud-native/master`. The remote `arambazamba/az-native` resolves to `alexander-kastil/az-cloud-native`; raw URLs use the resolved name. The repo is private, so Gamma reaches the images only while it is public.

## Delivery

- Class OneDrive folder: `E:\Onedrive\OneDrive - Integrations IT Solutions\Classes\az-native`. It already holds the older decks; their file names do not collide with the new ones.
- The OneDrive copy is the owner's working copy: `cmp` before overwriting a deck already there.
