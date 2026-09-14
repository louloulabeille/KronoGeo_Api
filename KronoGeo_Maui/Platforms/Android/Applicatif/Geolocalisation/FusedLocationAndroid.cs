#if ANDROID
using Android.App;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Java.Lang;
using Kotlin.Jvm.Internal;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
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
    public class FusedLocationAndroid : IServiceGeolocalisation
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

        ///  inclusion des packages qui ne sont pas  a jour pour Xamarin.GooglePlayServices.Location 
		/// avec mes versions de packages Maui à jour en.net10
        /// Xamarin.AndroidX.Collection.Jvm dans mon projet est 1.6.0.1
		/// et dans Xamarin.GooglePlayServices.Location il veut 1.5.0.1 de même 
        /// pour Xamarin.AndroidX.Lifecycle.Runtime


        #region private properties
        // -- mise en place d'un handler thread pour lancer la géolocalisation sur un thread parallèle au thread principal
        private readonly HandlerThread _handlerThread = new("LocationHandlerthread");
        private Handler? _handler;
        private LocationCallback? _locationCallback = default;
        private readonly IFusedLocationProviderClient? _locationClient = LocationServices.GetFusedLocationProviderClient(Application.Context);
        #endregion


        #region public properties interface IServiceGeolocalisation
        public bool Pause { get; set; } // -- modifié pour faire mettre en arrêt le systeme lors des pauses

        public event EventHandler<GeolocationLocationChangedEventArgs>? LocationChanged;
        public event EventHandler<GeolocationListeningFailedEventArgs>? ListeningFailed;
        #endregion

        #region pulic method interface IServiceGeolocalisation
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Donne le current location en passant par FusedLocation et la package Xamarin.GooglePlayService.Location
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Location?> GetCurrentLocationAsync(CancellationToken token)
        {
            // -- création de l'object pour custom la location request 
            var request = new CurrentLocationRequest.Builder().SetPriority(Priority.PriorityHighAccuracy)
            .SetDurationMillis(10000)
            .Build();

            // -- mise en place du task pour traiter l'api google avec une méthode asynchrone de c#
            var tcs = new TaskCompletionSource<Location?>();

            using (token.Register(() => tcs.TrySetCanceled()))
            {
                // Appel de la méthode Java native
                var javaTask = _locationClient?.GetCurrentLocation(request, null);

                // Conversion en Task C# via les listeners Java
                javaTask?.AddOnSuccessListener(new OnSuccessListener(location =>
                {
                    // -- récuépration de location android 
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

        }

        /// <summary>
        /// method qui lance la prise des localisations en continue
        /// </summary>
        public void StartLocationUpdatesAsync()
        {
            try
            {
                if (LocationChanged is not null)
                {
                    // -- initial handler thread
                    // sert à lancer dans un autre thread que le principal
                    _handlerThread.Start(); 
                    if (_handlerThread.Looper is null)
                    {
                        Log.Error("GeoAndroidService", "handlerThread.looper est null");
                        return;
                    }
                    _handler = new(_handlerThread.Looper);

                    // -- object du custom request 
                    var locationRequest = new LocationRequest.Builder(Priority.PriorityHighAccuracy, 5000) // 5 sec
                    .SetMinUpdateIntervalMillis(2000)
                    .Build();

                    // -- traitement à faire pour le retour du Callback de base ici 
                    // -- un callback spécifique
                    _locationCallback = new CustomLocationCallback(location =>
                    {
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

                        Log.Debug("GeoAndroidService", $"Latitude : {location.Latitude}, Longitude : {location.Longitude}");
                        // -- appel de eventhandler pour appeler le code qui doit être traité
                        LocationChanged?.Invoke(this, new GeolocationLocationChangedEventArgs(locationSmoother!));
                    });

                    // -- appel de la fonction du lancement de l'écoute
                    _locationClient?.RequestLocationUpdates(locationRequest, _locationCallback, _handler.Looper);
                }


                //Log.Error("GeoAndroidService", "Le fournisseur GPS n'est pas activé sur l'appareil. {}");
                ////System.Diagnostics.Debug.WriteLine("Le fournisseur GPS n'est pas activé sur l'appareil.");
                //throw new FeatureNotEnabledException("Le fournisseur GPS n'est pas activé sur l'appareil.");  

            }
            catch (Java.Lang.SecurityException ex)
            {
                Log.Error("GeoAndroidService", $"Permission de localisation refusée. Veuillez accorder les permissions nécessaires \n {ex.Message}");
                //System.Diagnostics.Debug.WriteLine($"Erreur de permission : {ex.Message}");
                throw new PermissionException($"Permission de localisation refusée. Veuillez accorder les permissions nécessaires.");
            }
        }

        /// <summary>
        /// Arrête le processus de prise de localisation
        /// il peut être prise en compte pour mettre en pause aussi
        /// </summary>
        public void StopLocationUpdates()
        {
            if (_locationClient != null && _locationCallback != null)
            {
                _locationClient.RemoveLocationUpdates(_locationCallback);
                _locationCallback = null;
            }
        }

        #endregion

        #region internal method
        // Helpers pour les listeners Java/Android Tasks vers c#
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

    public class CustomLocationCallback(Action<global::Android.Locations.Location> onLocationReceived) : LocationCallback
    {
        private readonly Action<global::Android.Locations.Location> _onLocationReceived = onLocationReceived;

        public override void OnLocationResult(LocationResult result)
        {
            if (result?.LastLocation != null)
                _onLocationReceived(result.LastLocation);
        }
    }
}
#endif
