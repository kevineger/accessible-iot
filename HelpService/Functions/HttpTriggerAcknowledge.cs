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
        private static HttpClient httpClient = new HttpClient();

        [FunctionName("HttpTriggerAcknowledge")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req,
            ILogger log,
            ExecutionContext context)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            var container = ContainerHelper.Build(context);
            var acknowledge = await RequestHelper.ReadAsync<Acknowledge>(req, log);
            var acknowledgeRepo = (IAcknoledgeRepository)container.GetService(typeof(IAcknoledgeRepository));


            var azureMapsRouteCallBase = "https://atlas.microsoft.com/route/directions/json?subscription-key=7TqHJ3076KEoC-FRyx_gycYRNefYeVJdldgKomtKBfI&api-version=1.0&routeRepresentation=polyline&travelMode=pedestrian&instructionsType=tagged";
            try
            {
                var res = await acknowledgeRepo.SaveAcknowledgeAsync(acknowledge);
                
                // Call Azure Maps with Lat and Long of Caretaker and Individual to get the route between the two
                var azureMapsRouteAPI = azureMapsRouteCallBase + GetRouteQueryParam(
                    acknowledge.CareTakerLocation.gpsLat,
                    acknowledge.CareTakerLocation.gpsLong,
                    acknowledge.CareRecipientLocation.gpsLat,
                    acknowledge.CareRecipientLocation.gpsLong);
                var mapsResponse = await GetAsync(azureMapsRouteAPI).ConfigureAwait(false);

                return (ActionResult)new OkObjectResult(mapsResponse);
            }
            catch (StorageException)
            {
                return (ActionResult)new OkObjectResult("Help is already dispatched. Thank you!");
            }
        }

        private static async Task<string> GetAsync(string url)
        {
            var responseObject = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (responseObject.IsSuccessStatusCode)
            {
                return await responseObject.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return $"Having trouble with AzureMaps. AzureMaps response Code : {responseObject.StatusCode}";
        }

        private static string GetRouteQueryParam(double startLat, double startLon, double endLat, double endLon)
        {
            return $"&query={startLat},{startLon}:{endLat},{endLon}";
        }
    }
}
