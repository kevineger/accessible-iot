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
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] UserLocation userLocation,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var container = ContainerHelper.Build(context);
            var repo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));
            var res = await repo.SaveLocationAsync(userLocation);

            if (res)
            {
                return (ActionResult)new OkObjectResult($"Done.");
            }
            else
            {
                log.LogError($"Failed to save user location.");
                return new BadRequestObjectResult("We messed up.");
            }
        }
    }
}
