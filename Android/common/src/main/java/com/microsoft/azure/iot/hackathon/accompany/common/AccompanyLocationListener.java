package com.microsoft.azure.iot.hackathon.accompany.common;

import android.location.Location;
import android.location.LocationListener;
import android.os.Bundle;
import android.util.Log;

import com.google.android.gms.common.internal.service.Common;
import com.microsoft.azure.sdk.iot.device.DeviceClient;
import com.microsoft.azure.sdk.iot.device.Message;

import org.json.JSONObject;

public class AccompanyLocationListener implements LocationListener {

    private static final String TAG = "AccompanyLocationListener";
    private DeviceClient deviceClient;

    public AccompanyLocationListener(DeviceClient deviceClient) {
        this.deviceClient = deviceClient;
    }

    @Override
    public void onLocationChanged(Location location) {
        Log.d(TAG, "Lat: " + location.getLatitude() + " Long: " + location.getLongitude());

        try {

            JSONObject jsonObject = new JSONObject();
            jsonObject.put("userId", CommonApplication.AndroidId);
            jsonObject.put("userType", "CareRecipient");


            JSONObject locationObject = new JSONObject();
            locationObject.put("lat", location.getLatitude());
            locationObject.put("long", location.getLongitude());

            jsonObject.put("location", locationObject);

            Message updateLocationMessage = new Message(jsonObject.toString());
            deviceClient.sendEventAsync(updateLocationMessage, null, null);
        } catch (Exception e ){
            Log.d(TAG, "Failed to send event. " + e.getMessage());
        }
    }

    @Override
    public void onStatusChanged(String provider, int status, Bundle extras) {

    }

    @Override
    public void onProviderEnabled(String provider) {

    }

    @Override
    public void onProviderDisabled(String provider) {

    }
}
