using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace KronoGeo_Blazor.Infrastructure.Extends.App
{
    public static class EndPointLogOutExtends
    {
        extension(IEndpointRouteBuilder app)
        {
            public IEndpointRouteBuilder AddEndPointLogOut()
            {
                // -- déconnexion par un button post
                app.MapPost("/Account/Logout", async (HttpContext ctx, [FromForm] string? returnUrl) =>
                {
                    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Results.LocalRedirect("/");
                }).AllowAnonymous()
                .DisableAntiforgery();

                // -- deconnexion avec une requete get
                app.MapGet("/Account/Logout", async (HttpContext ctx) =>
                {
                    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    return Results.LocalRedirect("/Account/authenticate");
                }).AllowAnonymous()
                .DisableAntiforgery();

                return app;
            }
        }
    }
}
