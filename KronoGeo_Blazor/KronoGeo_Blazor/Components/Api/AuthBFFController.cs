using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Parameter;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Xml.Linq;


namespace KronoGeo_Blazor.Components.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]
    public class AuthBFFController(  IMediator mediaR
        , ILogger<AuthBFFController> logger ) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        //private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<AuthBFFController> _logger = logger;
        #endregion

        /// <summary>
        /// Retourne les infos de session cookie du httpOnly
        /// vers le webAssembly
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("Me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            if ( User.Identity?.IsAuthenticated != true )
            {
                return Ok(new UserInfos()
                {
                    IsAuthenticate = false,
                    Id = string.Empty,
                });
            }

            var info = new UserInfos()
            {
                IsAuthenticate = true,
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty,
                Login = User.Identity.Name ?? string.Empty,
                Roles = [.. User.FindAll(ClaimTypes.Role).Select(r => r.Value)],
                Claims = User.Claims.Where(c => c.Type != ClaimTypes.Name &&
                    c.Type != ClaimTypes.Role && c.Type != ClaimTypes.Name)
                    .ToDictionary(c => c.Type, c => c.Value)
            };

            return Ok(info);
        }
    }
}
