using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;

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
                services.Configure<UrlApiMetier>(config.GetSection("ApiMetier"));
                services.Configure<UrlApiManager>(config.GetSection("ApiManager"));
                return services;
            }

            /// <summary>
            /// ajoute le service AddHttpClient en injection de dépendance
            /// avec l'injection de IOptions-UrlApi- pour récupérer l'url de base de l'API
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddHttpClientService()
            {
                // - HttpClient utilisé par injection de dépendance
                
                // -- service métier HttpClient
                services.AddHttpClient<IServiceHttpKronoGeo, HttpClientKronoGeo>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiMetier>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })  // - nombre d'essai 5 et intervalle en milliseconde i = (50 + i*150)
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 5,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));

                // -- service http pour toutes les connexion vers les service de manager
                // -- possiblité d'utiliser 2 serveurs différents
                services.AddHttpClient<IServiceHttpManagerKronoGeo, HttpClientManagerKronogeo>((serviceProvider, client) =>
                {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiManager>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })  // - nombre d'essai 5 et intervalle en milliseconde i = (50 + i*150)
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 5,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));


                return services;
            }



        }
    }
}
