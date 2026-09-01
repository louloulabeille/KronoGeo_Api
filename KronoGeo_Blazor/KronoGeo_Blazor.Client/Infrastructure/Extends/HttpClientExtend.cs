using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Options;
using Polly;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class HttpClientExtend
    {
        extension ( IServiceCollection services) {

            /// <summary>
            /// ajout des Url Api pour le client
            /// ne pas mettre de fichier de config, tout est en clair au 
            /// niveau client
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddUrlApiExtend()
            {
                services.AddOptions();
                services.Configure<UrlApiBlazorClient>(options => {
                    options.Login = "api/v1/AuthBFF/Login";
                    options.Me = "api/v1/AuthBFF/Me";
                });
                return services;
            }

            /// <summary>
            /// Déclaration du HttpClient pour le client pour l'injection de dépendance 
            /// du service avec comme base adresse l'adresse du serveur blazor par défaut
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public IServiceCollection AddHttpClientBFF (WebAssemblyHostBuilder builder )
            {
                // -- configuration de Httpclient avec l'adresse de base de url donc de lui même 
                services.AddHttpClient<IServiceHttpClientAssembly, HttpBlazorClient>((serviceProvider, client) => {
                    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                })
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;
                
                services.AddScoped<IServiceHttpKronoGeo>(sp => sp.GetRequiredService<IServiceHttpClientAssembly>());
                return services;
            }
        }
    }
}
