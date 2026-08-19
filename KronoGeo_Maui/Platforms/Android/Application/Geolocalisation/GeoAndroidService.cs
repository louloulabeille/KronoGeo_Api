using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using Android.Util;
using Android.Content.PM;
using CommunityToolkit.Mvvm.Messaging;
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
    public class GeoAndroidService : Service
    {
        #region private properties
        // -- information pour la notification du service foreground
        private const string NOTIFICATION_CHANNEL_ID = "46100";
        private readonly int NOTIFICATION_ID = 1;
        private const string CHANNEL_NAME = "location_notification_channel";

        private IServiceGeolocalisation? _serviceGeo; // -- service de géolocalisation qui sera injecté par OnCreate
        private readonly CancellationTokenSource _cancellationTokenSource = new ();
        private PowerManager.WakeLock? _wakeLock = null; // -- WakeLock pour empêcher le téléphone de se mettre en veille pendant que le service est actif
        private NotificationManager? _notificationManager;
        #endregion

        #region public const properties action pour démarrer le service de géolocalisation
        public const string ActionStart = "Start_Geolocation";
        public const string ActionPause = "Pause_Geolocation";
        public const string ActionStopPause = "StopPause_Geolocation";
        public const string ActionStop = "Stop_Geolocation";
        #endregion
        public GeoAndroidService() : base()
        {
        }
        
        #region public method override
        public override IBinder? OnBind(Intent? intent) => null;

        /// <summary>
        /// method pour implementer le service de géolocalisation dans l'application MAUI
        /// il n'accepte qu'un constructeur sans paramètre,
        /// donc on ne peut pas passer de paramètre au service
        /// </summary>
        public override void OnCreate()
        {
            base.OnCreate();
            _serviceGeo = IPlatformApplication.Current?.Services.GetService<IServiceGeolocalisation>();

            if( _serviceGeo is null )
            {
                // Sécurité au cas où le service de géolocalisation n'est pas disponible
                Log.Error("GeoAndroidService", "Le service de géolocalisation n'a pas pu être récupéré.");
            }
            _serviceGeo?.LocationChanged += OnLocalicationChanged;
        }

        /// <summary>
        /// method qui est appelé lorsque le service est démarré
        /// </summary>
        /// <param name="intent"></param>
        /// <param name="flags"></param>
        /// <param name="startId"></param>
        /// <returns></returns>
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
            if( _wakeLock is not null && _wakeLock.IsHeld )
            {
                _wakeLock?.Release();
                _wakeLock?.Dispose();
                _wakeLock = null;
            }

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
            _notificationManager?.Cancel(NOTIFICATION_ID);
            base.OnDestroy();
        }
        #endregion

        #region private method
        /// <summary>
        /// WakeLock pour empêcher le téléphone de se mettre
        /// en veille pendant que le service est actif
        /// </summary>
        private void AcquireWakeLock()
        {
            if (_wakeLock is not null && _wakeLock.IsHeld) return;
            
            var powerManager = GetSystemService(Context.PowerService) as PowerManager;
            if ( powerManager is not null )
            {
                _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "GeoAndroidService:BackgroundTrackingLock");
                _wakeLock?.Acquire();
            }
            

        }
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
            _notificationManager = Platform.AppContext.GetSystemService(Context.NotificationService) as NotificationManager;
            // 1. Créer le canal de notification (obligatoire pour Android 8+)
            if (_notificationManager is not null && OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                CreateNotificationChannel(_notificationManager);
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
    }
}
