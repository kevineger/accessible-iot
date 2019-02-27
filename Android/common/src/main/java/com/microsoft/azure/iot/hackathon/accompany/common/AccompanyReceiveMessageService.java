package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.support.v4.app.NotificationCompat;
import android.support.v4.content.LocalBroadcastManager;
import android.util.Log;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.firebase.messaging.FirebaseMessagingService;
import com.google.firebase.messaging.RemoteMessage;
import com.google.gson.Gson;
import com.mapbox.geojson.Geometry;
import com.mapbox.geojson.GeometryCollection;
import com.mapbox.geojson.LineString;
import com.mapbox.geojson.Point;
import com.microsoft.azure.iot.hackathon.accompany.common.constants.AccompanyIntents;
import com.microsoft.azure.iot.hackathon.accompany.common.models.LineGeometry;

import java.util.ArrayList;
import java.util.List;

public class AccompanyReceiveMessageService extends FirebaseMessagingService {
    private static final String TAG = "AccompanyReceiveMessage";

    @Override
    public void onMessageReceived(RemoteMessage remoteMessage) {
        Log.d(TAG, "From: " + remoteMessage.getFrom() + " Message: " + remoteMessage.getData());

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
                response -> {
                    Log.d(TAG, "Retrieved line geometry response. Response: " + response.toString());
                    LineGeometry lineGeometry = new Gson().fromJson(response.toString(), LineGeometry.class);

                    try {
                        List<Point> pointList = new ArrayList<Point>();
                        pointList.add(Point.fromLngLat(-122.1335814,47.6366275));
                        pointList.add(Point.fromLngLat(-122.33, 47.64));

                        Point destPoint = Point.fromLngLat(-122.1335814,47.6366275);

                        //AccompanyMapData.lineGeom = LineString.fromLngLats(pointList);
                        AccompanyMapData.destination = destPoint;

                        Log.d(TAG, "GEOMETRY: " + lineGeometry + " SIZE: " + lineGeometry.coordinates.size());

                        List<Geometry> pointCollection = new ArrayList<Geometry>();
                        for(int x = 0; x < lineGeometry.coordinates.size(); x++) {
                            for(int y = 0; y < lineGeometry.coordinates.get(x).size(); y++) {
                                try {
                                    Log.d(TAG, "Point X: " + lineGeometry.coordinates.get(x).get(y) + " Point Y: " + lineGeometry.coordinates.get(x).get(y+1));
                                    pointCollection.add(Point.fromLngLat(lineGeometry.coordinates.get(x).get(y+1), lineGeometry.coordinates.get(x).get(y)));
                                } catch(Exception e) {

                                }
                            }
                        }

                        pointCollection.add(LineString.fromLngLats(pointList));

                        AccompanyMapData.destinations = GeometryCollection.fromGeometries(pointCollection);

                        //AccompanyMapData.lineGeom = LineString.fromJson(response.toString());
                        LocalBroadcastManager.getInstance(service).sendBroadcast(new Intent(AccompanyMapData.UPDATED));
                    } catch (Exception ex) {
                        Log.e(TAG, ex.getMessage());
                    }
                }, error -> Log.e(TAG, "Error retrieving resource geometry to display pedestrian path on map control. VolleyError: " + error));

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
