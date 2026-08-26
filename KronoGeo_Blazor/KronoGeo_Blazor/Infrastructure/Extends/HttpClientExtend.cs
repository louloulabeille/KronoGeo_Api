using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Polly;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class HttpClientExtend
    {
        extension( IServiceCollection services )
        {
            /// <summary>
            /// method pour ajouter l'injection de dépendance pour la liaison de donnée par HttpClient
            /// avec ajout DelegatingHandler -> TokenHeaderHandler
            /// qui injecter le token dans le header de la requete HttpClient vers le Api
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddHttpClientExtend()
            {
                services.AddHttpClient<IServiceHttpKronoGeo, HttpClientKronoGeo>((serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApi>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);

                })
                    .AddHttpMessageHandler<TokenHeaderHandler>()
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;
                return services;
            }
        }

    }
}
