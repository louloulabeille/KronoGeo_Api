using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services.Geolocation
{
    /// <summary>
    /// implémentation de la geolocalisation pour les autres systemes
    /// </summary>
    public class GeolocationOther : IServiceGeolocalisation
    {
        public CancellationTokenSource CancellationTokenSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Pause { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void StartLocationUpdates()
        {
            throw new NotImplementedException();
        }

        public void StopLocationUpdates()
        {
            throw new NotImplementedException();
        }
    }
}
