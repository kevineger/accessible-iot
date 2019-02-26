package com.microsoft.azure.iot.hackathon.accompany.providerapp;

import android.os.Bundle;

import com.microsoft.azure.iot.hackathon.accompany.common.MapActivity;

public class MainActivity extends MapActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        mapControl = findViewById(R.id.mapControl);
        mapControl.onCreate(savedInstanceState);
    }
}
