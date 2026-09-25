using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Provider;
using Android.Util;
using AndroidX.Core.App;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.Factory.Geolocalisation;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.Platforms.Android.Applicatif.Factory.WakeLock;
using KronoGeo_Maui.Platforms.Android.Applicatif.OsManager;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Text;


namespace KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation
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
        private FactoryWakelock? _factoryWakelock = default;
        private PowerSaveModeReceiver? _powerSaveReceiver = default;
        private IServiceBattery? _serviceBattery = default;
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
            // -- injection de la factory FactoryGeolocation
            var factory = IPlatformApplication.Current?.Services.GetService<FactoryGeolocation>();
            _serviceGeo = factory?.GetServiceGeolocalisation();

            // -- factory pour créer le wakelock selon le fabricant du téléphone
            var factoryWakeLock  = IPlatformApplication.Current?.Services.GetService<FactoryWakelock>();
            _factoryWakelock = factoryWakeLock;

            // -- injection du service de gestion de battery saver
            var serviceBattery = IPlatformApplication.Current?.Services.GetService<IServiceBattery>();
            _serviceBattery = serviceBattery;

            if ( _serviceGeo is null )
            {
                // Sécurité au cas où le service de géolocalisation n'est pas disponible
                Log.Error("GeoAndroidService", "Le service de géolocalisation n'a pas pu être récupéré.");
            }
            //_serviceGeo?.LocationChanged += OnLocalicationChanged;
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
                    Log.Debug("GeoAndroidService", "Début du service OnStartCommand");
                    // 3. Démarrer le service en mode "Foreground"
                    // Depuis Android 14, il faut impérativement spécifier le type de service ici aussi
                    StartForegroundServiceGeo();

                    // -- mise en place du WakeLock pour empêcher de mettre en service en pause
                    AcquireWakeLock();

                    // -- lancement de l'écoute sur la modification du système d'économie d'énergie
                    RegisterPowerSaveModeReceiver();

                    // 4. C'est ICI que tu lances ta logique de géolocalisation
                    // (ex: un timer ou un abonnement au GPS qui enregistre tes points)
                    Task.Run(async () => await StartGeolocalisation());
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
        public async override void OnDestroy()
        {
            Log.Debug("GeoAndroidService", "Fin du service OnDestroy");

            // -- désabonnement du BroadcastReceiver
            if (_powerSaveReceiver is not null)
            {
                UnregisterReceiver(_powerSaveReceiver);
                _powerSaveReceiver = null;
            }

            // Demander l'annulation des tâches asynchrones et libérer les ressources
            try
            {
                _cancellationTokenSource.Cancel();
            }
            catch { }

            if ( _wakeLock is not null && _wakeLock.IsHeld )
            {
                // Si un WakeLock est encore détenu, le relâcher proprement
                _wakeLock?.Release();
                _wakeLock?.Dispose();
                _wakeLock = null;
                Log.Debug("GeoAndroidService", "Stop le wakelock");
            }

            // Arrêter proprement le GPS ici pour économiser la batterie
            if (_serviceGeo is not null)
            {
                await _serviceGeo.StopLocationUpdatesAsync();
                _serviceGeo?.Dispose();
            } 

            // -- arrêt du service
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
            
            try { _cancellationTokenSource.Dispose(); } catch { }

            _serviceGeo?.LocationChanged -= OnLocalicationChanged;

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
            // Éviter un WakeLock permanent : n'acquérir que pour une courte durée
            if (_wakeLock is not null && _wakeLock.IsHeld) return;

            var powerManager = GetSystemService(Context.PowerService) as PowerManager;
            if ( powerManager is not null )
            {
                // -- ne marche pas à cause de samsung qui est trop agressif dans la gestion de la batterie 
                //_wakeLock = powerManager.NewWakeLock(WakeLockFlags.Partial, "GeoAndroidService:BackgroundTrackingLock");
                // -- utilisation d'un wakelock qui va empêcher d'éteindre l'écran mais va diminuer la luminosité
                //_wakeLock = powerManager.NewWakeLock(WakeLockFlags.ScreenDim | WakeLockFlags.OnAfterRelease,
                //    "GeoAndroidService:ScreenDimTrackingLock");

                _wakeLock = _factoryWakelock?.GetWakeLock(powerManager, DeviceInfos.Manufacturer);
                try
                {
                    // Acquérir avec timeout (10s) pour démarrer proprement la géolocalisation
                    // cela évite de garder le CPU allumé indéfiniment et économise la batterie
                    Log.Debug("GeoAndroidService", "-- wakelock demarré --");
                    //_wakeLock?.Acquire(10000);
                    _wakeLock?.Acquire(); // -- sans timeout marche toute le temps attention à la batterie
                }
                catch
                {
                    // fallback : acquérir sans timeout si la plateforme ne supporte pas l'overload
                    try { _wakeLock?.Acquire(); } catch { }
                }
            }

        }
        /// <summary>
        /// Pour mettre en pause le service de géolocalisation,
        /// par exemple lorsque l'utilisateur met l'application en arrière-plan
        /// </summary>
        private void Pause()
        {

            if ( _serviceGeo?.Pause == true ) return;

            // -- arrêt des écoutes sur onchanged pour ne pas envoyer de message
            // à l'application MAUI
            _serviceGeo?.LocationChanged -= OnLocalicationChanged;
            _serviceGeo?.Pause = true;

            // 2. Relâcher le wakelock pendant la pause (pas besoin de garder le CPU actif)
            if (_wakeLock is not null && _wakeLock.IsHeld)
            {
                _wakeLock.Release();
                Log.Debug("GeoAndroidService", "Wakelock relâché pour la pause");
            }

            Log.Debug("GeoAndroidService", "Service mis en pause");
        }

        /// <summary>
        /// Pour mettre en pause le service de géolocalisation,
        /// par exemple lorsque l'utilisateur met l'application en arrière-plan
        /// </summary>
        private void StopPause()
        {
            if (_serviceGeo?.Pause == false) return;

            AcquireWakeLock();

            // -- arrêt des écoutes sur onchanged pour ne pas envoyer de message
            // à l'application MAUI
            _serviceGeo?.LocationChanged += OnLocalicationChanged;
            _serviceGeo?.Pause = false;

            Log.Debug("GeoAndroidService", "Service repris");
        }

        /// <summary>
        /// Demarre la géolocation grace au service de geolocation d'android 
        /// et envoie les messages de changement de localisation à l'application MAUI
        /// avec un message utilisation de community toolkit MVVM messenger
        /// </summary>
        private async Task StartGeolocalisation()
        {
            if (_serviceGeo is not null )
            {
                _serviceGeo.LocationChanged += OnLocalicationChanged;
                await _serviceGeo.StartLocationUpdatesAsync(_cancellationTokenSource.Token);
                //_serviceGeo?.Pause = false;
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
        private void StartForegroundServiceGeo()
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
            notification.SetSmallIcon(Microsoft.Maui.Resource.Drawable.notification_bg_normal);
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

        /// <summary>
        /// Method qui change le message de la notication
        /// sans la re-créer
        /// </summary>
        /// <param name="contentText"></param>
        private void UpdateNotification(string contentText)
        {
            if (_notificationManager is null) return;

            var notification = new NotificationCompat.Builder(this, NOTIFICATION_CHANNEL_ID)?
                .SetOngoing(true)?
                .SetSmallIcon(Microsoft.Maui.Resource.Drawable.notification_bg_normal)?
                .SetContentTitle("Suivi GPS actif")?
                .SetContentText(contentText)?
                .Build();

            _notificationManager.Notify(NOTIFICATION_ID, notification);
        }

        /// <summary>
        /// Lancement de l'écoute avec un BroadcastReceiver
        /// </summary>
        private void RegisterPowerSaveModeReceiver()
        {
            _powerSaveReceiver = new PowerSaveModeReceiver(OnPowerSaveModeChanged);
            var filter = new IntentFilter(PowerManager.ActionPowerSaveModeChanged);
            RegisterReceiver(_powerSaveReceiver, filter);
        }

        /// <summary>
        /// méthod appelé dans le BroadCastReceiver
        /// </summary>
        /// <param name="isActive"></param>
        private async void OnPowerSaveModeChanged(bool isActive)
        {
            if (isActive)
            {
                string message = $"⚠️ Précision GPS réduite (économie d'énergie active)";
                // Mettre à jour la notification foreground pour informer en direct
                UpdateNotification(message);

                // -- affichage message
                var cancellationToken = new System.Threading.CancellationToken();
                await Toast.Make(message, ToastDuration.Long)
                    .Show(cancellationToken);

                // ouverture de la fenêtre de gestion de la batterie
                // sans bloquer le reste 
                var dispatcher = Dispatcher.GetForCurrentThread();
                var timer = dispatcher?.CreateTimer();
                if (timer is null)
                {   // -- ouverture de la fenêtre d'économie d'énergie
                    _serviceBattery?.OpenWindowBatterySaver();
                    return;
                }
                timer.Interval = TimeSpan.FromSeconds(5);
                timer.IsRepeating = false;
                timer.Tick += (s, e) => {
                    _serviceBattery?.OpenWindowBatterySaver();
                };
                timer.Start();

            }
            else
            {
                UpdateNotification("Votre position est enregistrée en arrière-plan avec KronoGeo.");
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
