
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{

    private readonly string NotificationHubsApiKey;

    private readonly string NotificationHubsNamespace;

    public NotificationsController(IOptions<SecretOptions> secrets)
    {
        this.NotificationHubsApiKey = secrets.Value.NotificationHubsConnectionString;
        this.NotificationHubsNamespace = secrets.Value.NotificationHubsNamespace;
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
        var notificationHubClient = NotificationHubClient.CreateClientFromConnectionString(NotificationHubsApiKey, NotificationHubsNamespace);

        var notificationModel = new LineGeometryNotification
        {
            LineGeometry = lineGeometry

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