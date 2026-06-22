using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Interface
{
    public interface IServiceGeolocalisation : IDisposable
    {
        #region public properties
        public bool Pause { get; set; }
        #endregion
        #region public event 
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region public method
        public void StartLocationUpdatesAsync();
        public void StopLocationUpdates();
        public Task StartLocationUpdatesAsync(CancellationTokenSource cancellationTokenSource);

        /// <summary>
        /// method qui retourne le localisation Current
        /// </summary>
        /// <param name="cancellationTokenSource"></param>
        /// <returns></returns>
        public async Task<Location?> GetCurrentLocationAsync(CancellationTokenSource cancellationTokenSource)
        {
            GeolocationRequest request = new (GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));

            var location = await Geolocation.Default.GetLocationAsync(request, cancellationTokenSource.Token);
            return location;
        }

        #endregion

    }
}
