using System;
using System.Text;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class EventHubTriggerPersistLocation
{
    [FunctionName("EventHubTriggerPersistLocation")]
    public static async System.Threading.Tasks.Task RunAsync(
    [EventHubTrigger("stegawiothub", Connection = "EventHubConnStr", ConsumerGroup = "azurefunctionsconsumergroup")] EventData eventHubMessage,
    DateTime enqueuedTimeUtc,
    Int64 sequenceNumber,
    string offset,
    ILogger log,
    ExecutionContext context)
    {
        log.LogInformation($"Event: {Encoding.UTF8.GetString(eventHubMessage.Body)}");
        // Metadata accessed by binding to EventData
        log.LogInformation($"EnqueuedTimeUtc={eventHubMessage.SystemProperties.EnqueuedTimeUtc}");
        log.LogInformation($"SequenceNumber={eventHubMessage.SystemProperties.SequenceNumber}");
        log.LogInformation($"Offset={eventHubMessage.SystemProperties.Offset}");
        // Metadata accessed by using binding expressions in method parameters
        log.LogInformation($"EnqueuedTimeUtc={enqueuedTimeUtc}");
        log.LogInformation($"SequenceNumber={sequenceNumber}");
        log.LogInformation($"Offset={offset}");

        var container = ContainerHelper.Build(context);
        var repo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));
        var userLocation = JsonConvert.DeserializeObject<UserLocation>(Encoding.UTF8.GetString(eventHubMessage.Body));
        var res = await repo.SaveLocationAsync(userLocation);

        if (res)
        {
            log.LogInformation($"Successfully stored location. User: {userLocation.UserId}. Lat: {userLocation.Location.gpsLat}. Long: {userLocation.Location.gpsLong}.");
        }
        else
        {
            log.LogError($"Failed to save user location.");
        }
    }
}