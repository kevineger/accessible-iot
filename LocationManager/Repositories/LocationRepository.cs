using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading;

public interface ICurrentLocationRepository
{
    Task<bool> SaveLocationAsync(UserLocation userLocation, CancellationToken ct = default(CancellationToken));

    Task<IEnumerable<UserLocationDTO>> GetUsers(UserType type, CancellationToken ct = default(CancellationToken));
}

public class CurrentLocationRepository : AzureStorageBaseRepository, ICurrentLocationRepository
{
    public CurrentLocationRepository(IOptions<AzureTableOptions> options): base(options, options.Value.LocationTableName) {}

    public async Task<bool> SaveLocationAsync(UserLocation userLocation, CancellationToken ct = default(CancellationToken))
    {
        var insertOperation = TableOperation.InsertOrReplace(userLocation.ToUserLocationDTO());
        var res = await table.ExecuteAsync(insertOperation, null, null, ct);

        return res.HttpStatusCode / 100 == 2;
    }

    public async Task<IEnumerable<UserLocationDTO>> GetUsers(UserType type, CancellationToken ct)
    {
        var query = new TableQuery<UserLocationDTO>().Where(
            TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, type.ToString("g")));

        var res = await table.ExecuteQueryAsync(query, ct);
        return res;
    }
}