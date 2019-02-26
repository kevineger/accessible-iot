using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

public static class EventHubTriggerPersistLocation
{
    public static string ConnectionStr() {
        return "";
    }

    [FunctionName("EventHubTriggerPersistLocation")]
    public static void Run(
    [EventHubTrigger("samples-workitems", Connection = "EventHubConnectionAppSetting")] EventData eventHubMessage,
    DateTime enqueuedTimeUtc,
    Int64 sequenceNumber,
    string offset,
    ILogger log,
    ExecutionContext context)
    {
        // var container = ContainerHelper.Build(context);

        log.LogInformation($"Event: {Encoding.UTF8.GetString(eventHubMessage.Body)}");
        // Metadata accessed by binding to EventData
        log.LogInformation($"EnqueuedTimeUtc={eventHubMessage.SystemProperties.EnqueuedTimeUtc}");
        log.LogInformation($"SequenceNumber={eventHubMessage.SystemProperties.SequenceNumber}");
        log.LogInformation($"Offset={eventHubMessage.SystemProperties.Offset}");
        // Metadata accessed by using binding expressions in method parameters
        log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
        log.LogInformation($"SequenceNumber={sequenceNumber}");
        log.LogInformation($"Offset={offset}");
    }
}