
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly string StorageAccountConnectionString;

    public RoutesController(IOptions<SecretOptions> secrets)
    {
        this.StorageAccountConnectionString = secrets.Value.TableStorageConnectionString;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LineGeometry>> GetRoute(string id)
    {
        string route = null;
        try
        {
            route = await BlobHelper.GetRouteFromBlob(StorageAccountConnectionString, id);
        }
        catch (Exception e)
        {
            // Catch and release ;)
            throw e;
        }

        return Ok(route);
    }
}