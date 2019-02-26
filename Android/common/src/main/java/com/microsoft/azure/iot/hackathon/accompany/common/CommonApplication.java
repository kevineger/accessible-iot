package com.microsoft.azure.iot.hackathon.accompany.common;

import android.app.Application;

import com.microsoft.appcenter.AppCenter;
import com.microsoft.appcenter.analytics.Analytics;
import com.microsoft.appcenter.crashes.Crashes;
import com.microsoft.azure.maps.mapcontrol.AzureMaps;

public class CommonApplication extends Application {

    @Override
    public void onCreate() {
        super.onCreate();

        AppCenter.start(this, BuildConfig.APPCENTER_KEY, Analytics.class, Crashes.class);

        AzureMaps.setSubscriptionKey(BuildConfig.AZUREMAPS_KEY);
    }
}
