---
slide: "02-03"
topic: 02-key-vault-app-config
title: "App Configuration and feature flags"
subtitle: "Settings load at startup, flags refresh on their own"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/03-hosting/_images/app-config-startup.svg"
media-file: labs/03-hosting/_images/app-config-startup.svg
visual-prompt: >
  Existing diagram of tracking-service loading Tracking:* from the App Configuration store at startup, and a revision restart that loads a new key, no generation. Referenced at its repo path.
order: 7
source: demos/03-hosting/02-key-vault-app-config/readme.md
gamma_prompt: >
  Slide titled "App Configuration and feature flags", subtitle "Settings load at startup, flags refresh
  on their own". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# App Configuration and feature flags

## Content

- The .NET provider loads key-values and feature flags into `IConfiguration`
- A **Key Vault reference** points to a secret that stays in the vault
- The AppHost declares the store in publish mode, `WithReference` grants a role
- Keys load at startup, so a new key needs a **revision restart**
- `UseFeatureFlags` and `IFeatureManager`, refreshed every 30 seconds

## Speaker Notes

Settings that change between releases belong in App Configuration, not in a file that ships with the build.

The provider: App Configuration stores key-values and feature flags, and the .NET provider loads them into `IConfiguration`. The tracking service selects its keys with `Select("Tracking:*")`.

Key Vault references: a key in App Configuration can point to a secret in Key Vault, and the app reads it like any other key. The provider resolves it with its own credential when you call `ConfigureKeyVault`, so App Configuration never holds the secret value.

Aspire wiring: the AppHost declares the store only in publish mode and never calls `RunAsEmulator`, so `aspire deploy` provisions it in Azure. The service references the store, and Aspire grants it a role on the store.

Startup load: the provider loads key-values when the service starts, so a key added later needs a revision restart, as the diagram shows for `Tracking:PageSize`.

Feature flags: flags are key-values with the `.appconfig.featureflag/` prefix, loaded by `UseFeatureFlags` and checked through `IFeatureManager`. The provider refreshes them on its own, every 30 seconds by default.
