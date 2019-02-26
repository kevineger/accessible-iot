using Microsoft.Azure;
using Microsoft.Azure.Storage;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading;

public interface IAcknoledgeRepository
{
    Task<bool> SaveAcknowledgeAsync(Acknowledge acknowledge, CancellationToken ct = default(CancellationToken));
}

public class AcknoledgeRepository : AzureStorageBaseRepository, IAcknoledgeRepository
{
    public AcknoledgeRepository(IOptions<AzureTableOptions> options) : base(options, options.Value.AcknowledgeTableName) { }

    public async Task<bool> SaveAcknowledgeAsync(Acknowledge acknowledge, CancellationToken ct = default(CancellationToken))
    {
        var insertOperation = TableOperation.Insert(acknowledge.ToAcknowledgeDTO());
        var res = await table.ExecuteAsync(insertOperation, null, null, ct);

        return res.HttpStatusCode / 100 == 2;
    }
}