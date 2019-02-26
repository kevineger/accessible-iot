using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationManager.Models
{
    public class ClosestPointResponse
    {
        [JsonProperty("result")]
        public ClosestPointResultEntry[] Result { get; set; }

        [JsonProperty("summary")]
        public PostClosestPointSummary Summary { get; set; }
    }

    public class PostClosestPointSummary
    {
        [JsonProperty("information")]
        public string Information { get; set; }

        [JsonProperty("sourcePoint")]
        public Coordinate SourcePoint { get; set; }

        [JsonProperty("udid")]
        public string Udid { get; set; }
    }

    public class ClosestPointResultEntry
    {
        [JsonProperty("distanceInMeters")]
        public int DistanceInMeters { get; set; }

        [JsonProperty("geometryId")]
        public string GeometryId { get; set; }

        [JsonProperty("position")]
        public Coordinate Position { get; set; }
    }

    public class Coordinate
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lon")]
        public double Lon { get; set; }
    }
}
