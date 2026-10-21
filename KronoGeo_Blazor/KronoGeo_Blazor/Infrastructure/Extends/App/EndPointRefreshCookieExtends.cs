using KronoGeo_Api.Infrastructure.Applications.Cookies;
using KronoGeo_Api.Models.Infrastructure.Cookie;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using System.Security.Principal;

namespace KronoGeo_Blazor.Infrastructure.Extends.App
{
    public static class EndPointRefreshCookieExtends
    {
        extension(IEndpointRouteBuilder app)
        {
            public IEndpointRouteBuilder BuildEndPointRefresh ()
            {
                // End Point
                app.MapGet("/Account/refresh", async (HttpContext ctx, IMemoryCache cache,
                    string key, string? returnUrl) =>
                {
                    // -- récupération des données pour le refactorisé le coockie
                    // -- si null alors on revient au niveau du login
                    if (!cache.TryGetValue(key, out RefreshCookieData? data) || data is null)
                        return Results.Redirect("/Account/Logout");
                    cache.Remove(key); // usage unique
                     // -- refactorisation des données pour le refactisation du cookie
                    var result = HttpOnlyCookie.BuildCookie(data.Token, data.Id, data.Login);
                    ClaimsPrincipal principal = result.Item1;
                    AuthenticationProperties props = result.Item2;
                    // -- envoie du cookie vers le navigateur
                    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);
                    // -- retourne vers url
                    return Results.LocalRedirect(returnUrl ?? "/");
                }).RequireAuthorization();

                return app;
            }
        }
    }
}
