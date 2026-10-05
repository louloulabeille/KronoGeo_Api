using KronoGeo_Api.Infrastructure.Service.Map;
using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class SharedServicesExtends
    {
        extension( IServiceCollection services)
        {
            /// <summary>
            /// ajout les services qui doivent être appellé en commun entre le client et le serveur
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddSharedServices()
            {
                services.AddScoped<IMapStateService, MapStateService>();
                return services;
            }
        }
    }
}
