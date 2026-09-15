using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class GestionPermissions : IServicePermissions
    {
        /// <summary>
        /// methode de gestion des permissions de localisation
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetLocalisationPermissionAsync()
        {
            // -- gestion des permissions de geolocalisation 
            PermissionStatus status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            //else if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS)
            //{
            //    ActiveWindowsReglage();
            //}
            ActiveWindowsReglage();
            return false;

        }

        /// <summary>
        /// methode de gestion des permissions pour la notification
        /// </summary>
        /// <returns></returns>
        public async Task<bool> GetNotificationPermissionAsync()
        {
            //var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            PermissionStatus status = await Permissions.RequestAsync<Permissions.PostNotifications>();
            if (status == PermissionStatus.Granted)
            {
                return true;
            }
            //else if ( status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS )
            //{
            //    ActiveWindowsReglage();
            //}
            ActiveWindowsReglage();
            return false;
        }

        public Task<bool> GetPhotoPermissionAsync()
        {
            throw new NotImplementedException();
        }

        #region private method
        /// <summary>
        /// ouvre la fenêtre de réglage selon IOS ou Android
        /// </summary>
        private static void ActiveWindowsReglage ()
        {
            // -- traitement qui peut arriver si la personne a refusée une première fois impossible
            // -- de rappeler la fenêtre de demande de géolocalisation ouverture de la fenêtre de réglage
            AppInfo.Current.ShowSettingsUI();
        }

#endregion
    }
}
