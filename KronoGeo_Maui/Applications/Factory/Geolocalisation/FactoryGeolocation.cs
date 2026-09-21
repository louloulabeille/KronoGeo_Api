using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.Interface;

using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Maui.Applications.Services.Geolocation;


#if ANDROID
using KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation;
#endif

namespace KronoGeo_Maui.Applications.Factory.Geolocalisation
{
    public class FactoryGeolocation (IServiceSaveParametrage saveParametrage
        , IServiceProvider serviceProvider)
    {
        #region private readonly properties
        private readonly IServiceSaveParametrage _saveParametrage = saveParametrage;
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        #endregion

        public IServiceGeolocalisation GetServiceGeolocalisation()
        {
#if ANDROID26_0_OR_GREATER
            var isLocationManager = (bool)_saveParametrage.GetParam("IsLocationManager", true);

            // -- Android 26
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                if (isLocationManager)
                {
                    // -- marche très bien sauf en arrière plan très bonne précision - utilise beaucoup de batterie
                    return _serviceProvider.GetRequiredKeyedService<IServiceGeolocalisation>("LocationManager");
                }
                else
                {
                    // -- fused marche très bien en arrière plan - moins précis que LocationManager - utilise tous les réseaux non filaires possible wifi etc 
                    // -- marche moins en campagne
                    return _serviceProvider.GetRequiredKeyedService<IServiceGeolocalisation>("Fused");
                }

            }
            return _serviceProvider.GetRequiredKeyedService<IServiceGeolocalisation>("Other");

#elif ANDROID21_0_OR_GREATER
                return _serviceProvider.GetRequiredKeyedService<IServiceGeolocalisation>("Other");
#elif !ANDROID
                return _serviceProvider.GetRequiredKeyedService<IServiceGeolocalisation>("Other");
#endif

        }
    }
}
