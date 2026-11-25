using KronoGeo_Api.Models.Infrastructure.Options;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class UrlExtends
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// url pour charger les images dans l'application
            /// </summary>
            /// <param name="configuration"></param>
            /// <returns></returns>
            public IServiceCollection AddIoptionsUrlImages(IConfiguration configuration)
            {
                services.AddOptions();
                services.Configure<UrlImage>(configuration.GetSection("UrlImage"));
                return services;
            }

            /// <summary>
            /// url pour charger les maps de Open Street Map à partir du proxy sur le serveur blazor
            /// </summary>
            /// <param name="configuration"></param>
            /// <returns></returns>
            public IServiceCollection AddIoptionsUrlProxyTile(IConfiguration configuration)
            {
                services.AddOptions();
                services.Configure<UrlProxyOpenStreetMap>(configuration.GetSection("UrlProxyOpenStreetMap"));
                return services;
            }
        }
    }
}
