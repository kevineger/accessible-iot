package com.microsoft.azure.iot.hackathon.accompany.consumerapp;

import android.Manifest;
import android.content.Context;
import android.content.Intent;
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

public class MainActivity extends MapActivity {

    private static final String TAG = "AccompanyConsumer";

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mapControl = findViewById(R.id.mapControl);
        mapControl.onCreate(savedInstanceState);
        FirebaseApp.initializeApp(this);
        FirebaseInstanceId.getInstance().getInstanceId()
                .addOnCompleteListener(new OnCompleteListener<InstanceIdResult>() {
                    @Override
                    public void onComplete(@NonNull Task<InstanceIdResult> task) {
                        if (!task.isSuccessful()) {
                            Log.w(TAG, "getInstanceId failed", task.getException());
                            return;
                        }

                        // Get new Instance ID token
                        String token = task.getResult().getToken();

                        // Log and toast

                        Log.d(TAG, token);
                        Toast.makeText(MainActivity.this, token, Toast.LENGTH_SHORT).show();
                    }
                });
        Intent intent = new Intent(this, AccompanyRegistrationIntentService.class);
        startService(intent);

        requestPermissions(new String[] { Manifest.permission.ACCESS_FINE_LOCATION }, 1340);
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
