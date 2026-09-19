#if ANDROID
using Android.Content;
using Android.OS;
#endif

using KronoGeo_Maui.Applications.Interface;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Api.Models.Infrastructure.Options;

namespace KronoGeo_Maui.Applications.Services
{
    public class GestionPermissions (IOptions<PackageNameAndroid> options ) : IServicePermissions
    {
        #region private properties
        private readonly IOptions<PackageNameAndroid> _options = options;
        #endregion

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

        /// <summary>
        /// méthode qui ouvre une fenêtre de la gestion de la batterie pour l'application
        /// il faut ajouter au niveau du manifest cette ligne 
        /// </summary>
        public void GestionBatterieAsync()
        {
            //< uses - permission android: name = "android.permission.REQUEST_IGNORE_BATTERY_OPTIMIZATIONS" />
#if ANDROID
            Intent intent = new ();
            string? packageName = Android.App.Application.Context.PackageName;
            PowerManager? pm = Android.App.Application.Context.GetSystemService(Context.PowerService) as PowerManager;

            if ( pm is not null && !pm.IsIgnoringBatteryOptimizations(packageName))
            {
                intent.SetAction(Android.Provider.Settings.ActionRequestIgnoreBatteryOptimizations);
                intent.SetData(Android.Net.Uri.Parse("package:" + packageName));
                Android.App.Application.Context.StartActivity(intent);
            }
#endif
        }

        #endregion
    }
}
