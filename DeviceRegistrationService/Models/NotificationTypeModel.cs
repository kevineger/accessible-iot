using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class NotificationTypeModel
{
    public NotificationType Type { get; set; }

    public string SourceDeviceId { get; set; }
    
    public string[] TargetDeviceIds { get; set; }
}

public enum NotificationType
{
    RequestNearbyHelp,
    RequestFriendsAndFamilyHelp,
    AlertNearby,
    AlertFriendsAndFamily,
    DeviationFromPath
}