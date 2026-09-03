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

            /// <summary>
            /// ajout des Url Api pour le client web-assembly
            /// ne pas mettre de fichier de config, tout est en clair au 
            /// niveau client
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddUrlApiExtend()
            {
                services.AddOptions();
                services.Configure<UrlApiBlazorClient>(options => {
                    options.BasicAdress = "https://localhost:7186";
                    options.Login = "api/v1/AuthBFF/Login";
                    options.Me = "api/v1/AuthBFF/Me";
                    options.Logout = "api/v1/AuthBFF/Logout";
                });
                return services;
            }
        }
    }
}
