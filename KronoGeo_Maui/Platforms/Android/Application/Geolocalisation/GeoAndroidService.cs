using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;


namespace KronoGeo_Maui.Platforms.Android.Application.Geolocalisation
{
    //[Service(ForegroundServiceType = Android.Content.PM.ForegroundService.TypeLocation)]
    [Service]
    public class GeoAndroidService(IServiceGeolocalisation serviceGeo) : Service
    {
        #region private properties
        private const string NOTIFICATION_CHANNEL_ID = "46100";
        private readonly int NOTIFICATION_ID = 1;
        private const string CHANNEL_NAME = "location_notification_channel";
        private readonly IServiceGeolocalisation _serviceGeo = serviceGeo;
        private readonly CancellationTokenSource _cancellationTokenSource = new ();
        #endregion

        #region public const properties
        public const string ActionStart = "Start_Geolocation";
        public const string ActionPause = "Pause_Geolocation";
        public const string ActionStopPause = "StopPause_Geolocation";
        public const string ActionStop = "Stop_Geolocation";
        #endregion

        #region public method override
        public override IBinder? OnBind(Intent? intent) => null;
        #endregion

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            

            var action = intent?.Action;
            switch (action)
            {
                case ActionStart:
                    // 3. Démarrer le service en mode "Foreground"
                    // Depuis Android 14, il faut impérativement spécifier le type de service ici aussi
                    StartForegroundService();

                    // 4. C'est ICI que tu lances ta logique de géolocalisation
                    // (ex: un timer ou un abonnement au GPS qui enregistre tes points)
                    StartGeolocalisation();
                    break;
                case ActionPause:
                    Pause();
                    break;
                case ActionStopPause:
                    StopPause();
                    break;
                case ActionStop:
                    OnDestroy();
                    break;
            }

            return StartCommandResult.Sticky;
        }
        
        /// <summary>
        /// Arrêter le service et libérer les ressources
        /// </summary>
        public override void OnDestroy()
        {
            // Arrêter proprement le GPS ici pour économiser la batterie
            _serviceGeo.StopLocationUpdates();
            _serviceGeo.Dispose();
            base.OnDestroy();
        }

        #region private method
        
        /// <summary>
        /// Pour mettre en pause le service de géolocalisation,
        /// par exemple lorsque l'utilisateur met l'application en arrière-plan
        /// </summary>
        private void Pause()
        {
            // -- arrêt des écoutes sur onchanged pour ne pas envoyer de message
            // à l'application MAUI
            _serviceGeo.LocationChanged -= OnLocalication_Changed;
            _serviceGeo.Pause = true;
        }

        /// <summary>
        /// Pour mettre en pause le service de géolocalisation,
        /// par exemple lorsque l'utilisateur met l'application en arrière-plan
        /// </summary>
        private void StopPause()
        {
            // -- arrêt des écoutes sur onchanged pour ne pas envoyer de message
            // à l'application MAUI
            _serviceGeo.LocationChanged += OnLocalication_Changed;
            _serviceGeo.Pause = false;
        }

        /// <summary>
        /// Demarre la géolocation grace au service de geolocation d'android 
        /// et envoie les messages de changement de localisation à l'application MAUI
        /// avec un message utilisation de community toolkit MVVM messenger
        /// </summary>
        private void StartGeolocalisation()
        {
            if ( !_serviceGeo.Pause)
            {
                _serviceGeo.StartLocationUpdatesAsync(_cancellationTokenSource.Token);
            }

            _serviceGeo.LocationChanged += OnLocalication_Changed;
            _serviceGeo.Pause = false;
        }


        /// <summary>
        /// création du canal de notification pour le service foreground
        /// </summary>
        /// <param name="notificationMnaManager"></param>
        private static void CreateNotificationChannel(NotificationManager notificationMnaManager)
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

        /// <summary>
        /// Démarre le service en mode "Foreground" avec une notification persistante
        /// </summary>
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

        #region public method event
        /// <summary>
        /// Envoyer le message de changement de localisation à l'application MAUI
        /// avec Messager de CommunityToolkit MVVM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLocalication_Changed(object? sender, GeolocationLocationChangedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new LocationChangedMessage(e.Location));
        }
        #endregion
    }
}
