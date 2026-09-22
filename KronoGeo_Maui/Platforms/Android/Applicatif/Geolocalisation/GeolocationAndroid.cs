#if ANDROID
using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Locations;
using Android.OS;
using Android.Util;
using Java.Lang;
using Java.Util.Concurrent;
using KronoGeo_Api.Interface.AbstractClass;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Outils.Geolocalisation;
using System.Runtime.Versioning;
using static Microsoft.Maui.LifecycleEvents.AndroidLifecycle;
using AndroidApplication = Android.App.Application;
using CancellationToken = System.Threading.CancellationToken;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using Task = System.Threading.Tasks.Task;

namespace KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation
{
    /// <summary>
    /// Problème avec Location Manager qui marche mal avec la gestion de la batterie
    /// ForegroundService et wake lock
    /// </summary>
    [SupportedOSPlatform("android26.0")]
    public class GeolocationAndroid : IServiceGeolocalisation
    {
        #region private properties
        // -- systeme natif d'android pour la géolocation
        private readonly LocationManager? _locationManager;
        // - systeme découte android pour la géolocation
        private readonly LocationListener _locationListener = new();
        private readonly LocationListener _locationOnePoint = new();
        private readonly BaseGpsSmoother _gpsSmoother; 

        // -- mise en place d'un handler thread pour lancer la géolocalisation sur un thread parallèle au thread principal
        private HandlerThread _handlerThread = new("LocationHandlerthread");
        private Handler? _handler;
        #endregion

        #region public properties
        public CancellationToken CancellationToken { get ; set ; } = new ();
        public bool Pause { get; set; } = false;
        //public Location? DefaultLocation { get; set; } = default;  
        #endregion

        #region event récupération des datas
        // -- changement de localisation l'event n'est plus utiliser -- utilisation de ValueChangedMessage<Location>
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        //public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region constructeur
        public GeolocationAndroid(BaseGpsSmoother gpsSmoother)
        {
            _gpsSmoother = gpsSmoother;
            _locationManager = (LocationManager?)AndroidApplication.Context.GetSystemService(Context.LocationService);
            Init();
        }
        #endregion

        #region public method interface
        public void Dispose()
        {
            _handlerThread?.Dispose();
                
            GC.SuppressFinalize(this);
        }

        public async Task StartLocationUpdatesAsync()
        {
            if (_locationManager is null) return;

            try
            {
                // -- initial handler thread - pour tourner le système sur son propre thread
                _handlerThread.Dispose();
                _handlerThread = new("LocationHandlerthread");
                
                _handlerThread.Start();

                if (_handlerThread.Looper is null )
                {
                    Log.Error("GeoAndroidService", "handlerThread.looper est null");
                    return;
                }
                _handler = new(_handlerThread.Looper);

                // On force l'utilisation exclusive du GPS (Haute précision)
                string provider = LocationManager.GpsProvider;

                if (_locationManager.IsProviderEnabled(provider))
                {
                    // Paramètres de mise à jour :
                    _locationManager.RequestLocationUpdates(
                    provider,
                    5000, // -- 15000 millisecondes d'intervalle minimum pour déclencher l'événement
                    5, // -- 5 mètres de distance minimale pour déclencher l'événement
                    _locationListener,
                    // -- on injecte l'aiguilleur ici en cas de désynchronisation
                    // entre eventhandler et la mainthread
                    // quand la method StartLocationUpdatesAsync est lancé avec Task
                    //Looper.MainLooper 
                    _handler.Looper
                    );
                }
                else
                {
                    Log.Error("GeoAndroidService", "Le fournisseur GPS n'est pas activé sur l'appareil.");
                    //System.Diagnostics.Debug.WriteLine("Le fournisseur GPS n'est pas activé sur l'appareil.");
                    throw new FeatureNotEnabledException("Le fournisseur GPS n'est pas activé sur l'appareil.");
                }
            }
            catch (Java.Lang.SecurityException ex)
            {
                Log.Error("GeoAndroidService", $"Le fournisseur GPS n'est pas activé sur l'appareil. \n {ex.Message}");
                //System.Diagnostics.Debug.WriteLine($"Erreur de permission : {ex.Message}");
                throw new PermissionException($"Permission de localisation refusée. Veuillez accorder les permissions nécessaires. {ex.Message}");
            }
            catch(Java.Lang.Exception ex)
            {
                Log.Error("GeoAndroidService", $"Erreur dans StartLocationUpdatesAsync \n {ex.Message}");
                throw new System.Exception(ex.Message);
            }
        }

        public async Task StopLocationUpdatesAsync()
        {
            if ( _locationManager is not null && _locationListener != null)
            {
                // -- réset le Smoother 
                _gpsSmoother.Reset();
                // Très important pour économiser la batterie quand on n'en a plus besoin
                await  Task.Run( () => 
                    _locationManager.RemoveUpdates(_locationListener));
            }
        }
        #endregion

