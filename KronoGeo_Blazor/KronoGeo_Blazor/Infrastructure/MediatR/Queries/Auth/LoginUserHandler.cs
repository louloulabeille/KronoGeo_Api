using BruTile.Cache;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Auth
{
    public class LoginUserHandler(IServiceHttpKronoGeo httpKronoGeo
        , IMemoryCache memoryCache, ILogger<LoginUserHandler> logger) 
        : AuthHandler<LoginUserHandler>( httpKronoGeo, memoryCache, logger)
        , IRequestHandler<LoginUserCommand, ResponseApiAuthenticateBlazor>
    {
        #region public method interface IRequestHandler
        public async Task<ResponseApiAuthenticateBlazor> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if ( HttpKronoGeo is not null)
            {
                // -- requete vers l'APi
                var result = await HttpKronoGeo.AuthenticateAsync(request.Register);
                ResponseApiAuthenticateBlazor retour = new()
                {
                    Message = result.Message,
                    ApiStatus = result.ApiStatus,
                    Register = result.Register
                };

                if (result.IsSuccess)
                {
                    if (!string.IsNullOrEmpty(result.Register?.Token))
                    { 
                        // -- mise en place de httponly cookie 


                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadJwtToken(result.Register.Token);

                        // -- extraction du role pour l'utilisateur
                        var roles = jwtToken.Claims.Where(r => r.Type == "role").ToList();

                        // -- Émettre le Cookie d'authentification vers le navigateur
                        var claims = new List<Claim>
                        {
                            new (ClaimTypes.Name, result.Register.Login )
                        };

                        // -- ajout du ou des roles dans le cookie
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.Value));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims
                            , CookieAuthenticationDefaults.AuthenticationScheme);

                        // -- ajout dans le cookie le token jwt
                        var authProperties = new AuthenticationProperties();
                        authProperties.StoreTokens(
                        [
                            new AuthenticationToken { Name = "jwt_token", Value = result.Register.Token}
                        ]);

                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        
                        retour.ClaimsPrincipal = claimsPrincipal;
                        retour.AuthenticationProperties = authProperties;
                    }
                }
                return retour;
            }


            return new()
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = "Le service Http n'a pas pu être chargé."
            };
        }
        #endregion
    }
}
