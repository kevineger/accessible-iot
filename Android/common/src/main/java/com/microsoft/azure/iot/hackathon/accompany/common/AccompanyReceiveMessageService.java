package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.mapbox.geojson.LineString;
import com.microsoft.azure.iot.hackathon.accompany.common.constants.AccompanyIntents;

import org.json.JSONObject;

public class AccompanyReceiveMessageService extends FirebaseMessagingService {
    private static final String TAG = "AccompanyReceiveMessage";

    private static final String ACCOMPANY_ACK = "ACCOMPANY_ACKNOWLEDGE";

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        Log.d(TAG, "From: " + remoteMessage.getFrom() + " Message: " + remoteMessage.getData());

        Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        v.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));

        String notificationType = remoteMessage.getData().get("Type");

        switch(notificationType) {
            case "5" :
                DisplayLineGeometry(remoteMessage);
                break;
            default:
                DisplayAckNotification(remoteMessage.getData().get("SourceDeviceId"));
        }
    }

    private void DisplayLineGeometry(RemoteMessage remoteMessage) {
        Log.d(TAG, "Showing directions on map.");

        String resourceUrl = remoteMessage.getData().get("ResourceUrl");

        RequestQueue requestQueue = Volley.newRequestQueue(this);
        String registrationService = BuildConfig.REGISTRATIONSERVICE_HOSTNAME;

        AccompanyReceiveMessageService service = this;

        JsonObjectRequest request = new JsonObjectRequest(Request.Method.GET, registrationService + resourceUrl, null,
                new Response.Listener<JSONObject>() {
                    @Override
                    public void onResponse(JSONObject response) {
                        Log.d(TAG, "Retrieved line geometry response. Response: " + response.toString());

                        try {
                            AccompanyMapData.lineGeom = LineString.fromJson(response.toString());
                            LocalBroadcastManager.getInstance(service).sendBroadcast(new Intent(AccompanyMapData.UPDATED));
                        } catch (Exception ex) {
                            Log.e(TAG, ex.getMessage());
                        }
                    }
                }, new Response.ErrorListener() {
                    @Override
                    public void onErrorResponse(VolleyError error) {
                        Log.e(TAG, "ERROR");
                    }
                });

        requestQueue.add(request);
    }

    private void DisplayAckNotification(String sourceDeviceId) {
        NotificationManager manager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        NotificationChannel channel = new NotificationChannel("default", "default", NotificationManager.IMPORTANCE_DEFAULT);
        channel.setDescription("Channel Description");

        // TODO: Depending on type of notification - display something different.


        Intent answerAckIntent = new Intent(this, AcknowledgeBroadcastReceiver.class);
        answerAckIntent.setAction(AccompanyIntents.ACK_INTENT);
        answerAckIntent.putExtra("NotificationId", 01);
        answerAckIntent.putExtra("SourceDeviceId", sourceDeviceId);

        PendingIntent pendingIntent = PendingIntent.getBroadcast(this, 1, answerAckIntent, PendingIntent.FLAG_ONE_SHOT);

        Intent denyAckIntent = new Intent(this, AcknowledgeBroadcastReceiver.class);
        denyAckIntent.setAction(AccompanyIntents.DENY_INTENT);
        denyAckIntent.putExtra("NotificationId", 01);

        PendingIntent pendingCloseIntent = PendingIntent.getBroadcast(this, 1, denyAckIntent, PendingIntent.FLAG_ONE_SHOT);

        manager.createNotificationChannel(channel);
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "default")
                .setSmallIcon(R.drawable.ic_launcher)
                .setContentTitle("Request for help")
                .setContentText("Acknowledge call for help.")
                .addAction(R.drawable.mapcontrol_point_circle_blue, "Yes, I can help", pendingIntent)
                .addAction(R.drawable.mapbox_info_icon_default, "Not right now", pendingCloseIntent)
                .setOngoing(true)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT);

        manager.notify(01, builder.build());
    }

    @Override
    public void onNewToken(String token) {
        Log.d(TAG,"Refreshed token: " + token);
    }
}
