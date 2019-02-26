package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.Application;
import android.content.Intent;
import android.provider.Settings;
import android.support.annotation.NonNull;
import android.util.Log;
import android.widget.Toast;

import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.JsonObjectRequest;
import com.android.volley.toolbox.Volley;
import com.google.android.gms.tasks.OnCompleteListener;
import com.google.android.gms.tasks.Task;
import com.google.firebase.FirebaseApp;
import com.google.firebase.iid.FirebaseInstanceId;
import com.google.firebase.iid.InstanceIdResult;
import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.analytics.Analytics;
import com.microsoft.appcenter.crashes.Crashes;
import com.microsoft.azure.maps.mapcontrol.AzureMaps;
import com.microsoft.azure.sdk.iot.device.DeviceClient;
import com.microsoft.azure.sdk.iot.device.IotHubClientProtocol;

import org.json.JSONException;
import org.json.JSONObject;

public class CommonApplication extends Application {

    private static final String TAG = "AccompanyCommon";

    private DeviceClient deviceClient;

    @Override
    public void onCreate() {
        super.onCreate();

        AppCenter.start(this, BuildConfig.APPCENTER_KEY, Analytics.class, Crashes.class);

        AzureMaps.setSubscriptionKey(BuildConfig.AZUREMAPS_KEY);

        // Register with DeviceRegistrationService.
        Register();
    }

    public void Register() {
        RequestQueue requestQueue = Volley.newRequestQueue(this);

        final String androidId = Settings.Secure.getString(getContentResolver(), Settings.Secure.ANDROID_ID);
        Log.d(TAG, "Android ID: " + androidId);
        JSONObject registrationObject = new JSONObject();
        try {
            registrationObject.put("deviceId", androidId);
            String registrationService = BuildConfig.REGISTRATIONSERVICE_HOSTNAME;
            JsonObjectRequest request = new JsonObjectRequest(registrationService + "/api/Registrations", registrationObject,
                    new Response.Listener<JSONObject>() {
                        @Override
                        public void onResponse(JSONObject response) {
                            try {
                                Log.d(TAG, "Successfully registered with the registration service.");
                                String connectionString = response.getString("connectionString");
                                String iothubHostname = BuildConfig.IOTHUB_HOSTNAME;
                                InitializeDeviceClient("HostName="+iothubHostname+";DeviceId=" + androidId + ";SharedAccessKey=" + connectionString);
                            } catch (Exception e) {
                                Log.e(TAG, "Error encountered while retrieving connection string to initialize device client. " + e.getMessage());
                            }
                        }
                    },
                    new Response.ErrorListener() {
                        @Override
                        public void onErrorResponse(VolleyError error) {
                            Log.e(TAG, "Error on registration. Error: " + error);
                        }
                    });

            requestQueue.add(request);
        } catch (JSONException jsonException) {
            Log.e(TAG, "Could not create registration object. Message: " + jsonException.getMessage() + " Stack:" + jsonException.getStackTrace());
        }

    }

    public void InitializeDeviceClient(String connectionString) {
        try {
            deviceClient = new DeviceClient(connectionString, IotHubClientProtocol.AMQPS);
            deviceClient.open();
        } catch (Exception e) {
            Log.e(TAG, "Could not open the device client. " + e.getMessage());
        }
    }
}
