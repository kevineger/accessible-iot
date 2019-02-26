using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;

namespace FunctionApp
{
    public static class HttpTriggerAcknowledge
    {
        private static string showDirectionsPushUrl = "https://stegawhackathon.azurewebsites.net/api/Notifications";

        [FunctionName("HttpTriggerAcknowledge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]Acknowledge acknowledge,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var container = ContainerHelper.Build(context);
            var acknowledgeRepo = (IAcknoledgeRepository)container.GetService(typeof(IAcknoledgeRepository));
            var mapsRepo = (IAzureMapsRepository)container.GetService(typeof(IAzureMapsRepository));
            
            try
            {
                var res = await acknowledgeRepo.SaveAcknowledgeAsync(acknowledge);

                var getDirectionsResponse = await mapsRepo.GetPedestrianDirections(acknowledge.CareTakerLocation, acknowledge.CareRecipientLocation);

                var directionsPathAsGeoJson = getDirectionsResponse.GetRouteAsGeoJson();

                var (nextStatus, nextResponseMessage) = await HttpClientHelper.PostAsync(showDirectionsPushUrl, directionsPathAsGeoJson);

                if (nextStatus == System.Net.HttpStatusCode.OK
                    || nextStatus == System.Net.HttpStatusCode.Created
                    || nextStatus == System.Net.HttpStatusCode.Accepted)
                {
                    return (ActionResult)new OkObjectResult($"Successfully triggered the next step. Show Directions. Body used : {directionsPathAsGeoJson}");
                }

                return (ActionResult)new BadRequestObjectResult($"Failed to trigger the next step.S how Directions. Body used : {directionsPathAsGeoJson}");
            }
            catch (StorageException)
            {
                return (ActionResult)new OkObjectResult("Help is already dispatched. Thank you!");
            }
        }
    }
}
