
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

    public RoutesController(IOptions<SecretOptions> secrets) {
        this.StorageAccountConnectionString = secrets.Value.TableStorageConnectionString;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LineGeometry>> GetRoute(string id)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageAccountConnectionString);

        var tableClient = storageAccount.CreateCloudTableClient();
        var cloudTable = tableClient.GetTableReference("routes");

        await cloudTable.CreateIfNotExistsAsync();

        var queryOperation = TableOperation.Retrieve<LineGeometryDataModel>(id, id);
        var retrieveResult = await cloudTable.ExecuteAsync(queryOperation);

        if(retrieveResult.Result != null) {

            var lineGeomDataModel = ((LineGeometryDataModel) retrieveResult.Result);

            return Ok(JsonConvert.DeserializeObject<LineGeometry>(lineGeomDataModel.Route));
        }else{ 
            return StatusCode(404);
        }
    }
}