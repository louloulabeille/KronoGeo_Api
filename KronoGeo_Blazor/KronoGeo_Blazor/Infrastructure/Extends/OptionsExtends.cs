using KronoGeo_Api.Models.Infrastructure.Options;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class OptionsExtends
    {
        extension(IServiceCollection services)
        {
            /// <summary>
            /// récupération et injection des adresses de l'API
            /// </summary>
            /// <param name="configuration"></param>
            /// <returns></returns>
            public IServiceCollection AddUrlApiExtend(IConfiguration configuration)
            {
                services.AddOptions();
                services.Configure<UrlApi>(configuration.GetSection("Api"));
                return services;
            }
        }
    }
}
