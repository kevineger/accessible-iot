package com.microsoft.azure.iot.hackathon.accompany.common.models;

import com.google.gson.annotations.SerializedName;

public class AccompanyLocation {
    public AccompanyLocation(double latitude, double longitude) {
        Latitude = latitude;
        Longitude = longitude;
    }

    @SerializedName("lat")
    public double Latitude;

    @SerializedName("long")
    public double Longitude;
}
