using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using Android.Util;
using Android.Content.PM;
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
    [Service(ForegroundServiceType = ForegroundService.TypeLocation)]
    //[Service]
    public class GeoAndroidService : Service, IServiceForegroundService
    {
        #region private properties
        private const string NOTIFICATION_CHANNEL_ID = "46100";
        private readonly int NOTIFICATION_ID = 1;
        private const string CHANNEL_NAME = "location_notification_channel";
        private IServiceGeolocalisation? _serviceGeo;
        private readonly CancellationTokenSource _cancellationTokenSource = new ();
        #endregion

        #region public const properties
        public const string ActionStart = "Start_Geolocation";
        public const string ActionPause = "Pause_Geolocation";
        public const string ActionStopPause = "StopPause_Geolocation";
        public const string ActionStop = "Stop_Geolocation";

        #endregion
        public GeoAndroidService() : base()
        {
            _serviceGeo?.LocationChanged += OnLocalicationChanged;
        }
        
        #region public method override
        public override IBinder? OnBind(Intent? intent) => null;

        public override void OnCreate()
        {
            base.OnCreate();
            _serviceGeo = IPlatformApplication.Current?.Services.GetService<IServiceGeolocalisation>();

            if( _serviceGeo is null )
            {
                // Sécurité au cas où le service de géolocalisation n'est pas disponible
                Log.Error("GeoAndroidService", "Le service de géolocalisation n'a pas pu être récupéré.");
            }
        }
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
            _serviceGeo?.StopLocationUpdates();
            _serviceGeo?.Dispose();
            if (OperatingSystem.IsAndroidVersionAtLeast(24))
            {
                StopForeground(StopForegroundFlags.Remove);
            }
            else
            {
                StopForeground(true);
            }
            StopSelf(); // -- arrêt du service
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
            _serviceGeo?.LocationChanged -= OnLocalicationChanged;
            _serviceGeo?.Pause = true;
        }

        /// <summary>
        /// Pour mettre en pause le service de géolocalisation,
        /// par exemple lorsque l'utilisateur met l'application en arrière-plan
        /// </summary>
        private void StopPause()
        {
            // -- arrêt des écoutes sur onchanged pour ne pas envoyer de message
            // à l'application MAUI
            _serviceGeo?.LocationChanged += OnLocalicationChanged;
            _serviceGeo?.Pause = false;
            
        }

        /// <summary>
        /// Demarre la géolocation grace au service de geolocation d'android 
        /// et envoie les messages de changement de localisation à l'application MAUI
        /// avec un message utilisation de community toolkit MVVM messenger
        /// </summary>
        private void StartGeolocalisation()
        {
            if (_serviceGeo is not null )
            {
                //_serviceGeo?.LocationChanged += OnLocalicationChanged;
                _serviceGeo?.StartLocationUpdatesAsync(_cancellationTokenSource.Token);
                _serviceGeo?.Pause = false;
            }
        }

        /// <summary>
        /// création du canal de notification pour le service foreground
        /// </summary>
        /// <param name="notificationManager"></param>
        private static void CreateNotificationChannel(NotificationManager notificationManager)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                if (notificationManager.GetNotificationChannel(NOTIFICATION_CHANNEL_ID) != null)
                {
                    return; // Le canal existe déjà
                }

                var channel = new NotificationChannel(NOTIFICATION_CHANNEL_ID, CHANNEL_NAME, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(channel);
            }
        }

        /// <summary>
        /// Démarre le service en mode "Foreground" avec une notification persistante
        /// </summary>
        private void StartForegroundService()
        {

        
            var notificationManager = Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;
            // 1. Créer le canal de notification (obligatoire pour Android 8+)
            if (notificationManager is not null && OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                CreateNotificationChannel(notificationManager);
            }

            // 2. Créer la notification qui sera visible par l'utilisateur
            NotificationCompat.Builder notification = new (this, NOTIFICATION_CHANNEL_ID);
            //notification.SetAutoCancel(false);
            notification.SetOngoing(true);
            notification.SetSmallIcon(Resource.Drawable.notification_bg_normal);
            notification.SetContentTitle("Suivi GPS actif");
            notification.SetContentText("Votre position est enregistrée en arrière-plan avec KronoGeo.");
            var notif = notification.Build();

            if (OperatingSystem.IsAndroidVersionAtLeast(29) && notif is not null)
            {
                StartForeground(NOTIFICATION_ID, notif, ForegroundService.TypeLocation);
            }
            else
            {
                StartForeground(NOTIFICATION_ID, notification.Build());
            }
            

        }
        #endregion

        #region public method event
        /// <summary>
        /// Envoyer le message de changement de localisation à l'application MAUI
        /// avec Messager de CommunityToolkit MVVM
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLocalicationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new LocationChangedMessage(e.Location));
        }
        #endregion

        #region public method interface
        /// <summary>
        /// méthod de démarrage du service
        /// </summary>
        public void StartService()
        {
            var intent = new Intent(this, typeof(GeoAndroidService));
            intent.SetAction(ActionStart);
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                StartForegroundService(intent);
            }
            else
            {
                StartService(intent);
            }
        }

        /// <summary>
        /// méthod de pause du service
        /// </summary>
        public void PauseService()
        {
            var intent = new Intent(this, typeof(GeoAndroidService));
            intent.SetAction(ActionPause);
            StartService(intent);
        }

        /// <summary>
        /// méthod de reprise du service après une pause
        /// </summary>
        public void StopPauseService()
        {
            var intent = new Intent(this, typeof(GeoAndroidService));
            intent.SetAction(ActionStopPause);
            StartService(intent);
        }

        /// <summary>
        /// méthod d'arrêt du service
        /// </summary>
        public void StopService()
        {
            var intent = new Intent(this, typeof(GeoAndroidService));
            intent.SetAction(ActionStop);
            StartService(intent);
        }
        #endregion
    }
}
