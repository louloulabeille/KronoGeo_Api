using Android.App;
using Android.Content;
using Android.Gms.Tasks;
using Android.Locations;
using Android.OS;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Outils.Geolocalisation;
using System.Runtime.Versioning;
using AndroidApplication = Android.App.Application;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using CancellationToken = System.Threading.CancellationToken;
using Task = System.Threading.Tasks.Task;
using Java.Util.Concurrent;

namespace KronoGeo_Maui.Platforms.Android.Application.Geolocalisation
{
    [SupportedOSPlatform("android26.0")]
    public class GeolocationAndroid : IServiceGeolocalisation
    {
        #region private properties
        // -- systeme natif d'android pour la géolocation
        private readonly LocationManager? _locationManager;
        // - systeme découte android pour la géolocation
        private readonly LocationListener _locationListener = new();
        private readonly LocationListener _locationOnePoint = new();
        #endregion

        #region public properties
        public CancellationToken CancellationToken { get ; set ; } = new ();
        public bool Pause { get; set; } = false;
        //public Location? DefaultLocation { get; set; } = default;  
        #endregion

        #region event récupération des datas
        // -- changement de localisation l'event n'est plus utiliser -- utilisation de ValueChangedMessage<Location>
        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region constructeur
        public GeolocationAndroid()
        {
            _locationManager = (LocationManager?)AndroidApplication.Context.GetSystemService(Context.LocationService);
            Init();
        }
        #endregion

        #region public method interface
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void StartLocationUpdatesAsync()
        {
            if (_locationManager == null) return;

            try
            {
                // On force l'utilisation exclusive du GPS (Haute précision)
                string provider = LocationManager.GpsProvider;

                if (_locationManager.IsProviderEnabled(provider))
                {
                    // Paramètres de mise à jour :
                    _locationManager.RequestLocationUpdates(
                    provider,
                    15000, // -- 15000 millisecondes d'intervalle minimum pour déclencher l'événement
                    5, // -- 5 mètres de distance minimale pour déclencher l'événement
                    _locationListener,
                    // -- on injecte l'aiguilleur ici en cas de désynchronisation
                    // entre eventhandler et la mainthread
                    // quand la method StartLocationUpdatesAsync est lancé avec Task
                    Looper.MainLooper 
                    );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Le fournisseur GPS n'est pas activé sur l'appareil.");
                    throw new FeatureNotEnabledException("Le fournisseur GPS n'est pas activé sur l'appareil.");
                }
            }
            catch (Java.Lang.SecurityException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur de permission : {ex.Message}");
                throw new PermissionException($"Permission de localisation refusée. Veuillez accorder les permissions nécessaires. {ex.Message}");
            }
        }

        public void StopLocationUpdates()
        {
            if ( _locationManager != null && _locationListener != null)
            {
                // Très important pour économiser la batterie quand on n'en a plus besoin
                _locationManager.RemoveUpdates(_locationListener);
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
                GpsSmoother smoother = new();
                var locationSmoother = smoother.AcceptableLocationCalcul(newLocation);

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

        /// <summary>
        /// Démarre la récupération de la localisation en tâche de fond avec un CancellationToken
        /// Attention avec les Thread et les Task.Run, il faut faire attention à ne pas bloquer le thread principal 
        /// et à gérer correctement les exceptions. 
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartLocationUpdatesAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {       
                StartLocationUpdatesAsync();
            }, cancellationToken);
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




