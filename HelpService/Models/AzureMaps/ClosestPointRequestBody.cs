using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LocationManager.Models.AzureMaps
{
    public class ClosestPointRequestBody
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("features")]
        public List<Feature> Features { get; set; }
    }

    public class Feature
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("properties")]
        public Properties Properties { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("coordinates")]
        public List<double> Coordinates { get; set; }
    }

    public class Properties
    {
        [JsonProperty("geometryId")]
        public string GeometryId { get; set; }
    }
}
