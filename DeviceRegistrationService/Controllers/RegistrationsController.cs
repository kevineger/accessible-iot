
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

[ApiController]
[Route("api/[controller]")]
public class RegistrationsController : ControllerBase
{
    private readonly string IoTHubConnectionString;
    private readonly string StorageAccountConnectionString;

    public RegistrationsController(IOptions<SecretOptions> options) {
        this.IoTHubConnectionString = options.Value.IotHubConnectionString;
        this.StorageAccountConnectionString = options.Value.TableStorageConnectionString;
    }

    [HttpPost]
    public async Task<ActionResult<string>> Register([FromBody] Registration model)
    {
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(StorageAccountConnectionString);
        RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubConnectionString);

        var tableClient = storageAccount.CreateCloudTableClient();
        var cloudTable = tableClient.GetTableReference("users");

        await cloudTable.CreateIfNotExistsAsync();

        var user = new User
        {
            PartitionKey = model.DeviceId,
            RowKey = model.DeviceId
        };

        var queryOperation = TableOperation.Retrieve<User>(model.DeviceId, model.DeviceId);
        var retrieveResult = await cloudTable.ExecuteAsync(queryOperation);

        var connectionString = "";

        if (retrieveResult.Result != null)
        {
            // Retrieved registration entity from table storage, get the connection string from registry manager.
            var device = await registryManager.GetDeviceAsync(model.DeviceId);
            connectionString = ((User) retrieveResult.Result).ConnectionString;
        }
        else
        {
            // Create registration entity in table and register the device with IoTHub.
            var device = await registryManager.AddDeviceAsync(new Device(model.DeviceId));
            connectionString = device?.Authentication.SymmetricKey.PrimaryKey;

            user.ConnectionString = connectionString;
            
            var insertOperation = TableOperation.Insert(user);
            await cloudTable.ExecuteAsync(insertOperation);
        }

        return Ok(new RegistrationResult
        {
            ConnectionString = connectionString
        });
    }
}