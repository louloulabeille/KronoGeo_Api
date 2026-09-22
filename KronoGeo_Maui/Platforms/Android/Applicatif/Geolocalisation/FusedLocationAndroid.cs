#if ANDROID
using Android.App;
using Android.Content;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Java.Lang;
using Kotlin.Jvm.Internal;
using KronoGeo_Api.Interface.AbstractClass;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.Applications.Outils.Geolocalisation;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AndroidApplication = Android.App.Application;
using Application = Android.App.Application;
using CancellationToken = System.Threading.CancellationToken;
using Location = Microsoft.Maui.Devices.Sensors.Location;
using LocationA = Android.Locations.Location;
using Task = System.Threading.Tasks.Task;
using TaskA = Android.Gms.Tasks.Task;



namespace KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation
{
    /// <summary>
    /// Faire attention au package et aux dépendances ce qui a été ajouté dans le fichier csproj
    /// Attention au mise à jour du package Xamarin.GooglePlayServices.Location aave le reste des 
    /// dépendances sur projet MAUI
    /// </summary>
    public class FusedLocationAndroid : IServiceGeolocalisation , IRecipient<LocationBroadcastMessage>
    {
        /// dans la partie PropertyGroup
        /// Empêche le warning NU1605 d'être traité comme une erreur bloquant
        /// <WarningsNotAsErrors>$(WarningsNotErrors);NU1605</WarningsNotAsErrors>
        /// ignore la vérification stricte des conflits de versions  transitives
        /// <NoWarn>$(NoWarn);NU1605;NU1608</NoWarn>
        /// ----------------------
        /// dans la partie package
        /// <ItemGroup Condition="'$(TargetFramework)' == 'net10.0-android'">
        /// <!-- force l'utilisation d'une seule version  -->
		/// <PackageReference Include = "Xamarin.AndroidX.Collection.Jvm" Version="1.6.0.1" />
		/// <PackageReference Include = "Xamarin.AndroidX.Lifecycle.Runtime" Version="2.11.0.1" />
		/// <!-- erreur lors du build les 2 versions n'étaient pas à la même version -->
		/// <PackageReference Include = "Xamarin.AndroidX.Fragment" Version="1.9.0" />
		/// <PackageReference Include = "Xamarin.AndroidX.Fragment.Ktx" Version="1.9.0" />
	    /// </ItemGroup>
	    /// <ItemGroup Condition = "'$(TargetFramework)' == 'net10.0-android'" >
        /// <PackageReference Include="Xamarin.GooglePlayServices.Location">
	    /// <Version>121.4.0.1</Version>
	    /// </PackageReference>
	    /// </ItemGroup>

        /// Inclusion des packages qui ne sont pas  a jour pour Xamarin.GooglePlayServices.Location 
		/// avec mes versions de packages Maui à jour en.net10
        /// Xamarin.AndroidX.Collection.Jvm dans mon projet est 1.6.0.1
		/// et dans Xamarin.GooglePlayServices.Location il veut 1.5.0.1 de même 
        /// pour Xamarin.AndroidX.Lifecycle.Runtime


        #region private properties
        // -- mise en place d'un handler thread pour lancer la géolocalisation sur un thread parallèle au thread principal
        //private readonly HandlerThread _handlerThread = new("LocationHandlerthread");
        //private Handler? _handler;
        private LocationCallback? _locationCallback = default;
        private PendingIntent? _locationPendingIntent = default;
        private readonly IFusedLocationProviderClient? _locationClient;
        /// <summary>
        ///  Limiteur de nombre de thread - ici un seul jeton avec une seul consommateur
        /// </summary>
        private readonly SemaphoreSlim _transitionLock = new(1, 1);
        private bool _isRunning = false;
        private readonly BaseGpsSmoother _gpsSmoother;
        #endregion

        #region public properties interface IServiceGeolocalisation
        /// <summary>
        /// gestion du système de pause 
        /// </summary>
        public bool Pause { get; set; } = false;

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        //public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region public constructeur
        public FusedLocationAndroid (BaseGpsSmoother gpsSmoother)
        {
            _gpsSmoother = gpsSmoother;
            _locationClient = LocationServices.GetFusedLocationProviderClient(Application.Context);
            InitPendingIntent();
        }
        #endregion

