using Microsoft.Azure;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;

public interface ICurrentLocationRepository
{

}

public class CurrentLocationRepository : ICurrentLocationRepository
{
    public CurrentLocationRepository(IOptions<AzureTableOptions> options)
    {
        var connectionString = options.Value.AzureStorageAccountConnectionString;
        var tableName = options.Value.TableName;

        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient();
        var table = tableClient.GetTableReference(tableName);
        // TODO: Fix async in constructor
        table.CreateIfNotExistsAsync().Wait();
    }
}