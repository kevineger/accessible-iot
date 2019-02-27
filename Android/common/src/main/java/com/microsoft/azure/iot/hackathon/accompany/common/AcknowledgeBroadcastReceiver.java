package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.NotificationManager;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;
import android.util.Log;
import android.widget.Toast;

import com.android.volley.RequestQueue;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.gson.Gson;
import com.microsoft.azure.iot.hackathon.accompany.common.constants.AccompanyIntents;
import com.microsoft.azure.iot.hackathon.accompany.common.models.AccompanyLocation;
import com.microsoft.azure.iot.hackathon.accompany.common.models.AcknowledgeRequest;

import org.json.JSONException;
import org.json.JSONObject;

public class AcknowledgeBroadcastReceiver extends BroadcastReceiver {

    private static final String TAG = "AccompanyBroadcastReceiver";

    @Override
    public void onReceive(Context context, Intent intent) {
        Log.d("Accompany","Received intent!" + intent.getAction());

        NotificationManager manager = (NotificationManager) context.getSystemService(Context.NOTIFICATION_SERVICE);

        switch(intent.getAction()) {
            case AccompanyIntents.ACK_INTENT:
                try {
                    Acknowledge(context, intent);
                } catch (JSONException e) {
                    e.printStackTrace();
                }
                break;
            default:
                break;
        }

        int notificationId = intent.getIntExtra("NotificationId", 0);
        manager.cancel(notificationId);
    }

    private void Acknowledge(Context context, Intent intent) throws JSONException {
        String sourceDeviceId = intent.getStringExtra("SourceDeviceId");

        RequestQueue requestQueue = Volley.newRequestQueue(context);
        String hostname = BuildConfig.ACKSERVICE_HOSTNAME;

        AcknowledgeRequest ackRequest = new AcknowledgeRequest();
        ackRequest.careTakerUserId = CommonApplication.AndroidId;
        ackRequest.careRecipientId = sourceDeviceId;
        ackRequest.action = "ProvideHelp";
        ackRequest.careRecipientLocation = new AccompanyLocation(42, 42);
        ackRequest.careTakerLocation = new AccompanyLocation(42, 42);

        String gsonData = new Gson().toJson(ackRequest);
        JSONObject acknowledgeObject = new JSONObject(gsonData);

        JsonObjectRequest request = new JsonObjectRequest(hostname + "/api/HttpTriggerAcknowledge", acknowledgeObject,
                response -> {
                    Log.d(TAG,"Acknowledged successfully.");
                    Toast.makeText(context, "Acknowledged. Generating Route.", Toast.LENGTH_SHORT).show();
                },
                error -> {
                    Log.d(TAG, "Already acknowledged. " + error);
                    Toast.makeText(context, "Already acknowledged.", Toast.LENGTH_SHORT).show();
                });

        requestQueue.add(request);
    }
}
