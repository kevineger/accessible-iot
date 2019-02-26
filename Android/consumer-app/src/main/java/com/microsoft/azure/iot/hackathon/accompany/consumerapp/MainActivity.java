package com.microsoft.azure.iot.hackathon.accompany.consumerapp;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;
import android.support.annotation.NonNull;
import android.util.Log;
import android.widget.Toast;

import com.google.android.gms.common.internal.service.Common;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.FirebaseApp;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.InstanceIdResult;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyLocationListener;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyRegistrationIntentService;
import com.microsoft.azure.iot.hackathon.accompany.common.CommonApplication;
import com.microsoft.azure.iot.hackathon.accompany.common.MapActivity;
import com.microsoft.azure.sdk.iot.device.Message;

import org.json.JSONObject;

import io.nlopez.smartlocation.OnLocationUpdatedListener;
import io.nlopez.smartlocation.SmartLocation;
import io.nlopez.smartlocation.location.config.LocationAccuracy;
import io.nlopez.smartlocation.location.config.LocationParams;

public class MainActivity extends MapActivity {

    private static final String TAG = "AccompanyConsumer";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mapControl = findViewById(R.id.mapControl);
        mapControl.onCreate(savedInstanceState);

        requestPermissions(new String[] { Manifest.permission.ACCESS_FINE_LOCATION }, 1340);

        LocationParams.Builder builder = new LocationParams.Builder()
                .setAccuracy(LocationAccuracy.HIGH)
                .setInterval(100);

        SmartLocation.LocationControl smartLocation = SmartLocation.with(this).location().config(builder.build());

        smartLocation.start(new OnLocationUpdatedListener() {
            @Override
            public void onLocationUpdated(Location location) {
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
                    CommonApplication.deviceClient.sendEventAsync(updateLocationMessage, null, null);
                } catch (Exception e ){
                    Log.d(TAG, "Failed to send event. " + e.getMessage());
                }
            }
        });
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        Log.d(TAG, "Retrieved permission results for requestCode " + requestCode);

        AccompanyLocationListener locationListener = new AccompanyLocationListener(CommonApplication.deviceClient);
        LocationManager locationManager = (LocationManager) getSystemService(Context.LOCATION_SERVICE);

        try {
            locationManager.requestLocationUpdates(LocationManager.GPS_PROVIDER, 0, 0, locationListener);
        } catch(SecurityException e) {
            Log.d(TAG, "Improper permissions for requesting location. " + e.getMessage());
        }
    }
}
