using CommunityToolkit.Maui;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.ExtendMethods;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Services;
using KronoGeo_Maui.Applications.Services.Geolocation;
using Microsoft.Extensions.Logging;
using KronoGeo_Api.Infrastructure.Service.Telemetry;
using KronoGeo_Api.Infrastructure.Service.Secours;
using The49.Maui.BottomSheet;




#if ANDROID
using KronoGeo_Maui.Platforms.Android.Application.Geolocalisation;
#endif

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
                // pour afficher les cartes de Microsoft.Maui.Controls.Maps
                .UseMauiMaps()
                 // Initialize the .NET MAUI Community Toolkit CameraView by adding the below line of code
                 //.UseMauiCommunityToolkitCamera()
                 // Initialize the The49.Maui.BottomSheet librairi
                 .UseBottomSheet()
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
            builder.Services.AddHttpClientService();
            #endregion

            #region injection pour la sauvegarde en memoire de l'utilisateur
            builder.Services.AddScoped<IServiceSaveUser,InMemoriMauiUser>();
            #endregion

            #region Injection Geolocation
#if ANDROID26_0_OR_GREATER
                // -- Android 26
                if (OperatingSystem.IsAndroidVersionAtLeast(26)) 
                    builder.Services.AddSingleton<IServiceGeolocalisation, GeolocationAndroid>();
#elif ANDROID21_0_OR_GREATER
                builder.Services.AddSingleton<IServiceGeolocalisation, GeolocationOther>();
#elif !ANDROID
                builder.Services.AddSingleton<IServiceGeolocalisation, GeolocationOther>();
#endif
            #endregion

            #region Camera Injection
            builder.Services.AddServiceCamera();
            #endregion

            #region injection divers - ajouter une method pour tout ces services en injection
            //builder.Services.AddTransient<IServiceSaveLocalisation, InMemorySaveLocalisation>();
            builder.Services.AddTransient<IServiceSaveLocalisation, InApiSaveLocalisation>();
            builder.Services.AddTransient<IDialogService, MauiDialogService>();
            builder.Services.AddScoped<IServiceSaveParametrage, InMemoryMauiParametrage>();
            builder.Services.AddTransient<IServiceTelemetry, ServiceTelemetry>();
            builder.Services.AddSingleton<IServiceBackupGps, GpsBackUpMauiService>();
            #endregion

            #region injection en développent du token tunnel de développement sécurisé
#if DEBUG
            builder.Services.AddTokenTunnelDeveloppement(builder.Configuration);
#endif
            #endregion

            return builder.Build();
        }
    }
}
