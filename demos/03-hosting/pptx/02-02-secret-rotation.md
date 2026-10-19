---
slide: "02-02"
topic: 02-key-vault-app-config
title: "Secret versions and rotation"
subtitle: "Key Vault events drive the rotation"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: opus-svg
media-file: demos/03-hosting/pptx/images/02-02-secret-rotation.svg
visual-prompt: >
  Opus 5.5, hand-written SVG, viewBox 0 0 1920 1080, ground #F8FAFC as in the module's existing
  diagrams, 'Segoe UI', Inter, Arial, sans-serif, Consolas for event names and code, text #0F172A and
  secondary #64748B, minimum 26px type. No heading inside the image; the slide title carries it. No
  arrow crosses a box or a label. A sequence diagram with four header boxes and dashed lifelines: green
  "Key Vault", purple "Event Grid", amber "rotation function", blue "Container Apps revision". A grey
  note on the Key Vault lifeline: "30 days before the expiration date". Six numbered messages, each with
  a blue circle badge in a left column: 1 Key Vault to Event Grid "Microsoft.KeyVault.SecretNearExpiry";
  2 Event Grid to rotation function "deliver the event"; 3 rotation function to Key Vault
  "SetSecretAsync writes a new version of carrier-api-key"; 4 Key Vault to Event Grid
  "Microsoft.KeyVault.SecretNewVersionCreated"; 5 rotation function to Container Apps revision "restart
  the revision"; 6 Container Apps revision to Key Vault "the container reads the new version". Every
  label sits in the gap between two lifelines.
order: 6
source: demos/03-hosting/02-key-vault-app-config/readme.md
gamma_prompt: >
  Slide titled "Secret versions and rotation", subtitle "Key Vault events drive the rotation". Layout:
  Two columns. Left column: the bullets. Right column, wider than the left: the provided diagram as an
  inline content image at its natural 16:9 aspect ratio, scaled to fit the column, never cropped, never
  used as a background or side accent. Use exactly the one image given for this card; do not generate
  or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Secret versions and rotation

## Content

- Each write creates a new **secret version**: `SetSecretAsync`
- Key Vault raises `SecretNearExpiry` 30 days before the expiration date
- Event Grid delivers it to a rotation function that writes the new version
- Restart the Container Apps revision so the container reads it
- Prefer managed identity; rotate only where a key is still needed

## Speaker Notes

Some workloads still need a password or a key, such as the carrier API key. For those, Key Vault versions and events drive the rotation.

Versions: each write to a secret creates a new version, so `SetSecretAsync` on `carrier-api-key` stores the rotated key as a new version of the secret that the shipping API reads.

Events: Key Vault raises `SecretNearExpiry` 30 days before the expiration date, `SecretNewVersionCreated` when a new version is written, and `SecretExpired` when the current version has expired.

The rotation function: the Learn tutorials route `SecretNearExpiry` through Event Grid to a function that writes a new version and updates the consumer.

Restart: Container Apps secret values need a revision restart before a container sees them, so plan rotations that restart the affected revision.

Managed identity first: prefer managed identity over shared secrets, and use rotation only where a workload still needs a password or a key.