        #region pulic method interface IServiceGeolocalisation
        public void Dispose()
        {
            WeakReferenceMessenger.Default.UnregisterAll(this);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Donne le current location en passant par FusedLocation et la package Xamarin.GooglePlayService.Location
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Location?> GetCurrentLocationAsync(CancellationToken token)
        {
            try
            {
                // -- création de l'object pour custom la location request 
                var request = new CurrentLocationRequest.Builder().SetPriority(Priority.PriorityHighAccuracy)
                .SetDurationMillis(10000)
                .Build();

                // -- mise en place du task java pour traiter l'api google avec une méthode asynchrone de c#
                var tcs = new TaskCompletionSource<Location?>();

                using (token.Register(() => tcs.TrySetCanceled()))
                {
                    // Appel de la méthode Java native
                    var javaTask = _locationClient?.GetCurrentLocation(request, null);

                    // Conversion en Task C# via les listeners Java
                    javaTask?.AddOnSuccessListener(new OnSuccessListener(location =>
                    {
                        // -- récupération de location android 
                        var localAndroid = location as LocationA;
                        // -- création du location de miscrosoft pour être traité dans le code c#
                        Location? loc = default;
                        if (localAndroid is not null)
                        {
                            loc = new Location
                            {
                                Accuracy = localAndroid.Accuracy,
                                Altitude = localAndroid.Altitude,
                                AltitudeReferenceSystem = AltitudeReferenceSystem.Ellipsoid,    // système de reférence utiliser dans android
                                Latitude = localAndroid.Latitude,
                                Longitude = localAndroid.Longitude,
                                Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(localAndroid.Time).ToLocalTime(), // -- conversion de milliseconde Unix mesure en DateTimeOffset
                                Speed = localAndroid.Speed,
                                VerticalAccuracy = // -- ne marche pas pour les versions android en dessous de 26
                                    OperatingSystem.IsAndroidVersionAtLeast(26) ? (double)(localAndroid.VerticalAccuracyMeters) : 0,
                                ReducedAccuracy = false, // -- ne marche que pour IOS
                                Course = localAndroid.Bearing
                            };
                        }
                        // -- retourne si success la location    
                        tcs.TrySetResult(loc);
                    }));

                    // -- en cas de problème on lève une exception 
                    // -- je ne sais plus si je le traite derrière :) 
                    javaTask?.AddOnFailureListener(new OnFailureListener(exception =>
                    {
                        tcs.TrySetException(exception);
                    }));
                    // -- retourn le résultat
                    return await tcs.Task;
                }
            
            } catch (Java.Lang.SecurityException ex)
            {
                Log.Error("GeoAndroidService", $"{ex.Message}");
                throw new PermissionException("Permission de localisation refusée. Veuillez accorder les permissions nécessaires.");
            }

        }

        /// <summary>
        /// method qui lance la prise des localisations en continue
        /// </summary>
        public async Task StartLocationUpdatesAsync()
        {
            await _transitionLock.WaitAsync();
            try
            {

                if ( _isRunning)
                {
                    Log.Debug("GeoAndroidService", "StartLocationUpdatesAsync est déjà lancé");
                    return;
                }

                if (LocationChanged is not null)
                {
                    // -- mise en place du messenger 
                    StartMessenger();

                    // -- initial handler thread plus besoin en utilisant un intent
                    // sert à lancer dans un autre thread que le principal
                    //_handlerThread.Start(); 
                    //if (_handlerThread.Looper is null)
                    //{
                    //    Log.Error("GeoAndroidService", "handlerThread.looper est null");
                    //    return;
                    //}
                    //_handler = new(_handlerThread.Looper);

                    // -- object du custom request 
                    var locationRequest = new LocationRequest.Builder(Priority.PriorityHighAccuracy, 5000) // 5 sec
                    .SetMinUpdateIntervalMillis(5000)
                    .SetWaitForAccurateLocation(true) // -- renvoie un point location stabilisé
                    .SetMaxUpdateDelayMillis(5000) // -- mode batching de Fused permet de lisser certains dégrader en un seul point
                    .SetGranularity(Granularity.GranularityFine) // -- obligé pour chaque fix location la plus fine possible
                    .SetMinUpdateDistanceMeters(2)
                    .Build();
                    #region mise en place d'un customlocationcallback code au cas ou
                    // -- traitement à faire pour le retour du Callback de base ici 
                    // -- un callback spécifique
                    /*_locationCallback = new CustomLocationCallback(location =>
                    {
                        if (Pause)
                        {
                            Log.Debug("GeoAndroidService", "Mise en Pause");
                            return;
                        }

                        // Traite le point GPS ici (ex. enregistrement BDD local ou envoi à un ViewModel)
                        var loc = new Location(location.Latitude, location.Longitude)
                        {
                            Accuracy = location.Accuracy,
                            Altitude = location.Altitude,
                            AltitudeReferenceSystem = AltitudeReferenceSystem.Ellipsoid,
                            // -- conversion de milliseconde Unix mesure en DateTimeOffset localtime
                            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(location.Time).ToLocalTime(),
                            Speed = location.Speed,
                            VerticalAccuracy =  // -- ne marche pas pour les versions android en dessous de 26
                                OperatingSystem.IsAndroidVersionAtLeast(26) ? (double)(location.VerticalAccuracyMeters) : 0,
                            ReducedAccuracy = false,    // -- ne marche que pour IOS
                            Course = location.Bearing
                        };

                        // -- va lisser les points GPS selon le degrès d'exactitude
                        GpsSmoother smoother = new();
                        var locationSmoother = smoother.AcceptableLocationCalcul(loc);

                        if (locationSmoother is null)
                        {
                            Log.Debug("GeoAndroidService", $"locationSmoother is null");
                            return;
                        }

                        Log.Debug("GeoAndroidService", $"Latitude : {location.Latitude}, Longitude : {location.Longitude}");
                        // -- appel de eventhandler pour appeler le code qui doit être traité
                        LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(locationSmoother));
                    });*/
                    #endregion
                    // -- appel de la fonction du lancement de l'écoute
                    //_locationClient?.RequestLocationUpdates(locationRequest, _locationCallback, _handler.Looper);
                    if (_locationPendingIntent is not null && _locationClient is not null)
                    {
                        Log.Debug("GeoAndroidService", "Lancement de la geolocalisation -- StartLocationUpdatesAsync");
                        await _locationClient.RequestLocationUpdatesAsync(locationRequest, _locationPendingIntent);
                        _isRunning = true;
                    }
                        
                }

            }
            catch (Java.Lang.SecurityException exS)
            {
                Log.Error("GeoAndroidService", $"Permission de localisation refusée. Veuillez accorder les permissions nécessaires \n {exS.Message}");
                //System.Diagnostics.Debug.WriteLine($"Erreur de permission : {ex.Message}");
                throw new PermissionException($"Permission de localisation refusée. Veuillez accorder les permissions nécessaires.");
            }
            catch(Java.Lang.Exception ex)
            {
                Log.Error("GeoAndroidService", $"{ex.Message}");
                throw new System.Exception(ex.Message); 
            }
            finally
            {
                _transitionLock.Release();
            }
        }

        /// <summary>
        /// Arrête le processus de prise de localisation
        /// il peut être prise en compte pour mettre en pause aussi
        /// </summary>
        public async Task StopLocationUpdatesAsync()
        {
            await _transitionLock.WaitAsync();

            if (_locationClient is not null && _locationPendingIntent is not null )
            {
                try
                {
                    if( !_isRunning )
                    {
                        Log.Debug("GeoAndroidService", $"StopLocationUpdatesAsync ignoré : la géolocalisation déjà arrêtée!!");
                        return;
                    }

                    // -- réset le Smoother 
                    _gpsSmoother.Reset();

                    await _locationClient.RemoveLocationUpdatesAsync(_locationPendingIntent);
                    Log.Debug("GeoAndroidService", "Arrêt du Fuse");

                    _locationCallback = null;
                    // -- arrêt du register pour ne pas l'avoir en double
                    WeakReferenceMessenger.Default.Unregister<LocationBroadcastMessage>(this);

                    _isRunning = false;
                }
                catch( Java.Lang.Exception ex)
                {
                    Log.Error("GeoAndroidService", $"Erreur dans StopLocationUpdates : \n {ex.Message}");
                }
                finally
                {
                    _transitionLock.Release();
                }
                
                
            }
        }

        #endregion

        #region public method IRecipient<LocationBroadcastMessage>
        /// <summary>
        /// Method qui est appelé lors d'un send 
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Receive(LocationBroadcastMessage message)
        {
            TraitementLocationMessenger(message.Value);
        }
        #endregion

        #region private method
        private void TraitementLocationMessenger ( Location location)
        {
            // - gestion de la pause
            if (Pause)
            {
                Log.Debug("GeoAndroidService", "Mise en Pause");
                return;
            }

            // -- va lisser les points GPS selon le degrès d'exactitude
            var locationSmoother = _gpsSmoother.AcceptableLocationCalcul(location);

            if (locationSmoother is null)
            {
                Log.Debug("GeoAndroidService", $"locationSmoother is null - Accuracy : { location.Accuracy }");
                return;
            }

            Log.Debug("GeoAndroidService", $"Latitude : {location.Latitude}, Longitude : {location.Longitude}");
            // -- appel de eventhandler pour appeler le code qui doit être traité
            LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(locationSmoother));

        }
        private void InitPendingIntent()
        {
            // -- context de l'application Android.App.Application.Context
            var context = Application.Context;

            // Initialisation du PendingIntent pointant vers le BroadcastReceiver
            var intent = new Intent(context, typeof(LocationBroadcastReceiver));

            // Mutability flag requis à partir d'Android 12 (API 31+)
            var flags = PendingIntentFlags.UpdateCurrent;
            if(OperatingSystem.IsAndroidVersionAtLeast(31))
            {
                flags |= PendingIntentFlags.Mutable;
            }

            _locationPendingIntent = PendingIntent.GetBroadcast(context, 0, intent, flags);
        }

