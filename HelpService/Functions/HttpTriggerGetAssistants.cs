using System.IO;
using System.Linq;
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
    public static class NewFunc
    {
        [FunctionName("HttpTriggerGetAssistants")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var individualUserLocationString = await (new StreamReader(req.Body, Encoding.UTF8)).ReadToEndAsync().ConfigureAwait(false);
            var individualLocation = JsonConvert.DeserializeObject<UserLocation>(individualUserLocationString);
            var container = ContainerHelper.Build(context);
            var locationRepo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));
            var mapsRepo = (IAzureMapsRepository)container.GetService(typeof(IAzureMapsRepository));
            
            var assistantsDTO = await locationRepo.GetUsers(UserType.Assistant);
            var assistantsLocations = assistantsDTO.Select(t => t.ToUserLocation());
            var closestUserIds = mapsRepo.GetClosestUserIds(assistantsLocations, individualLocation);

            // TODO: Call Azure Maps for the collection of Users to send a message to. Determine closest 5.
            // TODO: Send notification to closest 5.

            return (ActionResult)new OkObjectResult(closestUserIds);
        }
    }
}
