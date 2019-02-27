
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{

    private readonly string NotificationHubsApiKey;

    private readonly string NotificationHubsNamespace;

    private readonly string StorageAccountConnectionString;

    public NotificationsController(IOptions<SecretOptions> secrets)
    {
        this.NotificationHubsApiKey = secrets.Value.NotificationHubsConnectionString;
        this.NotificationHubsNamespace = secrets.Value.NotificationHubsNamespace;
        this.StorageAccountConnectionString = secrets.Value.TableStorageConnectionString;
    }

    [HttpPost]
    public async Task<ActionResult> Notify([FromBody] NotificationTypeModel model)
    {
        var notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(NotificationHubsApiKey, NotificationHubsNamespace);
        var message = new Message
        {
            Data = model
        };

        try
        {
            var outcome = await notificationHubClient.SendFcmNativeNotificationAsync(JsonConvert.SerializeObject(message));

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("directions")]
    public async Task<ActionResult> ShowDirections([FromBody] LineGeometry lineGeometry)
    {
        var routeId = Guid.NewGuid().ToString();

        await BlobHelper.PostRouteToBlob(StorageAccountConnectionString, routeId, JsonConvert.SerializeObject(lineGeometry));

        var notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(NotificationHubsApiKey, NotificationHubsNamespace);

        var notificationModel = new LineGeometryNotification
        {
            ResourceUrl = $"/api/Routes/{routeId}"
        };

        var message = new Message
        {
            Data = notificationModel
        };

        try
        {
            var outcome = await notificationHubClient.SendFcmNativeNotificationAsync(JsonConvert.SerializeObject(message));

            return Ok();
        }
        catch (Exception e)
        {
            return StatusCode(500);
        }
    }
}