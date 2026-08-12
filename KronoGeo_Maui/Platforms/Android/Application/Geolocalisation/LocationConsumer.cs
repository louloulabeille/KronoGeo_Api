using Java.Util.Functions;
using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;

namespace KronoGeo_Maui.Platforms.Android.Application.Geolocalisation
{
    internal class LocationConsumer : Java.Lang.Object, IConsumer
    {
        private readonly Action<global::Android.Locations.Location> _onLocationReceived;

        public LocationConsumer(Action<global::Android.Locations.Location> onLocationReceived)
        {
            _onLocationReceived = onLocationReceived;
        }

        public void Accept(Java.Lang.Object? obj)
        {
            // Cast the Java object to a Location and call the provided callback
            if (obj is global::Android.Locations.Location location)
            {
                _onLocationReceived?.Invoke(location);
            }
        }
    }
}
