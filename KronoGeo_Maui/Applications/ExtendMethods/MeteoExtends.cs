using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Infrastructure.Service.Http.Meteo;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
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
                services.Configure<UrlApiMeteo>(config.GetSection("UrlApiMeteo"));
                return services;
            }

            /// <summary>
            /// injection du service HttpClient Meteo
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddHttpClientMeteo ()
            {
                // -- service métier HttpClient
                services.AddHttpClient<IServiceApiMeteo, HttpClientOpenMeteo>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiMeteo>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })
                    // - nombre d'essai 5 et intervalle en milliseconde i = (50 + i*150)
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 5,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));

                return services;
            }

        }
    }
}
