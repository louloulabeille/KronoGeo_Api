using KronoGeo_Api.Infrastructure.Service.Http;
using Microsoft.AspNetCore.Components.Authorization;

namespace KronoGeo_Blazor.Client.Infrastructure.Extends
{
    public static class AutorizationExtend
    {
        extension (IServiceCollection services)
        {
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
