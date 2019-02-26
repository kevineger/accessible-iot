using LocationManager.Models.AzureMaps;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class GetDirectionsResponse
{
    public string formatVersion { get; set; }
    public string copyright { get; set; }
    public string privacy { get; set; }
    public List<Route> routes { get; set; }

    public string GetRouteAsGeoJson()
    {
        var geometry = new LineGeometry()
        {
            Type = "LineString",
            Coordinates = routes.SelectMany(r => r.legs.SelectMany(l => l.points.Select(p => new List<double>() { p.longitude, p.latitude }))).ToList(),
        };

        return JsonConvert.SerializeObject(geometry);
    }
}

public class Summary
{
    public int lengthInMeters { get; set; }
    public int travelTimeInSeconds { get; set; }
    public int trafficDelayInSeconds { get; set; }
    public DateTime departureTime { get; set; }
    public DateTime arrivalTime { get; set; }
}

public class Point
{
    public double latitude { get; set; }
    public double longitude { get; set; }
}

/// <summary>
/// This is to be used in the map control to draw a line.
/// </summary>
public class LineGeometry
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("coordinates")]
    public List<List<double>> Coordinates { get; set; }
}

public class Leg
{
    public Summary summary { get; set; }
    public List<Point> points { get; set; }
}

public class Section
{
    public int startPointIndex { get; set; }
    public int endPointIndex { get; set; }
    public string sectionType { get; set; }
    public string travelMode { get; set; }
}

public class Route
{
    public Summary summary { get; set; }
    public List<Leg> legs { get; set; }
    public List<Section> sections { get; set; }
}
