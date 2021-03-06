package com.microsoft.azure.iot.hackathon.accompany.consumerapp;

import android.Manifest;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Color;
import android.hardware.Sensor;
import android.hardware.SensorManager;
import android.location.Location;
import android.location.LocationManager;
import android.os.Bundle;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.google.gson.Gson;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyLocationListener;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyMapData;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanySensorListener;
import com.microsoft.azure.iot.hackathon.accompany.common.CommonApplication;
import com.microsoft.azure.iot.hackathon.accompany.common.MapActivity;
import com.microsoft.azure.iot.hackathon.accompany.common.models.AccompanyLocation;
import com.microsoft.azure.iot.hackathon.accompany.common.models.UpdateLocationRequest;
import com.microsoft.azure.maps.mapcontrol.layer.LineLayer;
import com.microsoft.azure.maps.mapcontrol.layer.SymbolLayer;
import com.microsoft.azure.maps.mapcontrol.source.DataSource;
import com.microsoft.azure.sdk.iot.device.Message;

import org.json.JSONObject;

import io.nlopez.smartlocation.OnLocationUpdatedListener;
import io.nlopez.smartlocation.SmartLocation;
import io.nlopez.smartlocation.location.config.LocationAccuracy;
import io.nlopez.smartlocation.location.config.LocationParams;

import static com.microsoft.azure.maps.mapcontrol.options.LineLayerOptions.strokeColor;
import static com.microsoft.azure.maps.mapcontrol.options.LineLayerOptions.strokeWidth;
import static com.microsoft.azure.maps.mapcontrol.options.SymbolLayerOptions.iconImage;

public class MainActivity extends MapActivity {

    private static final String TAG = "AccompanyConsumer";

    DataSource routeDataSource = new DataSource();
    LineLayer routeLineLayer = new LineLayer(routeDataSource, strokeColor(Color.parseColor("#e55e5e")), strokeWidth(5f));

    DataSource destinationDataSource = new DataSource();
    SymbolLayer destinationSymbolLayer = new SymbolLayer(destinationDataSource, iconImage("dest-icon"));

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mapControl = findViewById(R.id.mapControl);
        mapControl.onCreate(savedInstanceState);
        mapControl.getMapAsync(map -> {
            map.images.add("dest-icon", R.drawable.mapcontrol_marker_red);
            map.sources.add(routeDataSource);
            map.sources.add(destinationDataSource);
            map.layers.add(destinationSymbolLayer);
            map.layers.add(routeLineLayer);
        });

        SensorManager sensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        sensorManager.registerListener(new AccompanySensorListener(getApplicationContext()), sensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER), SensorManager.SENSOR_DELAY_NORMAL);

        requestPermissions(new String[] { Manifest.permission.ACCESS_FINE_LOCATION }, 1340);

        LocationParams.Builder builder = new LocationParams.Builder()
                .setAccuracy(LocationAccuracy.HIGH)
                .setInterval(100);

        SmartLocation.LocationControl smartLocation = SmartLocation.with(this).location().config(builder.build());

        smartLocation.start(location -> {
            Log.d(TAG, "Latitude: " + location.getLatitude() + " Longitude: " + location.getLongitude());

            try {
                UpdateLocationRequest updateLocationRequest = new UpdateLocationRequest();
                updateLocationRequest.userId = CommonApplication.AndroidId;

                // TODO: This value depends on provider or consumer app.
                updateLocationRequest.userType = "CareRecipient";

                AccompanyLocation locationObj = new AccompanyLocation(location.getLatitude(), location.getLongitude());
                updateLocationRequest.location = locationObj;

                String data = new Gson().toJson(updateLocationRequest);

                Message updateLocationMessage = new Message(data);

                if (CommonApplication.deviceClient != null) {
                    CommonApplication.deviceClient.sendEventAsync(updateLocationMessage, null, null);
                } else {
                    Log.d(TAG, "Device client is null.");
                }
            } catch (Exception e ){
                Log.d(TAG, "Failed to send event. " + e.getMessage());
            }
        });
    }

    @Override
    public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
        Log.d(TAG, "Retrieved permission results for requestCode " + requestCode);
    }

    private void updateMapData() {
        Log.d(TAG, "Updating map data.");
        routeDataSource.clear();
        if (AccompanyMapData.lineGeom != null) {
            routeDataSource.add(AccompanyMapData.lineGeom);
        }

        destinationDataSource.clear();
        if (AccompanyMapData.destination != null) {
            destinationDataSource.add(AccompanyMapData.destination);
        }

        if (AccompanyMapData.destinations != null) {
            destinationDataSource.add(AccompanyMapData.destinations);
        }
    }

    private BroadcastReceiver updateReceiver = new BroadcastReceiver() {
        @Override
        public void onReceive(Context context, Intent intent) {
            updateMapData();
        }
    };

    @Override
    public void onResume() {
        super.onResume();
        updateMapData();
        LocalBroadcastManager.getInstance(this).registerReceiver(updateReceiver, new IntentFilter(AccompanyMapData.UPDATED));
    }

    @Override
    public void onPause() {
        super.onPause();
        LocalBroadcastManager.getInstance(this).unregisterReceiver(updateReceiver);
    }


}
