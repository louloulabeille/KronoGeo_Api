using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Maui.Applications.Services.Geolocation;
using KronoGeo_Maui.Applications.Factory.Geolocalisation;



#if ANDROID
using KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation;
#endif

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class GeolocalisationExtends 
    {
        extension ( IServiceCollection services )
        {
            /// <summary>
            /// method pour ajouter les services de géolocation et la factory qui va avec 
            /// à charger après le service de Paramétrage
            /// </summary>
            /// <param name="parametrage"></param>
            /// <returns></returns>
            public IServiceCollection AddCharginGeolocation(IServiceSaveParametrage parametrage )
            {
                // -- ajout des différents services de géolocation
#if ANDROID26_0_OR_GREATER
                var isLocationManager =  (bool)parametrage.GetParam("IsLocationManager", true);

                // -- Android 26
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {   
                    // -- marche très bien sauf en arrière plan très bonne précision - utilise beaucoup de batterie
                    services.AddScoped<IServiceGeolocalisation, GeolocationAndroid>();
                
                    // -- fused marche très bien en arrière plan - moins précis que LocationManager - utilise tous les réseaux non filaires possible wifi etc 
                    // -- marche moins en campagne
                    services.AddScoped<IServiceGeolocalisation, FusedLocationAndroid>();
                }
#elif ANDROID21_0_OR_GREATER
                services.AddScoped<IServiceGeolocalisation, GeolocationOther>();
#elif !ANDROID
                services.AddScoped<IServiceGeolocalisation, GeolocationOther>();
#endif
                // -- ajout du factory
                services.AddScoped<FactoryGeolocation>();

                return services;
            }

        }
    }
}
