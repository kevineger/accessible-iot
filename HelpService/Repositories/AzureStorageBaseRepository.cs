using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

public class AzureStorageBaseRepository
{
    public readonly CloudTable table;

    public AzureStorageBaseRepository(IOptions<AzureTableOptions> options, string tableName)
    {
        var connectionString = options.Value.AzureStorageAccountConnectionString;
        var storageAccount = CloudStorageAccount.Parse(connectionString);
        var tableClient = storageAccount.CreateCloudTableClient();
        table = tableClient.GetTableReference(tableName);
        // TODO: Fix async in constructor
        table.CreateIfNotExistsAsync().Wait();
    }
}
