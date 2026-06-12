using KronoGeo_Maui.Applications.ExtendMethods;
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
            builder.Configuration.AddAppsettingsConfiguration();
            builder.Services.AddUrlApiOptions( builder.Configuration );
            builder.Services.AddHttpClientService( builder.Configuration);
            #endregion

            return builder.Build();
        }
    }
}
