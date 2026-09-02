using KronoGeo_Api.Infrastructure.Service.Http;
using Microsoft.AspNetCore.Components.Authorization;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class AutorizationExtend
    {
        extension (IServiceCollection services)
        {
            /// <summary>
            /// Ajoute les services nécessaires pour l'authentification et l'autorisation dans une application Blazor.
            /// AuthenticationStateProvider contient le mécanisme pour gérer l'état d'authentification de l'utilisateur.
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddAutorizationClient()
            {
                services.AddAuthorizationCore();
                services.AddScoped<AuthenticationStateProvider, BffAuthentificationStateProvider>();
                services.AddCascadingAuthenticationState();
                return services;
            }
        }
    }
}
