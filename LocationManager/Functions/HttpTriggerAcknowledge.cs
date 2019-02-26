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
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var azureMapsRouteCallBase = "https://atlas.microsoft.com/route/directions/json?subscription-key=7TqHJ3076KEoC-FRyx_gycYRNefYeVJdldgKomtKBfI&api-version=1.0&routeRepresentation=polyline&travelMode=pedestrian&instructionsType=tagged";

            string caretaker = req.Query["caretaker"];
            string action = req.Query["action"];
            string individual = req.Query["individual"];
            double gpslongcaretaker = Convert.ToDouble(req.Query["gpslongcaretaker"]);
            double gpslatcaretaker = Convert.ToDouble(req.Query["gpslatcaretaker"]);
            double gpslongindividual = Convert.ToDouble(req.Query["gpslongindividual"]);
            double gpslatindividual = Convert.ToDouble(req.Query["gpslatindividual"]);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            caretaker = caretaker ?? data?.caretaker;
            action = action ?? data?.action;
            individual = individual ?? data?.individual;
            gpslongcaretaker = data?.gpslongcaretaker;
            gpslatcaretaker = data?.gpslatcaretaker;
            gpslongindividual = data?.gpslongindividual;
            gpslatindividual = data?.gpslatindividual;

            // Define the row
            string sRow = caretaker + action + individual + gpslongcaretaker + gpslatcaretaker + gpslongindividual + gpslatindividual;

            // Create the Entity and set the partition
            AckEntity _ackEntity = new AckEntity(individual, sRow);
            _ackEntity.Caretaker = caretaker;
            _ackEntity.Action = action;
            _ackEntity.Individual = individual;
            _ackEntity.GPSLongCaretaker = gpslongcaretaker;
            _ackEntity.GPSLatCaretaker = gpslatcaretaker;
            _ackEntity.GPSLongIndividual = gpslongindividual;
            _ackEntity.GPSLatIndividual = gpslatindividual;

            // Connect to the Storage account to write caretaker + action + individual + gpslongcaretaker + gpslatcaretaker
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=geofencingappacb737;AccountKey=4iyN6jL//5n0J3ay13Gm3VuFiSrCVGPfyi6Vv1bJF8RAlLfAIlv7jqWKpvUa/wwkfKWqUGOW5+590lq4rZjbXQ==;EndpointSuffix=core.windows.net");
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            CloudTable table = tableClient.GetTableReference("GeoFencingTableStorage");
            await table.CreateIfNotExistsAsync();

            TableOperation insertOperation = TableOperation.Insert(_ackEntity);

            try
            {
                await table.ExecuteAsync(insertOperation);

                // Call Azure Maps with Lat and Long of Caretaker and Individual to get the route between the two
                var azureMapsRouteAPI = azureMapsRouteCallBase + GetRouteQueryParam(gpslatcaretaker, gpslongcaretaker, gpslatindividual, gpslongindividual);
                var mapsResponse = await GetAsync(azureMapsRouteAPI).ConfigureAwait(false);

                return (ActionResult)new OkObjectResult(mapsResponse);
            }
            catch (StorageException)
            {
                return (ActionResult)new OkObjectResult("Help is already dispatched. Thank you!");
            }

            //return caretaker != null
            //? (ActionResult)new OkObjectResult($"Hello, you are {caretaker} with GPS location Long {gpslongcaretaker} and Lat {gpslatcaretaker}. You want to {action} for {individual} with GPS location Long {gpslongindividual} and Lat {gpslatindividual}")
            //: new BadRequestObjectResult("Please pass a caretaker on the query string or in the request body");
        }

        private static async Task<string> GetAsync(string url)
        {
            var responseObject = await httpClient.GetAsync(url).ConfigureAwait(false);
            if(responseObject.IsSuccessStatusCode)
            {
                return await responseObject.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            return $"Having trouble with AzureMaps. AzureMaps response Code : {responseObject.StatusCode}";
        }

        private static string GetRouteQueryParam(double startLat, double startLon, double endLat, double endLon)
        {
            return $"&query={startLat},{startLon}:{endLat},{endLon}";
        }

        public class AckEntity : TableEntity
        {
            public AckEntity(string skey, string srow)
            {
                this.PartitionKey = skey;
                this.RowKey = srow;
            }

            public AckEntity() { }
            public string Caretaker { get; set; }
            public string Action { get; set; }
            public string Individual { get; set; }
            public double GPSLongCaretaker { get; set; }
            public double GPSLatCaretaker { get; set; }
            public double GPSLongIndividual { get; set; }
            public double GPSLatIndividual { get; set; }
        }
    }
}
