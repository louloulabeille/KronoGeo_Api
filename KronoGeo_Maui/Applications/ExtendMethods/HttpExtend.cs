using KronoGeo_Api.Infrastructure.Service.HttpClient;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Polly;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class HttpExtend
    {
        extension(IConfigurationBuilder builder)
        {
            /// <summary>
            /// Récupération du fichier json appsetting dans "resources raw"
            /// et injection dans builder.configuration
            /// </summary>
            /// <returns></returns>
            public IConfigurationBuilder AddAppsettingsConfiguration()
            {
                using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;

                var config = new ConfigurationBuilder().AddJsonStream(stream).Build();

                builder.AddConfiguration(config);

                return builder;
            }
        }

        extension ( IServiceCollection services)
        {
            /// <summary>
            /// IOptions UrlApi config
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public IServiceCollection AddUrlApiOptions( IConfiguration config )
            {
                services.AddOptions();
                services.Configure<UrlApi>(config.GetSection("API"));
                return services;
            }

            /// <summary>
            /// ajoute le service AddHttpClient en injection de dépendance
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddHttpClientService(IConfiguration config )
            {

                services.AddHttpClient<IServiceHttpKronoGeo, HttpClientKronoGeo>((serviceProvider, client) =>
                { 
                    
                });
                return services;
            }



        }
    }
}
