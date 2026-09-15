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
        //public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region public method
        public void StartLocationUpdatesAsync();
        public void StopLocationUpdates();
        /// <summary>
        /// Démarre la récupération de la localisation en tâche de fond avec un CancellationToken
        /// Attention avec les Thread et les Task.Run, il faut faire attention à ne pas bloquer le thread principal 
        /// et à gérer correctement les exceptions. 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public virtual Task StartLocationUpdatesAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                StartLocationUpdatesAsync();
            }, cancellationToken);
        }

        /// <summary>
        /// method qui retourne le localisation Current
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task<Location?> GetCurrentLocationAsync(CancellationToken token);
        /*{
            GeolocationRequest request = new (GeolocationAccuracy.Best, TimeSpan.FromSeconds(1));

            var location = await Geolocation.Default.GetLocationAsync(request, cancellationTokenSource.Token);
            return location;
        }*/

        #endregion

    }
}
