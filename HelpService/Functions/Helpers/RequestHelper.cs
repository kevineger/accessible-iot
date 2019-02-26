using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public static class RequestHelper
{
    public static async Task<T> ReadAsync<T>(HttpRequest req, ILogger log)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        T val = default(T);
        try
        {
            val = JsonConvert.DeserializeObject<T>(requestBody);
        }
        catch (Exception e)
        {
            log.Log(LogLevel.Information, e, $"Bad request. User payload: {requestBody}.");
            throw e;
        }

        return val;
    }
}