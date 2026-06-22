using CommunityToolkit.Maui;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.ExtendMethods;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Services;
using KronoGeo_Maui.Applications.Services.Geolocation;
using KronoGeo_Maui.Platforms.Android.Application.Geolocalisation;
using Microsoft.Extensions.Logging;

namespace KronoGeo_Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                // Initialize the .NET MAUI Community Toolkit by adding the below line of code
                .UseMauiCommunityToolkit()
                .UseMauiMaps() // pour afficher les cartes de Microsoft.Maui.Controls.Maps
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons-Regular"); // - matériel design
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            #region MvvM Injection
            builder.Services.AddMvvmInjection();
            #endregion

            #region AddHttpClient
            builder.Configuration.AddAppsettingsConfiguration();    // - ajout d'un fichier de configuration dans Iconfiguration
            builder.Services.AddUrlApiOptions( builder.Configuration );
            builder.Services.AddHttpClientService( builder.Configuration);
            #endregion

            #region injection pour la sauvegarde en memoire de l'utilisation
            builder.Services.AddScoped<IServiceSaveUser,InMemoriMauiUser>();
            #endregion

            #region Injection Geolocation
            #if ANDROID
            // -- android 26
                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                    builder.Services.AddSingleton<IServiceGeolocalisation, GeolocationAndroid>();
                else
                    builder.Services.AddSingleton <IServiceGeolocalisation,GeolocationOther>();
            #endif

            #if !ANDROID
                builder.Services.AddSingleton <IServiceGeolocalisation,GeolocationOther>();
            #endif

            #endregion

            builder.Services.AddSingleton<IDialogService, MauiDialogService>();
            builder.Services.AddScoped<IServiceSaveParametrage, InMemoryMauiParametrage>();

            return builder.Build();
        }
    }
}
