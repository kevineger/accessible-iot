public class LineGeometry {
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("coordinates")]
    public List<List<double>> Coordinates { get; set; }
}