        /// <summary>
        /// Pour gérer le lancement et relancement du système de géolocalisation 
        /// </summary>
        private void StartMessenger()
        {
            // -- arrêt du register pour ne pas l'avoir en double
            WeakReferenceMessenger.Default.Unregister<LocationBroadcastMessage>(this);
            // -- register du messenger 
            WeakReferenceMessenger.Default.Register<LocationBroadcastMessage>(this, (recipient, message ) => {
                Receive(message);
            });

        }
        #endregion

        #region internal method
        // Helpers pour les listeners Java/Android Tasks vers c#
        /// <summary>
        /// helpers pour gérer les retours des api Java/android pour gérer les retour task de java
        /// et faire le pont avec celui de c#
        /// </summary>
        /// <param name="action"></param>
        internal class OnSuccessListener(Action<Java.Lang.Object?> action) : Java.Lang.Object, IOnSuccessListener
        {
            private readonly Action<Java.Lang.Object?> _action = action;
            //public OnSuccessListener => _action = action;
            public void OnSuccess(Java.Lang.Object? result) => _action(result);
        }

        internal class OnFailureListener(Action<Java.Lang.Exception> action) : Java.Lang.Object, IOnFailureListener
        {
            private readonly Action<Java.Lang.Exception> _action = action;
            //public OnFailureListener => _action = action;
            public void OnFailure(Java.Lang.Exception exception) => _action(exception);
        }
        #endregion

    }

