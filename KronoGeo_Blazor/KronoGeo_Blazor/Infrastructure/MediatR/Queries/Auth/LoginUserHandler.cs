using BruTile.Cache;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Auth
{
    public class LoginUserHandler(IServiceHttpKronoGeo httpKronoGeo
        , IMemoryCache memoryCache, ILogger<LoginUserHandler> logger) 
        : AuthHandler<LoginUserHandler>( httpKronoGeo, memoryCache, logger)
        , IRequestHandler<LoginUserCommand, ResponseApiAuthenticate>
    {
        #region public method interface IRequestHandler
        public async Task<ResponseApiAuthenticate> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            if ( HttpKronoGeo is not null)
            {
                // -- requete vers l'APi
                var result = await HttpKronoGeo.AuthenticateAsync(request.Register);
                return result;
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
