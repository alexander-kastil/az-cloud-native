---
slide: "02-01"
topic: 02-key-vault-app-config
title: "Key Vault access with a managed identity"
subtitle: "The same code runs on a laptop and in Azure"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/03-hosting/_images/carrier-key-health-check.svg"
media-file: labs/03-hosting/_images/carrier-key-health-check.svg
visual-prompt: >
  Existing diagram of shipping-api reading carrier-api-key from key-vault through SecretClient as its managed identity on every /health call, no generation. Referenced at its repo path.
order: 5
source: demos/03-hosting/02-key-vault-app-config/readme.md
gamma_prompt: >
  Slide titled "Key Vault access with a managed identity", subtitle "The same code runs on a laptop and
  in Azure". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Key Vault access with a managed identity

## Content

- `DefaultAzureCredential` uses the Azure CLI sign-in locally and the **managed identity** in Azure
- `SecretClient` reads secrets with that credential, so no password is stored
- Read access comes from the **Key Vault Secrets User** role, scoped to the vault
- `AddAzureKeyVault` and `WithReference` let Aspire assign the role
- The default is Key Vault Administrator; `WithRoleAssignments` narrows it

## Speaker Notes

Carrier API keys and connection details must not sit in `appsettings.json` or in plain environment variables. Key Vault holds them, and a managed identity removes the password.

One credential type: `DefaultAzureCredential` decides at runtime, using the Azure CLI sign-in on a developer machine and the app's managed identity in Azure. The same code runs in both places without environment checks.

The client: `SecretClient` takes the vault URI and that credential. The diagram shows the lab's health check reading `carrier-api-key` this way on every `/health` call.

The role: grant the identity Key Vault Secrets User with `az role assignment create`, scoped to the vault's resource ID.

Aspire wiring: the AppHost declares the vault with `AddAzureKeyVault` and references it from the service, and Aspire assigns roles on the vault. The AppHost needs the `Aspire.Hosting.Azure.KeyVault` package.

Least privilege: by default the consuming project gets Key Vault Administrator. For read-only access, call `WithRoleAssignments` with `KeyVaultBuiltInRole.KeyVaultSecretsUser`, as the Aspire example does.
