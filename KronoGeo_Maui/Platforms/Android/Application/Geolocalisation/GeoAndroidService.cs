using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;


namespace KronoGeo_Maui.Platforms.Android.Application.Geolocalisation
{
    //[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation)]
    [Service]
    public class GeoAndroidService : Service
    {
        #region private properties
        private const string NOTIFICATION_CHANNEL_ID = "46100";
        private int NOTIFICATION_ID = 1;
        private const string CHANNEL_NAME = "location_notification_channel";
        #endregion

        #region public method override
        public override IBinder? OnBind(Intent? intent) => null;
        #endregion

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            // 3. Démarrer le service en mode "Foreground"
            // Depuis Android 14, il faut impérativement spécifier le type de service ici aussi
            StartForegroundService();

            // 4. C'est ICI que tu lances ta logique de géolocalisation
            // (ex: un timer ou un abonnement au GPS qui enregistre tes points)
            //DemarrerGeolocalisation();

            return StartCommandResult.Sticky;
        }

       /* private void DemarrerGeolocalisation()
        {
            // Ton code C# pour écouter le GPS (Geolocator, MAUI Geolocation, etc.)
        }*/

        public override void OnDestroy()
        {
            // Arrêter proprement le GPS ici pour économiser la batterie
            base.OnDestroy();
        }

        #region private method
        /// <summary>
        /// création du canal de notification pour le service foreground
        /// </summary>
        /// <param name="notificationMnaManager"></param>
        private void CreateNotificationChannel(NotificationManager notificationMnaManager)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                if (notificationMnaManager.GetNotificationChannel(NOTIFICATION_CHANNEL_ID) != null)
                {
                    return; // Le canal existe déjà
                }

                var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Low);
                notificationMnaManager.CreateNotificationChannel(channel);
            }
        }

        private void StartForegroundService()
        {
            var notifcationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            // 1. Créer le canal de notification (obligatoire pour Android 8+)

            if (notifcationManager is not null && OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                CreateNotificationChannel(notifcationManager);
            }

            // 2. Créer la notification qui sera visible par l'utilisateur
            var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID);
            notification.SetAutoCancel(false);
            notification.SetOngoing(true);
            notification.SetSmallIcon(global::Android.Resource.Drawable.IcMenuCompass); // Change l'icône selon tes besoins
            notification.SetContentTitle("Suivi GPS actif");
            notification.SetContentText("Votre position est enregistrée en arrière-plan.");

            StartForeground(NOTIFICATION_ID, notification.Build());

        }
        #endregion
    }
}
