using BruTile.Cache;
using KronoGeo_Api.Infrastructure.Applications.Cookies;
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
    public class LoginUserHandler(IServiceHttpManagerKronoGeo httpKronoGeo
        , ILogger<LoginUserHandler> logger) 
        : AuthHandler<LoginUserHandler>( httpKronoGeo, logger)
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
                        (retour.ClaimsPrincipal, retour.AuthenticationProperties) 
                            = HttpOnlyCookie.BuildCookie(result.Register.Token
                            , result.Register.Id??string.Empty, result.Register.Login);
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
