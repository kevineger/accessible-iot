package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;

import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.android.gms.common.internal.service.Common;

import org.json.JSONException;
import org.json.JSONObject;

import javax.json.JsonObject;

public class AcknowledgeBroadcastReceiver extends BroadcastReceiver {

    @Override
    public void onReceive(Context context, Intent intent) {
        Log.d("Accompany","Received intent!" + intent.getAction());

        NotificationManager manager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        switch(intent.getAction()) {
            case "ACK_INTENT":
                Acknowledge(context, intent);
                break;
            default:
                break;
        }

        int notificationId = intent.getIntExtra("NotificationId", 0);
        manager.cancel(notificationId);
    }

    private void Acknowledge(Context context, Intent intent) {

        String sourceDeviceId = intent.getStringExtra("SourceDeviceId");

        RequestQueue requestQueue = Volley.newRequestQueue(context);
        String hostname = BuildConfig.ACKSERVICE_HOSTNAME;

        JSONObject acknowledgeObject = new JSONObject();
        try {
            acknowledgeObject.put("careTakerUserId", CommonApplication.AndroidId);
            acknowledgeObject.put("careRecipientId", sourceDeviceId);
            acknowledgeObject.put("action", "ProvideHelp");

            JSONObject locationObject = new JSONObject();
            locationObject.put("lat", 42);
            locationObject.put("long", 42);

            acknowledgeObject.put("careTakerLocation", locationObject);
            acknowledgeObject.put("careRecipientLocation", locationObject);
        } catch(JSONException e) {

        }

        JsonObjectRequest request = new JsonObjectRequest(hostname + "/api/HttpTriggerAcknowledge", acknowledgeObject,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        Log.d("Accompany","Acknowledged successfully.");
                    }
                },
                new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Log.d("Accompany", "Already acknowledged.");
                    }
                });

        requestQueue.add(request);
    }
}
