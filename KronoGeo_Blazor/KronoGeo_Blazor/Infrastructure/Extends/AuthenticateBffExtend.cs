using KronoGeo_Api.Infrastructure.Service.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class AuthenticateBffExtend
    {
        extension ( IServiceCollection services )
        {
            /// <summary>
            /// method qui ajoute les options d'autehntication sur le serveur Blazor
            /// pour la mise en place du design pattern Bff (backend for frontend)
            /// en le serveur  blazor et web assembly (client blazord)
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddAuthentifcateBffServeur()
            {
                services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => 
                    {
                        options.Cookie.Name = "BffCookie";
                        options.Cookie.HttpOnly = true;
                        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                        options.Cookie.SameSite = SameSiteMode.Strict;
                        options.ExpireTimeSpan = TimeSpan.FromHours(6); 

                        // -- vérification si le token est toujours valide
                        options.Events.OnValidatePrincipal = async ctx =>
                        {
                            var token = ctx.Properties.GetTokenValue("jwt_token");
                            var jwt = string.IsNullOrEmpty(token)
                                ? null
                                : new JwtSecurityTokenHandler().ReadJwtToken(token);

                            if (jwt is null || jwt.ValidTo < DateTime.UtcNow)
                            {
                                ctx.RejectPrincipal();
                                await ctx.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            }
                        };
                    });

                services.AddHttpContextAccessor();

                services.AddAuthorization();
                //services.AddScoped<AuthenticationStateProvider, BffAuthentificationStateProvider>();
                //services.AddCascadingAuthenticationState();

                return services;
            }
        }
    }
}
