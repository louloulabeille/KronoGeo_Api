using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Interface
{
    public interface IServiceGeolocalisation : IDisposable
    {
        #region public properties
        public CancellationTokenSource CancellationTokenSource { get; set; }
        public bool Pause { get; set; }
        #endregion
        #region public event 
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region public method
        public void StartLocationUpdates();
        public void StopLocationUpdates();
        #endregion

    }
}
