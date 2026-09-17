#if ANDROID
using Android.Locations;
#endif
using KronoGeo_Maui.Applications.Interface;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;
using GeolocationMaui = Microsoft.Maui.Devices.Sensors.Geolocation;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace KronoGeo_Maui.Applications.Services.Geolocation
{
    /// <summary>
    /// implémentation de la geolocalisation pour les autres systemes
    /// </summary>
    public partial class GeolocationOther : IServiceGeolocalisation
    {
        #region public properties

        public bool Pause { get; set; } = false;
        #endregion

        #region public event
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        //public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
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

        public async Task StopLocationUpdatesAsync()
        {
            GeolocationMaui.LocationChanged -= LocationChanged;
            
            await Task.Run(()=> GeolocationMaui.StopListeningForeground());
            
        }

        public async Task StartLocationUpdatesAsync()
        {
            CancellationToken token = new ();
            await Task.Run( () => StartLocationUpdatesAsync(token));
        }

        public async Task<Location?> GetCurrentLocationAsync(CancellationToken token)
        {
            GeolocationRequest request = new(GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));

            var location = await GeolocationMaui.Default.GetLocationAsync(request, token);
            return location;
        }

        #endregion
    }
}
