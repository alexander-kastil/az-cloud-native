---
slide: "03-01"
topic: 03-revisions
title: "Revision modes and traffic splitting"
subtitle: "Weighted traffic per revision and rollback"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "demos/03-hosting/03-revisions/_images/traffic-shifting.svg"
media-file: demos/03-hosting/03-revisions/_images/traffic-shifting.svg
visual-prompt: >
  Existing diagram of requests split between the old and the new revision at weights 100/0, 90/10, 50/50 and 0/100, with the old revision kept for rollback, no generation. Referenced at its repo path.
order: 8
source: demos/03-hosting/03-revisions/readme.md
gamma_prompt: >
  Slide titled "Revision modes and traffic splitting", subtitle "Weighted traffic per revision and
  rollback". Layout: Two columns. Left column: the bullets. Right column, wider than the left: the
  provided diagram as an inline content image at its natural wide aspect ratio, scaled to fit the
  column, never cropped, never used as a background or side accent. Use exactly the one image given for
  this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Revision modes and traffic splitting

## Content

- **Single** mode is the default: the new revision takes over once ready
- **Multiple** mode keeps several revisions active for blue-green and canary
- `az containerapp ingress traffic set` sets weights that add up to 100
- Roll back with the same command and the previous revision at 100
- **Labels** (preview) give a revision a stable URL

## Speaker Notes

A new tracking-service release must not reach every parcel at once. Container Apps keeps each version of an app as a revision, and you decide how much traffic each revision receives.

Single mode: the default, where the new revision takes over after its replicas pass their probes and the old revision deactivates on its own. Most services in the class use it.

Multiple mode: several revisions stay active and you set the weight of each, for blue-green and canary releases. A later `aspire deploy` returns the app to single mode, so run `az containerapp revision set-mode` again after each deploy.

Weights: they must add up to 100, set by revision name or by label. The readme command sends 90 percent to the current revision and 10 percent to the new one.

Rollback: the same command with the previous revision at 100. Keep the previous revision active until the new one proves itself, and the rollback needs only a traffic change.

Labels: a preview mode in which named revisions such as `dev`, `staging` and `prod` each get a stable URL. Moving a label to another revision promotes it and keeps the URL, which suits a canary group.
