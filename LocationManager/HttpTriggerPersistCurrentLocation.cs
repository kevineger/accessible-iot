using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace Azure.IoT
{
    public static class HttpTriggerPersistCurrentLocation
    {
        [FunctionName("HttpTriggerPersistCurrentLocation")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var container = new ContainerBuilder()
                .RegisterModule(new Module(context))
                .Build();

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            UserLocation userLocation = null;
            try
            {
                userLocation = JsonConvert.DeserializeObject<UserLocation>(requestBody);
            }
            catch (Exception e)
            {
                log.Log(LogLevel.Information, e, $"Bad request. User payload: {requestBody}.");
                return new BadRequestObjectResult(e);
            }

            var repo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));

            return (ActionResult)new OkObjectResult($"Done.");
        }
    }
}
