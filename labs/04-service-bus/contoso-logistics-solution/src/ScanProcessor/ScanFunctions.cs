using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Messaging.ServiceBus;
using Contoso.Contracts;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ScanProcessor;

public class ScanFunctions(ILogger<ScanFunctions> logger, ParcelScanStore store)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    [Function("ProcessScan")]
    public async Task ProcessScan(
        [ServiceBusTrigger("parcel-events", "tracking", Connection = "ServiceBusConnection", AutoCompleteMessages = false, IsSessionsEnabled = true)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        var scan = message.Body.ToObjectFromJson<ParcelScanned>(JsonOptions)!;

        logger.LogInformation("Scan {MessageId} for session {SessionId}", message.MessageId, message.SessionId);

        await store.TryAppendAsync(message.SessionId, scan.ScanType.ToString(), message.MessageId);

        await messageActions.CompleteMessageAsync(message);
    }
}
