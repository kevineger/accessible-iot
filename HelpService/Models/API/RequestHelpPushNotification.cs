using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RequestHelpPushNotification
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("sourceDeviceId")]
    public string SourceDeviceId { get; set; }

    [JsonProperty("targetDeviceIds")]
    public string[] TargetDeviceIds { get; set; }

    public static RequestHelpPushNotification Create(string sourceDeviceId, IEnumerable<string> targetDeviceIds, string requestType)
    {
        return new RequestHelpPushNotification()
        {
            SourceDeviceId = sourceDeviceId,
            TargetDeviceIds = targetDeviceIds.ToArray(),
            Type = requestType,
        };
    }
}