using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class MeteoExtends
    {
        extension( IServiceCollection services )
        {
            /// <summary>
            /// Attention à mettre après AddAppsettingsConfiguration() qui va charger dans Iconfiguration 
            /// le fichier de config de l'application Maui
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public IServiceCollection AddIoptionsParamMeteo (IConfiguration config)
            {
                services.AddOptions();
                services.Configure<ParamMeteo>(config.GetSection("ParamMeteo"));
                return services;
            }
        }
    }
}
