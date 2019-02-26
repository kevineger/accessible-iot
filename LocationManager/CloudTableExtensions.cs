using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

public static class CloudTableExtensions
{
    public static async Task<IList<T>> ExecuteQueryAsync<T>(
        this CloudTable table,
        TableQuery<T> query,
        CancellationToken ct = default(CancellationToken),
        Action<IList<T>> onProgress = null) where T : TableEntity, new()
    {
        var items = new List<T>();
        TableContinuationToken tableToken = null;

        do
        {
            var seg = await table.ExecuteQuerySegmentedAsync<T>(query, tableToken);
            tableToken = seg.ContinuationToken;
            items.AddRange(seg);
            if (onProgress != null) onProgress(items);

        } while (tableToken != null && !ct.IsCancellationRequested);

        return items;
    }
}