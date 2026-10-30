using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Infrastructure.Service.Map;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Polly;

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

                services.AddOptions();
                services.Configure<UrlApiBlazorClient>(options => {
                    options.BasicAdress = "https://localhost:7186";
                    options.Login = "api/v1/AuthBFF/Login";
                    options.Me = "api/v1/AuthBFF/Me";
                    options.Logout = "api/v1/AuthBFF/Logout";
                    options.GetUserGroupLocalisation = "api/v1/GpsBFF/GetAllGroup";
                    options.UpdateImage = "api/v1/ImagesBFF/UpdateImage";
                    options.DeleteImage = "api/v1/ImagesBFF/DeleteImage";
                    options.GetLocalisationsById = "api/v1/GpsBFF/GetLocalisationsById";
                    options.UpdateLocalisationGroup = "api/v1/GpsBFF/UpdateLocalisationGroup";
                    options.DeleteLocalisationGroup = "api/v1/GpsBFF/DeleteLocalisationGroup";
                });
                // -- ajout du service ToastsService pour l'affichage des notifications toast
                // -- javascript interop pour l'affichage des notifications toast
                services.AddScoped<ToastsService>();

                return services;
            }

        }
    }
}
