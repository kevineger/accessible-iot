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

import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyLocationListener;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanyMapData;
import com.microsoft.azure.iot.hackathon.accompany.common.AccompanySensorListener;
import com.microsoft.azure.iot.hackathon.accompany.common.CommonApplication;
import com.microsoft.azure.iot.hackathon.accompany.common.MapActivity;
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

                    if (CommonApplication.deviceClient != null) {
                        CommonApplication.deviceClient.sendEventAsync(updateLocationMessage, null, null);
                    } else {
                        Log.d(TAG, "Device client is null.");
                    }
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

    private void updateMapData() {
        routeDataSource.clear();
        if (AccompanyMapData.lineGeom != null) {
            routeDataSource.add(AccompanyMapData.lineGeom);
        }

        destinationDataSource.clear();
        if (AccompanyMapData.destination != null) {
            destinationDataSource.add(AccompanyMapData.destination);
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
