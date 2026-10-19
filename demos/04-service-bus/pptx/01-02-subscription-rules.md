---
slide: "01-02"
topic: 01-subscriptions-filters
title: "Subscription rules in code and Bicep"
subtitle: "Correlation filters on Subject for every Grey Logistics subscription"
layout: diagram
visual-weight: 1/2
visual-type: svg
media-source: existing
photo-link: "labs/04-service-bus/_images/subject-filters.svg"
media-file: labs/04-service-bus/_images/subject-filters.svg
visual-prompt: >
  Existing diagram of the AppHost subscription rules: one CorrelationFilter rule on Subject per event, named after its subject, with the two DeliveryDelayed rules kept apart by ConfigureInfrastructure, no generation. Referenced at its repo path.
order: 4
source: demos/04-service-bus/01-subscriptions-filters/readme.md
gamma_prompt: >
  Slide titled "Subscription rules in code and Bicep", subtitle "Correlation filters on Subject for
  every Grey Logistics subscription". Layout: Two columns. Left column: the bullets. Right column,
  wider than the left: the provided diagram as an inline content image at its natural 16:9 aspect
  ratio, scaled to fit the column, never cropped, never used as a background or side accent. Use
  exactly the one image given for this card; do not generate or add any other images, icons or logos.
  Theme: Integrations House (themeId idege54tfobdv2k), Montserrat bold headings, Poppins body, #1B365D accent.
---

# Subscription rules in code and Bicep

## Content

- **Correlation filters** match fixed properties exactly; Microsoft Learn recommends them whenever they fit
- SQL filters add `LIKE`, `IS NULL` and `EXISTS` over properties, never the message body
- A rule action runs `SET` or `REMOVE` on the copy the subscription receives
- `ServiceBusAdministrationClient.CreateRuleAsync` creates rules in code
- The AppHost declares `AzureServiceBusRule`, and Aspire generates the Bicep

## Speaker Notes

The previous slide showed what each subscription selects. This one shows the filter types behind those rules and how Grey Logistics declares them, one correlation rule per subject.

Correlation filters: they compare properties such as `Subject`, `CorrelationId` or user properties for equality, and the comparison is case-sensitive. Microsoft Learn recommends them whenever they fit, because SQL filters reduce throughput.

SQL filters: choose them only when a rule needs an operator such as `LIKE` or `IS NULL`. System properties need the `sys.` prefix, so `sys.Label` addresses `Subject`, and every value a rule needs must sit in a property.

Rule actions: an action runs after the filter matches and uses `SET` and `REMOVE` statements. It changes only the copy the subscription receives, and that copy carries a `RuleName` property.

Rules in code: `ServiceBusAdministrationClient` takes the namespace and a `TokenCredential`, and the identity needs the Azure Service Bus Data Owner role. A duplicate rule name fails with `MessagingEntityAlreadyExists`.

Rules in the AppHost: Grey Logistics declares its rules with `AzureServiceBusRule` in `src/AppHost/Program.cs`, and Aspire generates the Bicep; hand-written Bicep uses the 2026-01-01 rules resource. Two subscriptions hold a rule named `DeliveryDelayed`, so the lab's `ConfigureInfrastructure` prefixes each rule's Bicep identifier with its subscription.
