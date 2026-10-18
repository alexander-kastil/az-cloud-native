using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ScanProcessor;

public class ScanFunctions(ILogger<ScanFunctions> logger, ParcelScanStore store)
{
    [Function("ProcessScan")]
    public async Task ProcessScan(
        [ServiceBusTrigger("parcel-events", "tracking", Connection = "ServiceBusConnection", AutoCompleteMessages = false, IsSessionsEnabled = true)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var (scan, error) = ScanValidator.Validate(message.Body);

        if (scan is null)
        {
            logger.LogWarning("Unreadable scan {MessageId} for session {SessionId}: {Error}", message.MessageId, message.SessionId, error);
            await messageActions.DeadLetterMessageAsync(message, null, "UnreadableLabel", error);
            return;
        }

        logger.LogInformation("Scan {MessageId} for session {SessionId}", message.MessageId, message.SessionId);

        var appended = await store.TryAppendAsync(message.SessionId, scan.ScanType.ToString(), message.MessageId);

        if (!appended)
        {
            logger.LogWarning("Duplicate scan {MessageId} for session {SessionId}", message.MessageId, message.SessionId);
        }

        await messageActions.CompleteMessageAsync(message);
    }
}