    /// <summary>
    /// Class de LocationCallback pour la réception des locations probleme avec des fuites de données
    /// passage vers un Broadcast avec un message
    /// </summary>
    /// <param name="onLocationReceived"></param>
    public class CustomLocationCallback(Action<global::Android.Locations.Location> onLocationReceived) : LocationCallback
    {
        private readonly Action<global::Android.Locations.Location> _onLocationReceived = onLocationReceived;

        public override void OnLocationResult(LocationResult result)
        {
            if (result?.LastLocation is not null)
                _onLocationReceived(result.LastLocation);
        }
    }


    /// <summary>
    /// Création d'un BroadcastReceiver pour recevoir les locations d'Android
    /// </summary>
    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class LocationBroadcastReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent == null) return;

            // Extraction du résultat de géolocalisation depuis l'Intent
            if (LocationResult.HasResult(intent))
            {
                var result = LocationResult.ExtractResult(intent);
                var lastLocation = result?.LastLocation;

                if (lastLocation is not null)
                {
                    Log.Debug("GeoAndroidService", $"Provider : {lastLocation.Provider} " +
                        $"- Time : {DateTimeOffset.FromUnixTimeMilliseconds(lastLocation.Time).ToLocalTime():dd/MM/yyyy}");
                    
                    // Traite le point GPS ici (ex. enregistrement BDD local ou envoi à un ViewModel)
                    var loc = new Location(lastLocation.Latitude, lastLocation.Longitude)
                    {
                        
                        Accuracy = lastLocation.Accuracy,
                        Altitude = lastLocation.Altitude,
                        AltitudeReferenceSystem = AltitudeReferenceSystem.Ellipsoid,
                        // -- conversion de milliseconde Unix mesure en DateTimeOffset localtime
                        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(lastLocation.Time).ToLocalTime(),
                        Speed = lastLocation.Speed,
                        VerticalAccuracy =  // -- ne marche pas pour les versions android en dessous de 26
                            OperatingSystem.IsAndroidVersionAtLeast(26) ? (double)(lastLocation.VerticalAccuracyMeters) : 0,
                        ReducedAccuracy = false,    // -- ne marche que pour IOS
                        Course = lastLocation.Bearing
                    };
                    Log.Debug("GeoAndroidService", "\n -------------------------------------------------------");
                    Log.Debug("GeoAndroidService", $"location longitude {loc.Longitude} au niveau LocationBroadcastReceiver");
                    // -- Traitement de la position envoi vers l'interface IServiceGeolocalisation
                    // -- en utilisant un système de messenger
                    WeakReferenceMessenger.Default.Send(new LocationBroadcastMessage(loc));
                }
            }
        }
    }
}
#endif
