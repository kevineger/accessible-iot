package com.microsoft.azure.iot.hackathon.accompany.common.models;

import com.google.gson.annotations.SerializedName;

public class AccompanyLocation {
    public AccompanyLocation(long latitude, long longitude) {
        Latitude = latitude;
        Longitude = longitude;
    }

    @SerializedName("lat")
    public long Latitude;

    @SerializedName("long")
    public long Longitude;
}
