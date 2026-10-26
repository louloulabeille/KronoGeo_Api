using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Blazor.Infrastructure.Http;
using Microsoft.AspNetCore.Components.Server.Circuits;
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
                // -- service pour lire le HttpContext en cours dans le handler
                services.AddHttpContextAccessor();
                // -- service pour injecter IMemoryCache appelé dans la classe TokenHeaderHandler
                services.AddMemoryCache();
                // -- injection de la classe TokenHeaderHandler
                // -- le mettre avant AddHttpClient
                services.AddTransient<TokenHeaderHandler>();
                // -- injection de UnauthorizedRedirectHandler pour pouvoir l'utiliser par la suite
                //services.AddTransient<UnauthorizedRedirectHandler>();
                // -- injection et options de l'injection du HttpClient
                services.AddHttpClient<IServiceHttpKronoGeo, HttpClientKronoGeo>("api",(serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiMetier>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);

                })
                    .AddHttpMessageHandler<TokenHeaderHandler>() //-- ajoue du handler qui va chercher le token en memoire selon la session id de l'utilisateur
                    //.AddHttpMessageHandler<UnauthorizedRedirectHandler>()
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;
                
                // -- injection du service httpclient Manager 
                services.AddHttpClient<IServiceHttpManagerKronoGeo, HttpClientManagerKronogeo>("apiManager", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiManager>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);

                })
                    .AddHttpMessageHandler<TokenHeaderHandler>() //-- ajoue du handler qui va chercher le token en memoire selon la session id de l'utilisateur
                    //.AddHttpMessageHandler<UnauthorizedRedirectHandler>()
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;

                return services;
            }



            /// <summary>
            /// Déclaration du HttpClient pour le client pour l'injection de dépendance 
            /// du service avec comme base adresse l'adresse du serveur blazor par défaut
            /// Mise en place sur le serveur pour le pré-rendu
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public IServiceCollection AddHttpClientBFF()
            {
                // -- configuration de Httpclient avec l'adresse de base de url donc de lui même 
                services.AddHttpClient<IServiceHttpClientAssembly, HttpBlazorClient>("ApiBff", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiBlazorClient>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;

                return services;
            }

        }

    }
}
