using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LocationManager.Models.AzureMaps;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using LocationManager.Models;

public interface IAzureMapsRepository {
    Task<IEnumerable<string>> GetClosestUserIds(IEnumerable<UserLocation> helpersLocations, UserLocation userLocation);
}

public class AzureMapsRepository : IAzureMapsRepository
{
    private string subscriptionKey;
    private static HttpClient httpClient = new HttpClient();
    private static string azureMapsGetClosestPointApi = "https://atlas.microsoft.com/spatial/closestPoint/json?subscription-key={0}&api-version=1.0&lat={1}&lon={2}&numberOfClosestPoints={3}";

    public AzureMapsRepository(IOptions<AzureMapsOptions> options)
    {
        subscriptionKey =  options.Value.SubscriptionKey;
    }

    public async Task<IEnumerable<string>> GetClosestUserIds(IEnumerable<UserLocation> helpersLocations, UserLocation userLocation)
    {
        var response = Enumerable.Empty<string>();
        var requestBody = GetClosestPointRequestBody(helpersLocations);
        var requestUrl = GetAzureMapsPostURLToGetClosestPoints(subscriptionKey, userLocation.Location.gpsLat, userLocation.Location.gpsLong);
        var requestBodySerialized = JsonConvert.SerializeObject(requestBody);
        var (responseCode, responseBodySerialized) = await HttpClientHelper.PostAsync(requestUrl, requestBodySerialized);

        if (responseCode == System.Net.HttpStatusCode.OK)
        {
            var closestPointResponse = JsonConvert.DeserializeObject<ClosestPointResponse>(responseBodySerialized);

            if (closestPointResponse != null)
            {
                response = closestPointResponse.Result.Select(t => t.GeometryId);
            }
        }

        return response;
    }

    private static string GetAzureMapsPostURLToGetClosestPoints(string subscriptionKey,
        double currentLat,
        double currentLon,
        int numberOfClosestPointsToReturn = 5)
    {
        return string.Format(azureMapsGetClosestPointApi
            , subscriptionKey
            , currentLat.ToString()
            , currentLon.ToString()
            , numberOfClosestPointsToReturn.ToString());
    }

    private static ClosestPointRequestBody GetClosestPointRequestBody(IEnumerable<UserLocation> helpersLocations)
    {
        var featureList = helpersLocations.Select(h => new Feature()
        {
            Type = "Feature",
            Properties = new Properties() { GeometryId = h.UserId.ToString() },
            Geometry = new Geometry() { Type = "Point", Coordinates = new List<double>() { h.Location.gpsLong, h.Location.gpsLat } },
        }).ToList();


        return new ClosestPointRequestBody()
        {
            Type = "FeatureCollection",
            Features = featureList,
        };
    }
}