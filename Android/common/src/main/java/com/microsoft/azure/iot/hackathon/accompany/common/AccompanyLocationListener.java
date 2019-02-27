package com.microsoft.azure.iot.hackathon.accompany.common;

import android.location.Location;
import android.location.LocationListener;
import android.os.Bundle;
import android.util.Log;

import com.google.gson.Gson;
import com.microsoft.azure.iot.hackathon.accompany.common.models.AccompanyLocation;
import com.microsoft.azure.iot.hackathon.accompany.common.models.UpdateLocationRequest;
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
            UpdateLocationRequest updateLocationRequest = new UpdateLocationRequest();
            updateLocationRequest.userId = CommonApplication.AndroidId;

            // TODO: This value depends on provider or consumer app.
            updateLocationRequest.userType = "CareRecipient";

            AccompanyLocation locationObj = new AccompanyLocation(location.getLatitude(), location.getLongitude());
            updateLocationRequest.location = locationObj;

            String data = new Gson().toJson(updateLocationRequest);

            Message updateLocationMessage = new Message(data);
            deviceClient.sendEventAsync(updateLocationMessage, null, null);
        } catch (Exception e) {
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