        #region private method
        private void Init()
        {
            if (Pause) return;
            // S'abonner au retour du listener
            _locationListener.OnLocationChangedAction = (location) =>
            {
                // Ici vous récupérez la position précise
                /*double latitude = location.Latitude;
                double longitude = location.Longitude;
                double altitude = location.Altitude;
                float accuracy = location.Accuracy; // Précision en mètres*/

                Log.Debug("GeoAndroidService", $"Provider : {location.Provider}");
                Log.Debug("GeoAndroidService", $"Accuracy : {location.Accuracy} " +
                    $"- Longitude : {location.Longitude} - Latitude : {location.Latitude}");

                
                Microsoft.Maui.Devices.Sensors.Location newLocation = new(
                    location.Latitude, location.Longitude, location.Altitude
                    )
                {
                    Accuracy = (double)location.Accuracy,
                    Speed = (double)location.Speed,
                    Timestamp = DateTimeOffset.Now,
                    Course = (double)location.Bearing,
                    VerticalAccuracy = (double)location.VerticalAccuracyMeters
                };
                
                var locationSmoother = _gpsSmoother.AcceptableLocationCalcul(newLocation);

                Log.Debug("GeoAndroidService", $"Geolocalisation : {newLocation.Latitude} - {newLocation.Longitude}");

                if (locationSmoother is not null)
                    LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(locationSmoother));

                /*if (accuracy > 15) // Seuil de précision (15 mètres dans cet exemple)
                {
                    return; // Ignorer cette position
                }*/

                // -- appel de l'événement pour le code partagé --
                /*LocationChanged?.Invoke(this,
                    new GeolocationLocationChangedEventArgs(new Microsoft.Maui.Devices.Sensors.Location(latitude, longitude, altitude)
                    {
                        Accuracy = (double)accuracy,
                        Speed = (double)location.Speed,
                        Timestamp = DateTimeOffset.Now,
                        Course = (double)location.Bearing,
                        VerticalAccuracy = (double)location.VerticalAccuracyMeters
                    }));*/

                // TODO: Envoyer ces données à votre code partagé (via un événement ou Messenger)
            };
        }

        public async Task<Location?> GetCurrentLocationAsync(CancellationToken token)
        {
            var tcs = new TaskCompletionSource<Location>();
            token.Register(() => tcs.TrySetCanceled());

            if (_locationManager == null)
                return null;

            // On force l'utilisation exclusive du GPS (Haute précision)
            string provider = LocationManager.GpsProvider;

            if (!_locationManager.IsProviderEnabled(provider))
            {
                // Le fournisseur GPS n'est pas activé, tu peux lever une exception ici si tu gères ça ailleurs
                return null;
            }

            // GetCurrentLocation natif est dispo à partir de l'API 30 (Android 11)
            if (OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                var cancellationSignal = new CancellationSignal();
                token.Register(() => cancellationSignal.Cancel());

                var consumer = new LocationConsumer(androidLocation =>
                {
                    if (androidLocation != null)
                    {
                        
                        // Mapping des données Android vers MAUI
                        var mauiLocation = new Microsoft.Maui.Devices.Sensors.Location
                        {
                            Latitude = androidLocation.Latitude,
                            Longitude = androidLocation.Longitude,
                            Altitude = androidLocation.HasAltitude ? androidLocation.Altitude : null,
                            Accuracy = androidLocation.HasAccuracy ? androidLocation.Accuracy : null,
                            Speed = androidLocation.HasSpeed ? androidLocation.Speed : null,
                            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(androidLocation.Time),
                            Course = androidLocation.HasBearing ? androidLocation.Bearing : null
                        };
                        
                        // VerticalAccuracy (dispo à partir de l'API 26)
                        if (OperatingSystem.IsAndroidVersionAtLeast(26) && androidLocation.HasVerticalAccuracy)
                        {
                            mauiLocation.VerticalAccuracy = androidLocation.VerticalAccuracyMeters;
                        }
                        
                        tcs.TrySetResult(mauiLocation);
                    }
                    else
                    {
                        // Si Android n'arrive pas à fixer un point du tout
                        Log.Error("GeoAndroidService", "Le Gps n'arrive pas à fixer un point gps.");
                        throw (new InvalidNavigationException("Le Gps n'arrive pas à fixer un point gps."));
                    }
                });

                // On lance la requête avec un exécuteur sur un thread séparé
                IExecutorService? executor = Java.Util.Concurrent.Executors.NewSingleThreadExecutor();

                if (executor is not null)
                {
                    _locationManager.GetCurrentLocation(
                    provider,
                    cancellationSignal,
                    executor,
                    consumer);
                }
            }
            

            return await tcs.Task;
        }
        #endregion

    }
}

#endif


