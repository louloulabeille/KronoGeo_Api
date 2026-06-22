using Android.Locations;
using Android.OS;
using Android.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using Location = Android.Locations.Location;

namespace KronoGeo_Maui.Platforms.Android.Application.Geolocalisation
{
    public class LocationListener : Java.Lang.Object, ILocationListener
    {

        #region event appelé lors du changemeent de la localisation
        // Action à appeler lorsque la localisation change - event
        public Action<Location>? OnLocationChangedAction { get; set; }
        #endregion

        #region public method interface
        /// <summary>
        /// method qui est appelé a chaque changement de localisation
        /// </summary>
        /// <param name="location"></param>
        public void OnLocationChanged(global::Android.Locations.Location location)
        {
            if (location is not null)
            {
                OnLocationChangedAction?.Invoke(location);
            }
        }

        /// <summary>
        /// method qui est appelé lors de la desactivation du systeme de géolocalisation
        /// </summary>
        /// <param name="provider"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnProviderDisabled(string provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// méthod qui est appelé lors de l'activatiin du système de géolocation
        /// </summary>
        /// <param name="provider"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnProviderEnabled(string provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Obsolète n'est plus utilisé dans les dernières versions d'Android
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="status"></param>
        /// <param name="extras"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnStatusChanged(string? provider, [GeneratedEnum] Availability status, Bundle? extras)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
