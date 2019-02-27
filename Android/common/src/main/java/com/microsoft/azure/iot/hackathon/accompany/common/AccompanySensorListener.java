package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.Activity;
import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.util.Log;
import android.widget.Toast;

import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.gson.Gson;
import com.microsoft.azure.iot.hackathon.accompany.common.models.AccompanyLocation;
import com.microsoft.azure.iot.hackathon.accompany.common.models.HelpRequest;

import org.joda.time.DateTime;
import org.json.JSONException;
import org.json.JSONObject;

public class AccompanySensorListener implements SensorEventListener {
    private static final String TAG = "AccompanySensorListener";

    private float mAccel = 0.00f; // acceleration apart from gravity
    private float mAccelCurrent = SensorManager.GRAVITY_EARTH; // current acceleration including gravity
    private float mAccelLast = SensorManager.GRAVITY_EARTH; // last acceleration including gravity

    private boolean helpRequested = false;
    private DateTime helpRequestedTime;

    private Context context;

    public AccompanySensorListener(Context context) {
        this.context = context;
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        float x = event.values[0];
        float y = event.values[1];
        float z = event.values[2];
        mAccelLast = mAccelCurrent;
        mAccelCurrent = (float) Math.sqrt((double) (x*x + y*y + z*z));
        float delta = mAccelCurrent - mAccelLast;
        mAccel = mAccel * 0.9f + delta; // perform low-cut filter

        if(mAccel > 25) {
            try {
                if(!helpRequested) {
                    helpRequested = true;
                    helpRequestedTime = DateTime.now();
                    RequestHelp();
                } else {
                    if(helpRequestedTime.plusSeconds(30).isBeforeNow()) {
                        helpRequested = false;
                    }
                }
            } catch (Exception e) {
                Log.d(TAG, "Error encountered: " + e);
            }
        }
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {

    }

    private void RequestHelp() throws JSONException {
        RequestQueue requestQueue = Volley.newRequestQueue(context);
        String hostname = BuildConfig.ACKSERVICE_HOSTNAME;

        HelpRequest helpRequest = new HelpRequest();
        helpRequest.userId = CommonApplication.AndroidId;
        helpRequest.userType = "Assistant";
        helpRequest.location = new AccompanyLocation(42, 42);

        String gsonData = new Gson().toJson(helpRequest);
        JSONObject helpRequestObject = new JSONObject(gsonData);

        JsonObjectRequest request = new JsonObjectRequest(hostname + "/api/HttpTriggerGetAssistants", helpRequestObject,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        Log.d(TAG,"Help requested");
                        Toast.makeText(context, "Help requested.", Toast.LENGTH_SHORT).show();
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Log.d(TAG, "Error while requesting help.");
                        Toast.makeText(context, "Issue with requesting help.", Toast.LENGTH_SHORT).show();
                    }
                });

        requestQueue.add(request);
    }
}
