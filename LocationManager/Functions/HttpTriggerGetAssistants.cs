using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

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
            var container = ContainerHelper.Build(context);
            var locationRepo = (ICurrentLocationRepository)container.GetService(typeof(ICurrentLocationRepository));
            var mapsRepo = (IAzureMapsRepository)container.GetService(typeof(IAzureMapsRepository));
            
            var assistants = await locationRepo.GetUsers(UserType.Assistant);
            // TODO: Call Azure Maps for the collection of Users to send a message to. Determine closest 5.
            // TODO: Send notification to closest 5.

            return (ActionResult)new OkObjectResult($"Done.");
        }
    }
}
