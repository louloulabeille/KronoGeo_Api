using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Polly;
using static Microsoft.Maui.Authentication.AppleSignInAuthenticator;

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
                // -- injection de SessionNotificationService qui est utilisé dans UnauthorizedRedirectHandler
                //services.AddScoped<SessionNotificationService>();
                // -- injection de la classe TokenHeaderHandler
                // -- le mettre avant AddHttpClient
                services.AddTransient<TokenHeaderHandler>();
                // -- injection de UnauthorizedRedirectHandler pour pouvoir l'utiliser par la suite
                services.AddScoped<UnauthorizedRedirectHandler>();
                // -- injection et options de l'injection du HttpClient
                services.AddHttpClient<IServiceHttpKronoGeo, HttpClientKronoGeo>("api",(serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiMetier>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);

                })
                    .AddHttpMessageHandler<TokenHeaderHandler>() //-- ajoue du handler qui va chercher le token en memoire selon la session id de l'utilisateur
                    .AddHttpMessageHandler<UnauthorizedRedirectHandler>()
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150))); ;
                
                // -- injection du service httpclient Manager 
                services.AddHttpClient<IServiceHttpManagerKronoGeo, HttpClientManagerKronogeo>("apiManager", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiManager>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);

                })
                    .AddHttpMessageHandler<TokenHeaderHandler>() //-- ajoute du handler qui va chercher le token en memoire selon la session id de l'utilisateur
                    .AddHttpMessageHandler<UnauthorizedRedirectHandler>()
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
                /*services.AddHttpClient<HttpBlazorClient>("ApiBff", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiBlazorClient>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));

                services.AddTransient<IServiceHttpClientAssembly>(sp => sp.GetRequiredService<HttpBlazorClient>());
                services.AddTransient<IServiceHttpManagerClientAssembly>(sp => sp.GetRequiredService<HttpBlazorClient>());
*/

                // -- configuration de Httpclient avec l'adresse de base de url donc de lui même 
                services.AddHttpClient<IServiceHttpClientAssembly, HttpBlazorClient>("ApiBff", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiBlazorClient>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));


                /*// -- ajout du service IServiceHttpManagerClientAssembly
                services.AddHttpClient<IServiceHttpManagerClientAssembly, HttpBlazorClient>("ApiManagerBff", (serviceProvider, client) => {
                    var options = serviceProvider.GetRequiredService<IOptions<UrlApiBlazorClient>>();
                    client.BaseAddress = new Uri(options.Value.BasicAdress);
                })
                    .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(
                        retryCount: 3,
                        retryNumber => TimeSpan.FromMilliseconds(50 + retryNumber * 150)));*/

                return services;
            }

        }

    }
}
