using Android.Locations;
using KronoGeo_Maui.Applications.Interface;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;
using GeolocationMaui = Microsoft.Maui.Devices.Sensors.Geolocation;

namespace KronoGeo_Maui.Applications.Services.Geolocation
{
    /// <summary>
    /// implémentation de la geolocalisation pour les autres systemes
    /// </summary>
    public class GeolocationOther : IServiceGeolocalisation
    {
        #region public properties
        public bool Pause { get; set; } = false;
        #endregion

        #region public event
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region public method interface
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }


        public async Task StartLocationUpdatesAsync(CancellationToken cancellationToken)
        {
            if (Pause) return;
            // Using GeolocationAccuracy.Best
            // Developers can adjust this value to High or Low based on their specific requirements.
            var request = new GeolocationListeningRequest(GeolocationAccuracy.Best);
            var success = await GeolocationMaui.StartListeningForegroundAsync(request);

            if (success)
            {
                GeolocationMaui.LocationChanged += LocationChanged;
            }
        }

        public void StopLocationUpdates()
        {
            GeolocationMaui.LocationChanged -= LocationChanged;
            GeolocationMaui.StopListeningForeground();
        }

        void IServiceGeolocalisation.StartLocationUpdatesAsync()
        {
            var cancellationToken = new CancellationTokenSource();
            StartLocationUpdatesAsync(cancellationToken.Token).Start();
        }
        #endregion
    }
}
