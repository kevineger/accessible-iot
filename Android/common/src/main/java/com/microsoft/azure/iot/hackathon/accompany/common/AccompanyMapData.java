package com.microsoft.azure.iot.hackathon.accompany.common;

import com.mapbox.geojson.GeometryCollection;
import com.mapbox.geojson.LineString;
import com.mapbox.geojson.Point;


public class AccompanyMapData {

    public static final String UPDATED = "MAP_DATA_UPDATED";

    public static LineString lineGeom = LineString.fromJson("{\n" +
            "   \"type\": \"LineString\",\n" +
            "   \"coordinates\": [\n" +
            "       [-122.33, 47.64], [47.64, -122.33]\n" +
            "   ]\n" +
            "}");

    public static Point destination =  Point.fromLngLat(-122.33, 47.64);

    public static GeometryCollection destinations = GeometryCollection.fromGeometry(Point.fromLngLat(-122.33, 47.64));
}
