using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Azure.IoT
{
    public static class HttpTriggerGetAssistants
    {
        [FunctionName("HttpTriggerGetAssistants")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var userLocation = await RequestHelper.ReadAsync<UserLocation>(req, log);
            var container = ContainerHelper.Build(context);
            var locationRepo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));
            var mapsRepo = (IAzureMapsRepository)container.GetService(typeof(IAzureMapsRepository));
            
            var assistantsDTO = await locationRepo.GetUsers(UserType.Assistant);
            var assistantsLocations = assistantsDTO.Select(t => t.ToUserLocation());
            var closestUserIds = await mapsRepo.GetClosestUserIds(assistantsLocations, userLocation);

            // TODO: Send notification to closest 5.

            return (ActionResult)new OkObjectResult(closestUserIds);
        }
    }
}
