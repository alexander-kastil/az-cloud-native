# Scan Processing

| Pattern | Service Bus feature | Implemented in |
| --- | --- | --- |
| Filtered subscriptions | Subscription correlation filters on `Subject` | `src/AppHost/Program.cs` (`AddSubjectSubscription`) |
| Ordered scans per parcel | Sessions, with the tracking number as session ID | `src/AppHost/Program.cs` (`RequiresSession` on the `tracking` subscription), `src/ScanProcessor/ScanFunctions.cs` (`IsSessionsEnabled = true`) |
| Dead-lettered unreadable scans | Dead-lettering with reason `UnreadableLabel` | `src/ScanProcessor/ScanFunctions.cs` (`DeadLetterMessageAsync`), `src/ScanProcessor/ScanValidator.cs` |
| Duplicate-safe consumer | Duplicate detection plus a `processed-{messageId}` marker written in the same Cosmos DB transactional batch as the scan | `src/AppHost/Program.cs` (`RequiresDuplicateDetection` on `parcel-events`), `src/ScanProcessor/ParcelScanStore.cs` (`TryAppendAsync`) |
| KEDA scale rule | `azure-servicebus` scale rule on the `tracking` subscription backlog, 0 to 10 replicas | `infra/scan-consumer.azcli` |
