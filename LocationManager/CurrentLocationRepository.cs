using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.Azure.Storage; // Namespace for StorageAccounts
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;

public interface ICurrentLocationRepository
{
    Task<bool> SaveLocationAsync(UserLocation userLocation);
}

public class CurrentLocationRepository : ICurrentLocationRepository
{
    private readonly CloudTable table;
 
    public CurrentLocationRepository(IOptions<AzureTableOptions> options)
    {
        var connectionString = options.Value.AzureStorageAccountConnectionString;
        var tableName = options.Value.TableName;

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient();
        table = tableClient.GetTableReference(tableName);
        // TODO: Fix async in constructor
        table.CreateIfNotExistsAsync().Wait();
    }

    public async Task<bool> SaveLocationAsync(UserLocation userLocation)
    {
        var insertOperation = TableOperation.InsertOrReplace(userLocation.ToUserLocationDTO());
        var res = await table.ExecuteAsync(insertOperation);

        return res.HttpStatusCode / 100 == 2;
    }

    public async Task<IEnumerable<UserLocationDTO>> GetUsers(UserType type) {
        var query = new TableQuery<UserLocationDTO>().Where(
            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, type.ToString("g")));

        var res = await table.ExecuteQuerySegmentedAsync(query, null);
        return res.Results;
    }
}