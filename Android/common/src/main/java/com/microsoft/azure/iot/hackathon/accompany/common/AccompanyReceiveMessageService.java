package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.content.Context;
import android.content.Intent;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.mapbox.geojson.LineString;

import org.json.JSONObject;

public class AccompanyReceiveMessageService extends FirebaseMessagingService {
    private static final String TAG = "AccompanyReceiveMessage";

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        Log.d(TAG, "From: " + remoteMessage.getFrom() + " Message: " + remoteMessage.getData());

        Vibrator v = (Vibrator) getSystemService(Context.VIBRATOR_SERVICE);
        v.vibrate(VibrationEffect.createOneShot(500, VibrationEffect.DEFAULT_AMPLITUDE));

        String notificationType = remoteMessage.getData().get("Type");

        switch(notificationType) {
            case "5" :
                Log.d(TAG, "Showing directions on map.");
                String lineGeom = remoteMessage.getData().get("LineGeometry");
                Log.d(TAG, "Line geometry: " + lineGeom);
                try {
                    AccompanyMapData.lineGeom = LineString.fromJson(lineGeom);
                    LocalBroadcastManager.getInstance(this).sendBroadcast(new Intent(AccompanyMapData.UPDATED));
                } catch (Exception ex) {
                    Log.e(TAG, ex.getMessage());
                }
                break;
            default:
                Log.d(TAG, "Other message");
                DisplayAckNotification();
        }
    }

    private void DisplayAckNotification() {
        NotificationManager manager = (NotificationManager) getSystemService(Context.NOTIFICATION_SERVICE);

        NotificationChannel channel = new NotificationChannel("default", "default", NotificationManager.IMPORTANCE_DEFAULT);
        channel.setDescription("Channel Description");

        // TODO: Depending on type of notification - display something different.

        manager.createNotificationChannel(channel);
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, "default")
                .setSmallIcon(R.drawable.ic_launcher)
                .setContentTitle("Request for help")
                .setContentText("Acknowledge call for help.")
                .addAction(R.drawable.mapcontrol_point_circle_blue, "Yes, I can help", null)
                .addAction(R.drawable.mapbox_info_icon_default, "Not right now", null)
                .setOngoing(true)
                .setPriority(NotificationCompat.PRIORITY_DEFAULT);

        manager.notify(01, builder.build());
    }

    @Override
    public void onNewToken(String token) {
        Log.d(TAG,"Refreshed token: " + token);
    }
}
