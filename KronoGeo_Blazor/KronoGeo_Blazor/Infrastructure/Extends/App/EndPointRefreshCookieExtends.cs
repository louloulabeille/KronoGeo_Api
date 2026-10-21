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

                // Program.cs
                app.MapGet("/account/refresh", async (HttpContext ctx, IMemoryCache cache,
                    string key, string? returnUrl) =>
                {
                    if (!cache.TryGetValue(key, out RefreshCookieData? data) || data is null)
                        return Results.Redirect("/Account/Login");
                    cache.Remove(key); // usage unique
                     
                    var result = HttpOnlyCookie.BuildCookie(data.Token, data.Id, data.Login);
                    ClaimsPrincipal principal = result.Item1;
                    AuthenticationProperties props = result.Item2;
                    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, props);

                    return Results.LocalRedirect(returnUrl ?? "/");
                }).RequireAuthorization();

                return app;
            }
        }
    }
}
