// lat: 47.606483, long: -122.295985
using Newtonsoft.Json;

public class GPSLocation
{
    [JsonProperty("lat")]
    public double gpsLat { get; set; }

    [JsonProperty("long")]
    public double gpsLong { get; set; }
}