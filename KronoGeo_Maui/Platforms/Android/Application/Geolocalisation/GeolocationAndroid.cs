using KronoGeo_Maui.Applications.Interface;
using Android.Content;
using Android.Locations;
using Android.App;
using Android.OS;
using AndroidApplication = Android.App.Application;
using System.Runtime.Versioning;
using KronoGeo_Maui.Applications.Outils.Geolocalisation;

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
        #endregion

        #region public properties
        public CancellationTokenSource CancellationTokenSource { get ; set ; } = new CancellationTokenSource();
        public bool Pause { get; set; } = false;
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
        #endregion
    }
}    




